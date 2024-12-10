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
            var validationResults = ValidateLoginAuditDto(loginAudit);
            if (!string.IsNullOrWhiteSpace(validationResults))
            {
                return new LoginAuditResponseDto(false, $"Errores de validación: {validationResults}");
            }

            bool createdLoginAudit = await _loginAuditRepository.CreateLoginAuditAsync(loginAudit.ToEntity());

            if (createdLoginAudit)
                return new LoginAuditResponseDto(true, "Se registró el inicio de sesión");
    
            return new LoginAuditResponseDto(false, "No se pudo registrar el inicio de sesión");
            //TODO: Al crear un nuevo token, el antiguo se revoca
        }

        
        public async Task<LoginAuditResponseDto> RevokeTokenAsync(string tokenId)
        {
            bool revokedToken = await _loginAuditRepository.RevokeTokenAsync(tokenId);
            if (revokedToken)
                return new LoginAuditResponseDto(true, "El token anterior ahora es inválido");

            return new LoginAuditResponseDto(false, "No se pudo invalidar el anterior token");
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

        
    }
}