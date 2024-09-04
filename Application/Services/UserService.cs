using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using BCrypt.Net;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await _userRepository.GetAll();
            var usersDto = users.Select(u => u.ToUserDto());
            return usersDto;
        }

        public async Task<UserEntity?> GetById(int id)
        {
            var user = await _userRepository.GetById(id);
            return user;
        }

        public async Task<LoginResponse?> Login(string email, string password)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null)
                return null; //La cuenta no existe

            bool checkPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (checkPassword)
                return new LoginResponse(checkPassword, "Login Successfully", GenerateJWTToken(user));
            else
                return new LoginResponse(checkPassword, "Invalid credentials");
        }

        public string GenerateJWTToken(UserEntity user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email.ToString())
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

        }

        public async Task<UserEntity?> Create(RegisterUserDto newUser)
        {
            var userEntity = newUser.ToUserEntity();

            return await _userRepository.Create(userEntity);
        }
    }
}
