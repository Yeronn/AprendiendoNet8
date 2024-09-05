using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasher;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<RegistrationResponse> RegisterUser(RegisterUserDto newUser)
        {
            var userExist = await _userRepository.GetByEmail(newUser.Email);

            if (userExist != null)
                return new RegistrationResponse("El email ya se encuentra registrado");

            var hashedPassword = _passwordHasher.HashPassword(newUser.Password);
            var userWithHashedPassword = new RegisterUserDto
            {
                Fullname = newUser.Fullname,
                IdentityCard = newUser.IdentityCard,
                Email = newUser.Email,
                Username = newUser.Username,
                Password = hashedPassword
            };

            var userEntity = userWithHashedPassword.ToUserEntity();
            var createdUser = await _userRepository.Create(userEntity);

            if (createdUser == null)
                return new RegistrationResponse("Hubo un error en el servidor al crear al usuario", internalServerError: true);
            return new RegistrationResponse("El usuario se creó correctamente", createdUser.Id);
        }

        public async Task<LoginResponse> Login(string email, string password)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null)
                return new LoginResponse(false, "La cuenta no existe");

            bool checkPassword = _passwordHasher.VerifyPassword(password, user.Password);

            if (checkPassword)
                return new LoginResponse(checkPassword, "Inicio de sesión exitoso", GenerateJWTToken(user));
            else
                return new LoginResponse(checkPassword, "Credenciales Inválidas");
        }

        public string GenerateJWTToken(UserEntity user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email.ToString())
                //TODO: Falta agregar el rol
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
                );
            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }

    }
}
