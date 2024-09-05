using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using BCrypt.Net;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
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

    }
}
