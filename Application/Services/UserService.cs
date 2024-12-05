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
        private readonly IRoleService _roleService;
        private readonly ICompanyService _companyService;
        private readonly IPasswordHasherService _passwordHasher;

        public UserService(IUserRepository userRepository, IRoleService roleService, ICompanyService companyService, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _roleService = roleService;
            _companyService = companyService;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(u => u.ToUserDto());
        }

        public async Task<UserDto?> GetUserByIdCCNitAsync(int IdCCNit)
        {
            var user = await _userRepository.GetUserByIdCCNitAsync(IdCCNit);
            return user?.ToUserDto();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user?.ToUserDto();
        }


        public async Task<UserResponseDto> CreateUserAsync(RegisterUserDto newUser)
        {
            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(newUser.RoleId);
            if (!roleExists.Success)
                return new UserResponseDto(false, "El rol no es válido", IsBadRequest:true);

            var company = await _companyService.GetCompanyByRoleIdAsync(newUser.RoleId);
            if (company == null)
                return new UserResponseDto(false, "La empresa a la que esta asociada el rol no existe: ");

            int idCCNit = newUser.CCIdentification + (int) company.NIT!; //TODO: Esta sumando y no concatenando
            bool IdCCNitExists = await VerifyIdCCNitExistsAsync(idCCNit);
            if (IdCCNitExists)
                return new UserResponseDto(false, "El IdCCNit no está disponible", IsConflict: true);

            var availableEmail = await IsEmailAvailableInCompanyAsync((int)company.Id!, newUser.Email);
            if (!availableEmail.Success)
                return new UserResponseDto(false, availableEmail.Message, IsConflict: availableEmail.IsConflict);

            var hashedPassword = _passwordHasher.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var userEntity = newUser.ToUserEntity();
            userEntity.IdCCNit = idCCNit;
            var createdUserId  = await _userRepository.CreateUserAsync(userEntity);

            if (createdUserId.HasValue)
            {
                var createdUser = await GetUserByIdAsync(createdUserId.Value);
                return new UserResponseDto(true, "El usuario se creó correctamente", createdUser);
            }
            return new UserResponseDto(false, "Error en el servidor al crear al usuario");
        }


        public async Task<UserResponseDto> UpdateUserAsync(int idCCNit, UpdateUserDto updateUserDto)
        {
            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(updateUserDto.RoleId);
            if (!roleExists.Success)
                return new UserResponseDto(false, "El rol no es válido", IsBadRequest:true);
            
            var company = await _companyService.GetCompanyByRoleIdAsync(updateUserDto.RoleId);
            if (company == null)
                return new UserResponseDto(false, "La empresa a la que esta asociada el rol no existe: ");
            
            // int newIdCCNit = updateUserDto.CCIdentification + (int) company.NIT!;
            bool idCCNitExists = await VerifyIdCCNitExistsAsync(idCCNit);
            if (!idCCNitExists)
                return new UserResponseDto(false, "El IdCCNit no está disponible", IsBadRequest: true);
    
            var availableEmail = await IsEmailAvailableInCompanyAsync((int)company.Id!, updateUserDto.Email);
            var currentUser = await GetUserByIdCCNitAsync(idCCNit);
            if (!availableEmail.Success && currentUser!.Email != updateUserDto.Email)
                return availableEmail;

            var hashedPassword = _passwordHasher.HashPassword(updateUserDto.Password);
            updateUserDto.Password = hashedPassword;

            var userEntity = updateUserDto.ToUserEntity();
            userEntity.IdCCNit = idCCNit;

            var isUpdated = await _userRepository.UpdateUserAsync(userEntity);
            if (isUpdated)
            {
                var updatedUser = await _userRepository.GetUserByIdCCNitAsync(idCCNit);
                return new UserResponseDto(true, "Se actualizó el usuario", updatedUser?.ToUserDto());
            }
            return new UserResponseDto(false, "No se pudo actualizar el usuario", IsBadRequest:true);
        }


        public async Task<bool> DeleteUserAsync(int IdCCNit)
        {
            return await _userRepository.DeleteUserAsync(IdCCNit);
        }
        

        public async Task<UserDto?> GetUserByLastJtiAsync(string lastJti)
        {
            var userEntity = await _userRepository.GetUserByLastJtiAsync(lastJti);
            return userEntity?.ToUserDto();  
        }


        public async Task<string?> GetPasswordByIdCCNitAsync(int IdCCNit)
        {
            var password = await _userRepository.GetPasswordByIdCCNitAsync(IdCCNit);
            if (password == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            return password;
        }


        public async Task<bool> UpdateLastJtiAsync(int IdCCNit, string lastJti)
        {
            // Verifica si el usuario existe con el IdCCNit (opcional)
            var user = await _userRepository.GetUserByIdCCNitAsync(IdCCNit);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Llama al repositorio para actualizar el LastJti
            return await _userRepository.UpdateLastJtiAsync(IdCCNit, lastJti);
        }


        public async Task<bool> VerifyIdCCNitExistsAsync(int IdCCNit)
        {
            var exists = await _userRepository.IdCCNitExistsAsync(IdCCNit);
            return exists;
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
