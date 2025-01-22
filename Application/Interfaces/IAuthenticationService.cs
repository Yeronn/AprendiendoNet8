using System.IdentityModel.Tokens.Jwt;
using Application.DTOs.Authentication;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginDto login, string ipAdress, string deviceInfo);
        Task<(bool Success, string Message)> GenerateJWTToken(UserJwtTokenDto user, bool isAccessToken);
        Task<bool> ValidateToken(string token);
        Task<RefreshTokenResponseDto> RefreshTokens(int userId, string ipAddress, string deviceInfo);
    }
}
