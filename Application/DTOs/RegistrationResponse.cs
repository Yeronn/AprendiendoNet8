using Application.DTOs.User;

namespace Application.DTOs
{
    public record RegistrationResponse(bool Success, string Message = null!, UserDto? UserDto = null);
}
