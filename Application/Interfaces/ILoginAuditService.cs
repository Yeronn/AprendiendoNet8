using Application.DTOs.LoginAudit;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoginAuditService
    {
        Task<LoginAuditDto?> GetLoginAuditByTokenIdAsync(string tokenId);
        Task<LoginAuditResponseDto> CreateLoginAuditAsync(LoginAuditDto loginAudit);
        Task<LoginAuditResponseDto> RevokeTokenAsync(string tokenId);
        Task<bool> IsTokenValidAsync(string tokenId);
    }
}