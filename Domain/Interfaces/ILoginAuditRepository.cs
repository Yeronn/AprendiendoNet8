using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILoginAuditRepository
    {
        Task CreateLoginAuditAsync(LoginAuditEntity loginAudit);
        Task RevokeTokenAsync(string tokenId);
        Task<bool> IsTokenValidAsync(string tokenId);
    }
}