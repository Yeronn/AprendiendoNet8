using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly IPasswordHasherService _passwordHasher;

        public UserService(IUserRepository userRepository, IRoleService roleService, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _roleService = roleService;
            _passwordHasher = passwordHasher;
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


        public async Task<UserResponseDto> UpdateUserAsync(int idCardNit, UpdateUserDto updateUserDto)
        {
            bool idCardNitExists = await VerifyIdCardNitExistsAsync(idCardNit);
            if (!idCardNitExists)
                return new UserResponseDto(false, "El IdCardNit no está disponible", IsBadRequest: true);

            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(updateUserDto.RoleId);
            if (!roleExists.Success)
                return new UserResponseDto(false, "El rol no es válido", IsBadRequest:true);
            
            var currentRole = await _roleService.GetRoleByIdAsync(updateUserDto.RoleId); // ? Se puede ahorrar esta consulta si se pide en el dto el id de la empresa
            var availableEmail = await IsEmailAvailableInCompanyAsync(currentRole!.CompanyId, updateUserDto.Email);
            var currentUser = await GetUserByIdCardNitAsync(idCardNit);
            if (!availableEmail.Success && currentUser!.Email != updateUserDto.Email)
                return availableEmail;

            //TODO: La identification no se puede repetir en la misma empresa

            var hashedPassword = _passwordHasher.HashPassword(updateUserDto.Password);
            updateUserDto.Password = hashedPassword;

            var userEntity = updateUserDto.ToUserEntity();
            userEntity.IdCCNit = idCardNit;

            var isUpdated = await _userRepository.UpdateUserAsync(userEntity);
            if (isUpdated)
            {
                var updatedUser = await _userRepository.GetUserByIdCardNitAsync(idCardNit);
                return new UserResponseDto(true, "Se actualizó el usuario", updatedUser?.ToUserDto());
            }
            return new UserResponseDto(false, "No se pudo actualizar el usuario", IsBadRequest:true);
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


        public async Task<UserResponseDto> IsEmailAvailableInCompanyAsync(int companyId, string email)
        {
            var availability = await _userRepository.IsEmailAvailableInCompanyAsync(email, companyId);
            return availability
                        ? new UserResponseDto(true, "Email disponible en la empresa")
                        : new UserResponseDto(false, "Email no disponible en la empresa", IsConflict: true);
        }


    }

}
