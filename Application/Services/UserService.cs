using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
        }





        public async Task<IEnumerable<UsersDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var usersDto = users.Select(u => u.ToUsersDto());
            return usersDto;
        }


        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;
            return user.ToUserDto();
        }


        //TODO: Hacer endpoints para
        //TODO: Obtener un usuario con sus roles
        //TODO: Obtener todos los usuarios con sus roles
        //TODO: Asignar roles al usuario
        //TODO: Remover roles del usuario
        //TODO: Remover los roles del usuario para eliminarlo
        //TODO: Obtener todos los usuarios de un rol

    }
}
