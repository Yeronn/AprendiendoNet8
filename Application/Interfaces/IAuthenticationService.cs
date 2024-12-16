using System.IdentityModel.Tokens.Jwt;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        string? GetClientIpAddress();
        string GetDeviceInfo();
        Task<LoginResponse> Login(LoginDto login);
        Task<string> GenerateJWTToken(UserJwtTokenDto user, bool isAccessToken);
        Task<bool> ValidateToken(string token);
        UserJwtTokenDto ExtractUserFromToken(JwtSecurityToken jwtToken);
    }
}
