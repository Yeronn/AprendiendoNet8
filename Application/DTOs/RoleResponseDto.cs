namespace Application.DTOs
{
    public record RoleResponseDto(bool Success, string Message, RoleWithoutPermissionsDto? Role = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);

}
