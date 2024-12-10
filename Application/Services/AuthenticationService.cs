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
                return new LoginResponse(false, "El usuario no existe");

            var hashedPassword = await _userService.GetPasswordByIdCCNitAsync(login.IdCCNit);
            bool checkPassword = _passwordHasher.VerifyPassword(login.Password, hashedPassword!);

            if (checkPassword)
            {
                //TODO: Poner en la funcion que genera el token, validar que el token se haya creado correctamente antes de ingresar al sistema el nuevo jti
                var newJti = Guid.NewGuid().ToString();
                bool updatedJti = await _userService.UpdateLastJtiAsync(login.IdCCNit, newJti);
                if (!updatedJti)
                    return new LoginResponse(false, "Ocurrió un error al actualizar la sesión");

                var userToken = user!.ToUserJwtTokenDto();
                var token = await GenerateJWTToken(userToken, newJti);
                return new LoginResponse(checkPassword, "Inicio de sesión exitoso", token);
            }
            else
                return new LoginResponse(checkPassword, "Credenciales Inválidas", IsBadRequest: true);
        }


        public async Task<string> GenerateJWTToken(UserJwtTokenDto user, string jti)
        {
            var permissions = await _permissionService.GetPermissionsByRoleIdAsync(user.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdCCNit.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
            };

            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission.Name)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            DateTime tokenExpiration = DateTime.UtcNow.AddMinutes(60);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: tokenExpiration,
                signingCredentials: credentials
                );

            // TODO: Guarda en la base de datos el inicio de sesion
            var newToken = new LoginAuditDto
            {
                TokenId = jti,
                IdCCNit = user.IdCCNit,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = tokenExpiration,
                IPAddress = GetClientIpAddress(),  
                DeviceInfo = GetDeviceInfo(),
            };

            await _loginAuditService.CreateLoginAuditAsync(newToken);
            
            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }


        public async Task<bool> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jti == null)
                return false; // Token inválido

            // Obtener el usuario por el jti
            var user = await _userService.GetUserByLastJtiAsync(jti);

            // TODO: Validar que el estado del token sea valid (true) y no revoked (false)
            // var isRevoked = await _auditRepository.IsTokenRevokedAsync(jti);
            if (user == null || user.LastJti != jti)
                return false; // Token no autorizado o ha expirado

            // Token es válido
            return true;
        }


    }
}
