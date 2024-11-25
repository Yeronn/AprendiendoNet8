using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasher;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasherService passwordHasher, IUserService userService, IRoleService roleService)
        {
            _userRepository = userRepository;
            _userService = userService;
            _roleService = roleService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }


        public async Task<RegistrationResponse> RegisterUser(RegisterUserDto newUser)
        {
            bool isUnique = await _userService.IsIdCardNitUniqueAsync(newUser.IdCardNit);
            if (!isUnique)
                throw new Exception("El IdCardNit ya está registrado.");

            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(newUser.RoleId);
            if (!roleExists.Success)
                throw new Exception("El rol del usuario no existe");
            
            //TODO: Implementar el PasswordSalt
            

            var hashedPassword = _passwordHasher.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var userEntity = newUser.ToUserEntity();
            var createdUserId  = await _userRepository.CreateUserAsync(userEntity);

            if (createdUserId.HasValue)
            {
                var createdUser = await _userService.GetUserByIdAsync(createdUserId.Value);
                return new RegistrationResponse(true, "El usuario se creó correctamente", createdUser);
            }
                return new RegistrationResponse(false, "Hubo un error en el servidor al crear al usuario");
        }


        public async Task<LoginResponse> Login(LoginDto login)
        {
            bool userExists = await _userService.VerifyIdCardNitExistsAsync(login.IdCardNit);
            if (!userExists)
                return new LoginResponse(false, "El usuario no existe");

            var hashedPassword = await _userService.GetPasswordByIdCardNitAsync(login.IdCardNit);
            bool checkPassword = _passwordHasher.VerifyPassword(login.Password, hashedPassword!);

            if (checkPassword)
            {
                var newJti = Guid.NewGuid().ToString();
                bool updatedJti = await _userService.UpdateLastJtiAsync(login.IdCardNit, newJti);
                if (!updatedJti)
                    return new LoginResponse(false, "Ocurrió un error al actualizar la sesión");
                var userJwtTokenDto = await _userService.GetUserJwtTokenByIdCardNitAsync(login.IdCardNit);
                return new LoginResponse(checkPassword, "Inicio de sesión exitoso", GenerateJWTToken(userJwtTokenDto, newJti));
            }
            else
                return new LoginResponse(checkPassword, "Credenciales Inválidas", IsBadRequest: true);
        }


        public string GenerateJWTToken(UserJwtTokenDto user, string jti)
        {
            // var roles = _roleRepository.GetAllRolesByUserIdAsync(user.Id);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdCardNit.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                // new Claim(ClaimTypes.Role, user.RoleId!),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

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
