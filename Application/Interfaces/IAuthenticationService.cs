using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        string? GetClientIpAddress();
        string GetDeviceInfo();
        Task<LoginResponse> Login(LoginDto login);
        Task<string> GenerateJWTToken(UserJwtTokenDto user);
        Task<bool> ValidateToken(string token);
    }
}
