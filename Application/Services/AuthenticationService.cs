using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
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
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasher;

        public AuthenticationService(
                IConfiguration configuration, 
                IPasswordHasherService passwordHasher, 
                IUserService userService, 
                IRoleService roleService,
                IPermissionService permissionService
            )
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _permissionService = permissionService;
        }


        public async Task<LoginResponse> Login(LoginDto login)
        {
            bool userExists = await _userService.VerifyIdCCNitExistsAsync(login.IdCCNit);
            if (!userExists)
                return new LoginResponse(false, "El usuario no existe");

            var hashedPassword = await _userService.GetPasswordByIdCCNitAsync(login.IdCCNit);
            bool checkPassword = _passwordHasher.VerifyPassword(login.Password, hashedPassword!);

            if (checkPassword)
            {
                var newJti = Guid.NewGuid().ToString();
                bool updatedJti = await _userService.UpdateLastJtiAsync(login.IdCCNit, newJti);
                if (!updatedJti)
                    return new LoginResponse(false, "Ocurrió un error al actualizar la sesión");
                var user = await _userService.GetUserByIdCCNitAsync(login.IdCCNit);
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
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission.Name)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
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
            {
                return false; // Token inválido
            }

            // Obtener el usuario por el jti
            var user = await _userService.GetUserByLastJtiAsync(jti);
            if (user == null || user.LastJti != jti)
            {
                return false; // Token no autorizado o ha expirado
            }

            // Token es válido
            return true;
        }


    }
}
