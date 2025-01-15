using Application.DTOs.Authentication;
using Application.DTOs.LoginAudit;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly ILoginAuditService _loginAuditService;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(
                IConfiguration configuration, 
                IPasswordHasherService passwordHasher, 
                IUserService userService, 
                IPermissionService permissionService,
                ILoginAuditService loginAuditService,
                IHttpContextAccessor httpContextAccessor
            )
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _permissionService = permissionService;
            _loginAuditService = loginAuditService;
            _httpContextAccessor = httpContextAccessor;
        }


        public string? GetClientIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            var ip = context?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            return !string.IsNullOrEmpty(ip) ? ip : context?.Connection.RemoteIpAddress?.ToString();
        }


        public string GetDeviceInfo()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        }


        public async Task<LoginResponse> Login(LoginDto login)
        {
            var user = await _userService.GetUserByIdCCNitAsync(login.IdCCNit);
            if (user == null)
                return new LoginResponse(false, "Credenciales Inválidas", IsBadRequest: true);

            var hashedPassword = await _userService.GetPasswordByIdCCNitAsync(login.IdCCNit);
            bool checkPassword = _passwordHasher.VerifyPassword(login.Password, hashedPassword!);

            if (!checkPassword)
                return new LoginResponse(checkPassword, "Credenciales Inválidas", IsBadRequest: true);

            var userToken = user!.ToUserJwtTokenDto();
            var (accessTokenSuccess, accessTokenValue) = await GenerateJWTToken(userToken);
            if (!accessTokenSuccess)
                return new LoginResponse(false, accessTokenValue);

            var (refreshTokenSuccess, refreshTokenValue) = await GenerateJWTToken(userToken, false);
            if (!refreshTokenSuccess)
            {
                await _loginAuditService.RevokeAllTokensAsync(userToken.IdCCNit);
                return new LoginResponse(false, refreshTokenValue);
            }

            return new LoginResponse(checkPassword, "Inicio de sesión exitoso", accessTokenValue, refreshTokenValue);
        }


        public async Task<(bool Success, string Message)> GenerateJWTToken(UserJwtTokenDto user, bool isAccessToken = true)
        {
            var jti = Guid.NewGuid().ToString();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("IdCCNIT", user.IdCCNit.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim("TokenType", isAccessToken ? TokenType.Access.ToString() : TokenType.Refresh.ToString())
            };

            DateTime tokenExpiration;

            if (isAccessToken)
            {
                var permissions = await _permissionService.GetPermissionsByRoleIdAsync(user.RoleId);

                claims.Add(new Claim("Fullname", $"{user.FirstName} {user.LastName}"));
                claims.AddRange(permissions.Select(permission => new Claim("Permissions", permission.Name)));

                tokenExpiration = DateTime.UtcNow.AddMinutes(1);
            }
            else
            {
                tokenExpiration = DateTime.UtcNow.AddMinutes(2);
            }

            var newLogin = new LoginAuditDto
            {
                TokenId = jti,
                IdCCNit = user.IdCCNit,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = tokenExpiration,
                IPAddress = GetClientIpAddress(),
                DeviceInfo = GetDeviceInfo(),
                TokenStatusId = (int)TokenStatus.Valid,
                TokenTypeId =  isAccessToken ? (int)TokenType.Access : (int)TokenType.Refresh,
            };
            var createdLoginAudit = await _loginAuditService.CreateLoginAuditAsync(newLogin);
            if (!createdLoginAudit.Success)
                return (false, createdLoginAudit.Message);

            var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: tokenExpiration,
                    signingCredentials: credentials
                    );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return (true, tokenValue);
        }


        public async Task<bool> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (jti == null)
                return false;

            var loginRecord = await _loginAuditService.GetLoginAuditByTokenIdAsync(jti);
            if (loginRecord == null || loginRecord.TokenStatusId == (int)TokenStatus.Revoked)
                return false;

            bool expiredToken = loginRecord!.ExpiresAt < DateTime.UtcNow;
            if (expiredToken)
            {
                await _loginAuditService.RevokeTokenAsync(jti);
                return false;
            }

            return true;
        }


        public async Task<RefreshTokenResponseDto> RefreshTokens(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtRefreshToken = tokenHandler.ReadJwtToken(refreshToken);

            string? idCCNit = jwtRefreshToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (idCCNit == null)
                return new RefreshTokenResponseDto(false, "El refresh token no tiene el idCCNit", IsBadRequest: true);

            var user = await _userService.GetUserByIdCCNitAsync(idCCNit);
            if (user == null)
                return new RefreshTokenResponseDto(false, "No se pudo refrescar los tokens, el usuario no existe", IsBadRequest: true);

            var userToken = user!.ToUserJwtTokenDto();

            var (accessTokenSuccess, newAccessToken) = await GenerateJWTToken(userToken);
            if (!accessTokenSuccess)
                return new RefreshTokenResponseDto(false, newAccessToken, IsBadRequest: true);

            var (refreshTokenSuccess, newRefreshToken) = await GenerateJWTToken(userToken, false);
            if (!refreshTokenSuccess)
            {
                await _loginAuditService.RevokeAllTokensAsync(userToken.IdCCNit);
                return new RefreshTokenResponseDto(false, newRefreshToken, IsBadRequest: true);
            }

            return new RefreshTokenResponseDto(true, "Tokens generados", newAccessToken, newRefreshToken);
        }



    }
}
