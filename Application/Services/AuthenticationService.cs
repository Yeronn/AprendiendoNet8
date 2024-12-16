using Application.DTOs.LoginAudit;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
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
            string token = await GenerateJWTToken(userToken);
            string refreshToken = await GenerateJWTToken(userToken, false);

            if (string.IsNullOrWhiteSpace(token))
                return new LoginResponse(false, "Error al registrar el inicio de sesión");

            return new LoginResponse(checkPassword, "Inicio de sesión exitoso", token, refreshToken);
        }


        public async Task<string> GenerateJWTToken(UserJwtTokenDto user, bool isAccessToken = true)
        {
            var jti = Guid.NewGuid().ToString();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            DateTime tokenExpiration;

            if (isAccessToken)
            {
                var permissions = await _permissionService.GetPermissionsByRoleIdAsync(user.RoleId);
                claims =
                [
                    new Claim(ClaimTypes.NameIdentifier, user.IdCCNit.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim(JwtRegisteredClaimNames.Jti, jti),
                    .. permissions.Select(permission => new Claim("Permissions", permission.Name)),
                ];

                tokenExpiration = DateTime.UtcNow.AddMinutes(60);

                var newLogin = new LoginAuditDto
                {
                    TokenId = jti,
                    IdCCNit = user.IdCCNit,
                    IssuedAt = DateTime.UtcNow,
                    ExpiresAt = tokenExpiration,
                    IPAddress = GetClientIpAddress(),  
                    DeviceInfo = GetDeviceInfo(),
                };

                var createdLoginAudit = await _loginAuditService.CreateLoginAuditAsync(newLogin);
                if(!createdLoginAudit.Success)
                    return "";
            }
            else
            {
                tokenExpiration = DateTime.UtcNow.AddMinutes(120);
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti));
            }
            
            var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: tokenExpiration,
                    signingCredentials: credentials
                    );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }


        public async Task<bool> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jti == null)
                return false;

            var loginRecord = await _loginAuditService.GetLoginAuditByTokenIdAsync(jti);
            if (loginRecord == null || !loginRecord.Status)
                return false;

            return true;
        }


        public UserJwtTokenDto ExtractUserFromToken(JwtSecurityToken jwtToken)
        {
            var idCCNit = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            var lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "LastName")!.Value;
            var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "RoleId")!.Value;

            return new UserJwtTokenDto
            {
                IdCCNit = idCCNit,
                FirstName = firstName,
                LastName = lastName,
                RoleId = Convert.ToInt32(roleId)
            };
        }


        public async Task<RefreshTokenResponseDto> RefreshToken(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(refreshToken);

            // Verificar si el refresh token es válido (no expirado)
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return new RefreshTokenResponseDto(false, "El RefreshToken ha expirado", IsBadRequest: true);
            }

            // Extraer los datos del usuario del refresh token
            var user = ExtractUserFromToken(jwtToken); 

            // Generar nuevos tokens (Access Token y Refresh Token)
            var newAccessToken = await GenerateJWTToken(user, true); // Generar Access Token
            var newRefreshToken = await GenerateJWTToken(user, false); // Generar Refresh Token

            // Retornar los nuevos tokens
            return new RefreshTokenResponseDto(true, "");//TODO: Hacer esto
        }


    }
}
