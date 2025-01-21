using Application.Authorization;
using Application.DTOs.Authentication;
using Application.DTOs.LoginAudit;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly ILoginAuditService _loginAuditService;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ICompanyService _companyService;

        public AuthenticationService(
                IConfiguration configuration, 
                IUserService userService, 
                IPermissionService permissionService,
                ILoginAuditService loginAuditService,
                IPasswordHasherService passwordHasher,
                ICompanyService companyService
            )
        {
            _configuration = configuration;
            _userService = userService;
            _permissionService = permissionService;
            _loginAuditService = loginAuditService;
            _passwordHasher = passwordHasher;
            _companyService = companyService;
        }


        public async Task<LoginResponse> Login(LoginDto login, string ipAddress, string deviceInfo)
        {
            var user = await _userService.GetUserByCCNumberAndNitAsync(login.CcNumber, login.Nit);
            if (user == null)
                return new LoginResponse(false, "Credenciales Inválidas", IsBadRequest: true);

            var hashedPassword = await _userService.GetPasswordByIdCCNitAsync(login.IdCCNit);
            bool checkPassword = _passwordHasher.VerifyPassword(login.Password, hashedPassword!);

            if (!checkPassword)
                return new LoginResponse(checkPassword, "Credenciales Inválidas", IsBadRequest: true);

            var userToken = user!.ToUserJwtTokenDto();
            userToken.IPAddress = ipAddress;
            userToken.DeviceInfo = deviceInfo;

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

            var company = await _companyService.GetCompanyByRoleIdAsync(user.RoleId);
            int companyId = company!.Id;

            var claims = new List<Claim>
            {
                new Claim(UserClaims.CompanyId.ToString(), $"{companyId}"),
                new Claim(UserClaims.IdCCNit.ToString(), user.IdCCNit.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(UserClaims.TokenType.ToString(), isAccessToken ? TokenType.Access.ToString() : TokenType.Refresh.ToString())
            };

            
            DateTime tokenExpiration;

            if (isAccessToken)
            {
                claims.Add(new Claim(UserClaims.Fullname.ToString(), $"{user.FirstName} {user.LastName}"));

                var permissions = await _permissionService.GetPermissionsByRoleIdAsync(user.RoleId);
                claims.AddRange(permissions.Select(permission => new Claim(UserClaims.Permissions.ToString(), permission.Name)));

                tokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!));
            }
            else
            {
                tokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:RefreshTokenExpirationMinutes"]!));
            }

            var newLogin = new LoginAuditDto
            {
                TokenId = jti,
                IdCCNit = user.IdCCNit,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = tokenExpiration,
                IPAddress = user.IPAddress,
                DeviceInfo = user.DeviceInfo,
                TokenStatusId = (int)TokenStatus.Valid,
                TokenTypeId =  isAccessToken ? (int)TokenType.Access : (int)TokenType.Refresh,
                CompanyId = companyId,
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


        public async Task<RefreshTokenResponseDto> RefreshTokens(string idCCNit, string ipAddress, string deviceInfo)
        {
            var user = await _userService.GetUserByIdCCNitAsync(idCCNit);
            if (user == null)
                return new RefreshTokenResponseDto(false, "No se pudo refrescar los tokens, el usuario no existe", IsBadRequest: true);

            var userToken = user.ToUserJwtTokenDto();
            userToken.IPAddress = ipAddress;
            userToken.DeviceInfo = deviceInfo;

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
