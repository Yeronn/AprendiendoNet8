using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILoginAuditRepository
    {
        Task<LoginAuditEntity?> GetLoginAuditByTokenIdAsync(string tokenId);
        Task<bool> CreateLoginAuditAsync(LoginAuditEntity loginAudit);
        Task<bool> RevokeAllTokensAsync(string idCCNit);
        Task<bool> IsTokenValidAsync(string tokenId);
        Task<bool> HasActiveTokensAsync(string idCCNit);
    }
}