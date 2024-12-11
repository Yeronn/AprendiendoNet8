using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILoginAuditRepository
    {
        Task<LoginAuditEntity?> GetLoginAuditByTokenIdAsync(string tokenId);
        Task<bool> CreateLoginAuditAsync(LoginAuditEntity loginAudit);
        Task<bool> RevokeAllTokensAsync(int idCCNit);
        Task<bool> IsTokenValidAsync(string tokenId);
        Task<bool> HasActiveTokensAsync(int idCCNit);
    }
}