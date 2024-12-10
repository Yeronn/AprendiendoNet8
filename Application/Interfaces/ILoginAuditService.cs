using Application.DTOs.LoginAudit;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoginAuditService
    {
        Task CreateLoginAuditAsync(LoginAuditDto loginAudit);
        Task RevokeTokenAsync(string tokenId);
        Task<bool> IsTokenValidAsync(string tokenId);
    }
}