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


        public async Task<UserResponseDto> CreateUserAsync(RegisterUserDto newUser)
        {
            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(newUser.RoleId, newUser.CompanyId); 
            if (!roleExists.Success)
                return new UserResponseDto(false, "El rol no es válido", IsBadRequest: true);

            var company = await _companyService.GetCompanyByRoleIdAsync(newUser.RoleId); //Agregar el companyId en el controlador a traves de los claims
            if (company == null)
                return new UserResponseDto(false, "La empresa a la que esta asociada el rol no existe: ");

            bool IdCCNitExists = await VerifyUserExistsByCCNumberAndCompanyIdAsync(newUser.CCNumber, newUser.CompanyId);
            if (IdCCNitExists)
                return new UserResponseDto(false, "La cédula ya está regitrada en otro usuario", IsConflict: true);

            var availableEmail = await IsEmailAvailableInCompanyAsync(newUser.Email, company.Id!);
            if (!availableEmail.Success)
                return new UserResponseDto(false, availableEmail.Message, IsConflict: availableEmail.IsConflict);

            var hashedPassword = _passwordHasher.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var userEntity = newUser.ToUserEntity();
            var createdUserId = await _userRepository.CreateUserAsync(userEntity);

            if (createdUserId.HasValue)
            {
                var createdUser = await GetUserByIdAndCompanyIdAsync(createdUserId.Value, company.Id);
                return new UserResponseDto(true, "El usuario se creó correctamente", createdUser);
            }

            return new UserResponseDto(false, "Error en el servidor al crear al usuario");
        }


        public async Task<UserResponseDto> UpdateUserAsync(string idCCNit, UpdateUserDto updateUserDto)
        {
            var roleExists = await _roleService.ValidateRoleExistsByIdAsync(updateUserDto.RoleId, 0); //TODO: Arreglar
            if (!roleExists.Success)
                return new UserResponseDto(false, "El rol no es válido", IsBadRequest:true);

            //var company = await _companyService.GetCompanyByRoleIdAsync(updateUserDto.RoleId);
            //if (company == null)
            //    return new UserResponseDto(false, "La empresa a la que esta asociada el rol no existe: ");

            //int newIdCCNit = updateUserDto.CCIdentification + (int)company.NIT!;
            //bool idCCNitExists = await VerifyUserExistsByCCNumberAndNitAsync(idCCNit);
            //if (!idCCNitExists)
            //    return new UserResponseDto(false, "El IdCCNit no está disponible", IsBadRequest: true);

            //var availableEmail = await IsEmailAvailableInCompanyAsync(updateUserDto.Email, (int)company.Id!);
            //var currentUser = await GetUserByIdCCNitAsync(idCCNit);
            //if (!availableEmail.Success && currentUser!.Email != updateUserDto.Email)
            //    return availableEmail;

            //var hashedPassword = _passwordHasher.HashPassword(updateUserDto.Password);
            //updateUserDto.Password = hashedPassword;

            //updateUserDto.IdCCNit = idCCNit;
            //var userEntity = updateUserDto.ToUserEntity();

            //var isUpdated = await _userRepository.UpdateUserAsync(userEntity);
            //if (isUpdated)
            //{
            //    var updatedUser = await _userRepository.GetUserByIdCCNitAsync(idCCNit);
            //    return new UserResponseDto(true, "Se actualizó el usuario", updatedUser?.ToUserDto());
            //}
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


        public async Task<UserResponseDto> IsEmailAvailableInCompanyAsync(string email, int companyId)
        {
            var availability = await _userRepository.IsEmailAvailableInCompanyAsync(email, companyId);
            return availability
                        ? new UserResponseDto(true, "Email disponible en la empresa")
                        : new UserResponseDto(false, "Email no disponible en la empresa", IsConflict: true);
        }




        public async Task<bool> VerifyUserExistsByCCNumberAndCompanyIdAsync(int ccNumber, int companyId)
        {
            var exists = await _userRepository.VerifyUserExistsByCCNumberAndCompanyIdAsync(ccNumber, companyId);
            return exists;
        }


        private async Task<UserDto?> GetUserByCCNumberAndCompanyIdAsync(int ccNumber, int companyId)
        {
            var user = await _userRepository.GetUserByCCNumberAndCompanyIdAsync(ccNumber, companyId);

            if (user == null)
                return null;

            return user.ToUserDto();
        }
    }

}
