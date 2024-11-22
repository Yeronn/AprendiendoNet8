namespace Application.DTOs.Company
{
    public record CompanyResponseDto(bool Success, string Message, CompanyDto? Company = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);
}