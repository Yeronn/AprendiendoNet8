using System.ComponentModel.DataAnnotations;
using Application.DTOs.LoginAudit;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;

namespace Application.Services
{
    public class LoginAuditService : ILoginAuditService
    {
        private readonly ILoginAuditRepository _loginAuditRepository;

        public LoginAuditService(ILoginAuditRepository loginAuditRepository)
        {
            _loginAuditRepository = loginAuditRepository;
        }


        public async Task<LoginAuditDto?> GetLoginAuditByTokenIdAsync(string tokenId)
        {
            var loginAudit = await _loginAuditRepository.GetLoginAuditByTokenIdAsync(tokenId);
            return loginAudit?.ToDto();
        }


        public async Task<LoginAuditResponseDto> CreateLoginAuditAsync(LoginAuditDto loginAudit)
        {
            string validationResults = ValidateLoginAuditDto(loginAudit);
            if (!string.IsNullOrWhiteSpace(validationResults))
                return new LoginAuditResponseDto(false, $"Errores de validación: {validationResults}");
            
            if (loginAudit.IsAccessToken)
            {
                bool revokedPreviousTokens = await RevokeAllTokensAsync(loginAudit.IdCCNit);
                if (!revokedPreviousTokens)
                    return new LoginAuditResponseDto(false, "No se pudo revocar los anteriores tokens");
            }
            else if (!loginAudit.IsAccessToken)
            {
                bool deletedRefreshTokens = await DeleteRefreshTokensAsync(loginAudit.IdCCNit);
                if (!deletedRefreshTokens)
                    return new LoginAuditResponseDto(false, "No se pudo eliminar los anteriores refresh tokens");
            }

            bool createdLoginAudit = await _loginAuditRepository.CreateLoginAuditAsync(loginAudit.ToEntity());

            if (createdLoginAudit)
                return new LoginAuditResponseDto(true, "Se registró el inicio de sesión");
    
            return new LoginAuditResponseDto(false, "No se pudo registrar el inicio de sesión");
        }


        public async Task<bool> IsTokenValidAsync(string tokenId)
        {
            return await _loginAuditRepository.IsTokenValidAsync(tokenId);
        }


        public async Task<bool> RevokeAllTokensAsync(string idCCNit)
        {
            bool hasActiveTokens = await _loginAuditRepository.HasActiveTokensAsync(idCCNit);
            if (!hasActiveTokens)
                return true;

            bool revokedTokens = await _loginAuditRepository.RevokeAllTokensAsync(idCCNit);
            return revokedTokens;
        }


        public async Task<bool> RevokeTokenAsync(string tokenId)
        {
            bool revokedToken = await _loginAuditRepository.RevokeTokenAsync(tokenId);
            return revokedToken;
        }




        private string ValidateLoginAuditDto(LoginAuditDto loginAudit)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(loginAudit);
            
            if (!Validator.TryValidateObject(loginAudit, context, validationResults, true))
            {
                var errors = string.Join("; ", validationResults.Select(e => e.ErrorMessage));
                return errors;
            }

            return "";
        }


        private async Task<bool> DeleteRefreshTokensAsync(string idCCNit)
        {
            bool hasActiveTokens = await _loginAuditRepository.HasActiveRefreshTokensAsync(idCCNit);
            if (!hasActiveTokens)
                return true;
            bool deletedRefreshTokens = await _loginAuditRepository.DeleteRefreshTokensAsync(idCCNit);
            return deletedRefreshTokens;
        }
    }
}