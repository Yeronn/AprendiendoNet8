using Application.DTOs;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByCCNumberAndNitAsync(int ccNumber, int nit);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserResponseDto> CreateUserAsync(RegisterUserDto newUser);
        Task<UserResponseDto> UpdateUserAsync(string idCCNit, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string idCCNit);
        Task<string?> GetPasswordByUserIdAsync(int userId);
        Task<UserResponseDto> IsEmailAvailableInCompanyAsync(string email, int companyId);
    }
}
