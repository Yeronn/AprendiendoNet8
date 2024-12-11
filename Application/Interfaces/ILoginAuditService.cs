using Application.DTOs.LoginAudit;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoginAuditService
    {
        Task<LoginAuditDto?> GetLoginAuditByTokenIdAsync(string tokenId);
        Task<LoginAuditResponseDto> CreateLoginAuditAsync(LoginAuditDto loginAudit);
        Task<bool> IsTokenValidAsync(string tokenId);
    }
}