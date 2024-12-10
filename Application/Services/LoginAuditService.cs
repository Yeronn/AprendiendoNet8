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

        
        public async Task CreateLoginAuditAsync(LoginAuditDto loginAudit)
        {
            await _loginAuditRepository.CreateLoginAuditAsync(loginAudit.ToEntity());
            //TODO: Al crear un nuevo token, el antiguo se revoca y no sirve
        }

        
        public async Task RevokeTokenAsync(string tokenId)
        {
            await _loginAuditRepository.RevokeTokenAsync(tokenId);
        }

        
        public async Task<bool> IsTokenValidAsync(string tokenId)
        {
            return await _loginAuditRepository.IsTokenValidAsync(tokenId);
        }
    }
}