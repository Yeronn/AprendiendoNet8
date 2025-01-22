using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILoginAuditRepository
    {
        Task<LoginAuditEntity?> GetLoginAuditByTokenIdAsync(string tokenId);
        Task<bool> CreateLoginAuditAsync(LoginAuditEntity loginAudit);
        Task<bool> RevokeAllTokensAsync(int userId);
        Task<bool> RevokeTokenAsync(string tokenId);
        Task<bool> IsTokenValidAsync(string tokenId);
        Task<bool> HasActiveTokensAsync(int userId);
        Task<bool> DeleteRefreshTokensAsync(int userId);
        Task<bool> HasActiveRefreshTokensAsync(int userId);
    }
}