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

        public async Task<IEnumerable<UserDto>> GetUsersByCompanyIdAsync(int companyId)
        {
            var users = await _userRepository.GetUsersByCompanyIdAsync(companyId);
            return users.Select(u => u.ToUserDto());
        }

        public async Task<UserResponseDto> GetUserByCCNumberAndNitAsync(int ccNumber, int nit)
        {
            var companyId = await _companyService.GetCompanyIdByNitAsync(nit);
            if (companyId == null)
                return new UserResponseDto(false, "El nit no existe", IsNotFound: true);

            if (ccNumber <= 0)
                return new UserResponseDto(false, "El número de cédula debe ser positivo", IsBadRequest: true);

            var user = await GetUserByCCNumberAndCompanyIdAsync(ccNumber, (int)companyId);
            if (user == null)
                return new UserResponseDto(false, "El usuario no existe", IsNotFound: true);

            return new UserResponseDto(true, "Usuario válido", user);
        }

        public async Task<UserDto?> GetUserByIdAndCompanyIdAsync(int userId, int companyId)
        {
            var user = await _userRepository.GetUserByIdAndCompanyIdAsync(userId, companyId);
            return user?.ToUserDto();
        }


        public async Task<UserResponseDto> CreateUserAsync(int companyId, RegisterUserDto newUser)
        {
            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(newUser.RoleId, companyId); 
            if (!roleExists.Success)
                return new UserResponseDto(false, "El rol no es válido", IsBadRequest: true);

            bool ccNumberExists = await VerifyUserExistsByCCNumberAndCompanyIdAsync(newUser.CCNumber, companyId);
            if (ccNumberExists)
                return new UserResponseDto(false, "La cédula ya está regitrada en otro usuario", IsConflict: true);

            var availableEmail = await IsEmailAvailableInCompanyAsync(newUser.Email, companyId);
            if (!availableEmail)
                return new UserResponseDto(false, "Email no disponible", IsConflict: true);

            var hashedPassword = _passwordHasher.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var userEntity = newUser.ToUserEntity();
            userEntity.CompanyId = companyId;
            var createdUserId = await _userRepository.CreateUserAsync(userEntity);

            if (createdUserId.HasValue)
            {
                var createdUser = await GetUserByIdAndCompanyIdAsync(createdUserId.Value, companyId);
                return new UserResponseDto(true, "El usuario se creó correctamente", createdUser);
            }

            return new UserResponseDto(false, "Error en el servidor al crear al usuario");
        }


        public async Task<UserResponseDto> UpdateUserAsync(int userId, int companyId, UpdateUserDto updateUserDto)
        {
            var currentUser = await GetUserByIdAndCompanyIdAsync(userId, companyId);
            if (currentUser == null)
                return new UserResponseDto(false, "El usuario no existe", IsNotFound: true);

            bool IdCCNitExists = await VerifyUserExistsByCCNumberAndCompanyIdAsync(updateUserDto.CCNumber, companyId);
            if (IdCCNitExists && currentUser.CCNumber != updateUserDto.CCNumber)
                return new UserResponseDto(false, "La cédula ya está regitrada en otro usuario", IsConflict: true);

            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(updateUserDto.RoleId, companyId);
            if (!roleExists.Success)
                return new UserResponseDto(false, "El rol no existe", IsBadRequest:true);

            var availableEmail = await IsEmailAvailableInCompanyAsync(updateUserDto.Email, companyId);

            if (!availableEmail && currentUser.Email != updateUserDto.Email)
                return new UserResponseDto(false, "Email no disponible", IsConflict: true);

            var hashedPassword = _passwordHasher.HashPassword(updateUserDto.Password);
            updateUserDto.Password = hashedPassword;

            var userEntity = updateUserDto.ToUserEntity();
            userEntity.Id = userId;
            userEntity.CompanyId = companyId;

            var isUpdated = await _userRepository.UpdateUserAsync(userEntity);
            if (isUpdated)
            {
                var updatedUser = await _userRepository.GetUserByIdAsync(userId);
                return new UserResponseDto(true, "Se actualizó el usuario", updatedUser?.ToUserDto());
            }
            return new UserResponseDto(false, "No se pudo actualizar el usuario", IsBadRequest:true);
        }


        public async Task<bool> DeleteUserAsync(string IdCCNit)
        {
            return await _userRepository.DeleteUserAsync(IdCCNit);
        }


        public async Task<string?> GetPasswordByUserIdAsync(int userId)
        {
            var password = await _userRepository.GetPasswordByUserIdAsync(userId);
            if (password == null)
                return null;

            return password;
        }


        public async Task<bool> VerifyUserExistsByCCNumberAndCompanyIdAsync(int ccNumber, int companyId)
        {
            var exists = await _userRepository.VerifyUserExistsByCCNumberAndCompanyIdAsync(ccNumber, companyId);
            return exists;
        }



        private async Task<bool> IsEmailAvailableInCompanyAsync(string email, int companyId)
        {
            var availability = await _userRepository.IsEmailAvailableInCompanyAsync(email, companyId);
            return availability;
        }


        private async Task<UserDto?> GetUserByCCNumberAndCompanyIdAsync(int ccNumber, int companyId)
        {
            var user = await _userRepository.GetUserByCCNumberAndCompanyIdAsync(ccNumber, companyId);

            if (user == null)
                return null;

            return user.ToUserDto();
        }


        private async Task<bool> CheckUserExistsByIdAsync(int userId)
        {
            return await _userRepository.CheckUserExistsByIdAsync(userId);
        }


        private async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user?.ToUserDto();
        }


    }

}
