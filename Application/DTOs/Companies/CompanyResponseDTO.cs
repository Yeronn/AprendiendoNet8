namespace Application.DTOs.Companies
{
    public record CompanyResponseDTO(bool Success, string Message, CompanyDto? Company = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);
}