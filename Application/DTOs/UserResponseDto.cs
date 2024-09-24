namespace Application.DTOs
{
    public record UserResponseDto(bool Success, string Message, UserWithoutRolesDto? User = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);
}