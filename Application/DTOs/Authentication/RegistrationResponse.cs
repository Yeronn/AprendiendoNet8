using Application.DTOs.User;

namespace Application.DTOs.Authentication
{
    public record RegistrationResponse(bool Success, string Message = null!, UserDto? UserDto = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);
}
