using Application.DTOs.User;

namespace Application.DTOs
{
    public record UserResponseDto(bool Success, string Message, UserDto? User = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);
}