using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

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

        public async Task<UserEntity?> Login(string email, string password)
        {
            var user = await _userRepository.Login(email, password);
            return user;
        }

        public async Task<UserEntity?> Create(RegisterUserDto newUser)
        {
            var userEntity = newUser.ToUserEntity();

            return await _userRepository.Create(userEntity);
        }
    }
}
