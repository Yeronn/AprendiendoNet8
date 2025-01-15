namespace Application.DTOs.LoginAudit
{
    public record LoginAuditResponseDto(bool Success, string Message, LoginAuditDto? LoginAuditDto = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false, bool IsInternalServerError = false);
}