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
            
            bool revokedPreviousTokens = await RevokeAllTokensAsync(loginAudit.IdCCNit);
            if (!revokedPreviousTokens)
                return new LoginAuditResponseDto(false, "No se pudo revocar los anteriores tokens de acceso");

            bool createdLoginAudit = await _loginAuditRepository.CreateLoginAuditAsync(loginAudit.ToEntity());

            if (createdLoginAudit)
                return new LoginAuditResponseDto(true, "Se registró el inicio de sesión");
    
            return new LoginAuditResponseDto(false, "No se pudo registrar el inicio de sesión");
        }

        
        public async Task<bool> IsTokenValidAsync(string tokenId)
        {
            return await _loginAuditRepository.IsTokenValidAsync(tokenId);
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


        private async Task<bool> RevokeAllTokensAsync(int idCCNit)
        {
            bool revokedToken = await _loginAuditRepository.RevokeAllTokensAsync(idCCNit);
            if (!revokedToken)
            {
                bool hasActiveTokens = await _loginAuditRepository.HasActiveTokensAsync(idCCNit);
                return !hasActiveTokens;
            }
            return revokedToken;
        }
        
    }
}