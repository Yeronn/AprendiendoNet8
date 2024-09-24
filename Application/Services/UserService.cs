using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IRoleService _roleService;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasherService passwordHasher, IRoleService roleService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _roleService = roleService;
        }


        public async Task<IEnumerable<UserWithoutRolesDto>?> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            if (!users.Any())
                return null;
            var usersDto = users.Select(u => u.ToUserWithoutRolesDto());
            return usersDto;
        }


        public async Task<UserWithoutRolesDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;
            return user.ToUserWithoutRolesDto();
        }


        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto) 
        {
            var userExists = await ValidateUserExistsByIdAsync(id);
            if (!userExists.Success)
                return userExists;
            
            var currentUser = await _userRepository.GetUserByIdAsync(id);
            var updateUserEntity = updateUserDto.ToUserEntity();
            updateUserEntity.Id = id;

            if (!string.IsNullOrEmpty(updateUserEntity.Fullname) && updateUserEntity.Fullname != currentUser!.Fullname)
            {
                var availableName = await CheckUserFullnameAvailabilityAsync(updateUserEntity.Fullname);
                if (!availableName.Success)
                    return availableName;

                await _userRepository.UpdateFullnameAsync(id, updateUserEntity.Fullname);
            }
            else
                updateUserEntity.Fullname = currentUser!.Fullname;


            if (!string.IsNullOrEmpty(updateUserEntity.Username) && updateUserEntity.Username != currentUser!.Username)
            {
                var availableName = await CheckUsernameAvailabilityAsync(updateUserEntity.Username);
                if (!availableName.Success)
                    return availableName;

                await _userRepository.UpdateUsernameAsync(id, updateUserEntity.Username);
            }
            else
                updateUserEntity.Username = currentUser!.Username;

            if(!string.IsNullOrEmpty(updateUserEntity.Password))
            {
                bool samePassword = _passwordHasher.VerifyPassword(updateUserEntity.Password!, currentUser.Password!);
                if (!samePassword)
                {
                    string hashedPassword = _passwordHasher.HashPassword(updateUserEntity.Password);
                    await _userRepository.UpdatePasswordAsync(id, hashedPassword);
                }
            }

            return new UserResponseDto(true, "Usuario actualizado correctamente.", updateUserEntity.ToUserWithoutRolesDto());
        }


        public async Task<UserDto?> GetUserWithRolesByUserIdAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return null;
            var userDto = user.ToUserDto();
            var roles = await _roleService.GetAllRolesByUserIdAsync(userId);
            userDto.Roles = roles.ToList();
            return userDto;
        }


        public async Task<IEnumerable<UserDto>?> GetAllUsersWithRolesAsync()
        {
            var usersWithRoles = await _userRepository.GetAllUsersWithRolesAsync();
            if (!usersWithRoles.Any())
                return null;

            return usersWithRoles.Select(user => user.ToUserDto());
        }


        
        //TODO: Hacer endpoints para
        //TODO: Asignar roles al usuario
        //TODO: Remover roles del usuario
        //TODO: Remover los roles del usuario para eliminarlo
        //TODO: Obtener todos los usuarios de un rol




        private async Task<UserResponseDto> ValidateUserExistsByIdAsync(int id)
        {
            bool userExists = await _userRepository.ExistUserByIdAsync(id);
            if (!userExists)
                return new UserResponseDto(false, "El usuario no existe.", IsNotFound: true);

            return new UserResponseDto(true, "El usuario existe.");
        }


        private async Task<UserResponseDto> CheckUserFullnameAvailabilityAsync(string userFullname)
        {
            bool userFullnameExists = await _userRepository.ExistUserByFullNameAsync(userFullname);
            if (userFullnameExists)
                return new UserResponseDto(false, "El nombre de usuario ya existe en el sistema", IsConflict: true);

            return new UserResponseDto(true, "Nombre de usuario válido.");
        }


        private async Task<UserResponseDto> CheckUsernameAvailabilityAsync(string userName)
        {
            bool userUsernameExists = await _userRepository.ExistUserByUsernameAsync(userName);
            if (userUsernameExists)
                return new UserResponseDto(false, "El nombre de usuario ya existe en el sistema", IsConflict: true);

            return new UserResponseDto(true, "Nombre de usuario válido.");
        }
    }
}
