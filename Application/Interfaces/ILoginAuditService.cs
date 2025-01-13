using Application.DTOs.LoginAudit;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoginAuditService
    {
        Task<LoginAuditDto?> GetLoginAuditByTokenIdAsync(string tokenId);
        Task<LoginAuditResponseDto> CreateLoginAuditAsync(LoginAuditDto loginAudit);
        Task<bool> IsTokenValidAsync(string tokenId);
        Task<bool> RevokeAllTokensAsync(string idCCNit);
        Task<bool> RevokeTokenAsync(string tokenId);
    }
}