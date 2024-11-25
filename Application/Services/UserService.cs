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
        

        public async Task<UserDto?> GetUserByLastJtiAsync(string lastJti)
        {
            var userEntity = await _userRepository.GetUserByLastJtiAsync(lastJti);
            return userEntity?.ToUserDto();  
        }

        //TODO: Borrar metodo, el metodo exists hace esto
        public async Task<bool> IsIdCardNitUniqueAsync(int idCardNit)
        {
            return !await _userRepository.IdCardNitExistsAsync(idCardNit);  // Devuelve true si no existe
        }


        public async Task<string?> GetPasswordByIdCardNitAsync(int idCardNit)
        {
            var password = await _userRepository.GetPasswordByIdCardNitAsync(idCardNit);
            if (password == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            return password;
        }


        public async Task<bool> UpdateLastJtiAsync(int idCardNit, string lastJti)
        {
            // Verifica si el usuario existe con el idCardNit (opcional)
            var user = await _userRepository.GetUserByIdCardNitAsync(idCardNit);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Llama al repositorio para actualizar el LastJti
            return await _userRepository.UpdateLastJtiAsync(idCardNit, lastJti);
        }


        public async Task<bool> VerifyIdCardNitExistsAsync(int idCardNit)
        {
            var exists = await _userRepository.IdCardNitExistsAsync(idCardNit);

            if (!exists)
                return exists;

            return exists;
        }


        public async Task<UserJwtTokenDto> GetUserJwtTokenByIdCardNitAsync(int idCardNit)
        {
            var user = await GetUserByIdCardNitAsync(idCardNit);
            
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            var userJwtTokenDto = user.ToUserJwtTokenDto();
            return userJwtTokenDto;
        }




    }

}
