using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
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

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(u => u.ToUserDto());
        }

        public async Task<UserDto?> GetUserByIdCardNitAsync(int idCardNit)
        {
            var user = await _userRepository.GetUserByIdCardNitAsync(idCardNit);
            return user?.ToUserDto();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user?.ToUserDto();
        }

        public async Task<UserDto?> CreateUserAsync(RegisterUserDto createUserDto)
        {
            var userEntity = createUserDto.ToUserEntity();
            var createdUserId = await _userRepository.CreateUserAsync(userEntity);
            
            if (createdUserId.HasValue)
            {
                
                var createdUser = await _userRepository.GetUserByIdAsync(createdUserId.Value);
                return createdUser?.ToUserDto();
            }
            return null;
        }


        public async Task<UserDto?> UpdateUserAsync(int idCardNit, UpdateUserDto updateUserDto)
        {
            var userEntity = updateUserDto.ToUserEntity();
            userEntity.IdCardNit = idCardNit; 

            var isUpdated = await _userRepository.UpdateUserAsync(userEntity);
            if (isUpdated)
            {
                var updatedUser = await _userRepository.GetUserByIdCardNitAsync(idCardNit);
                return updatedUser?.ToUserDto();
            }
            return null;
        }

        public async Task<bool> DeleteUserAsync(int idCardNit)
        {
            return await _userRepository.DeleteUserAsync(idCardNit);
        }
        
    }

}
