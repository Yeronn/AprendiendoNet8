using System.IdentityModel.Tokens.Jwt;
using Application.DTOs.Authentication;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        string? GetClientIpAddress();
        string GetDeviceInfo();
        Task<LoginResponse> Login(LoginDto login);
        Task<(bool Success, string Message)> GenerateJWTToken(UserJwtTokenDto user, bool isAccessToken);
        Task<bool> ValidateToken(string token);
        Task<RefreshTokenResponseDto> RefreshTokens(string idCCNit);
    }
}
