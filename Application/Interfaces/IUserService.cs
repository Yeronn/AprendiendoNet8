using Application.DTOs;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByCCNumberAndNitAsync(int ccNumber, int nit);
        Task<IEnumerable<UserDto>> GetUsersByCompanyIdAsync(int companyId);
        Task<UserDto?> GetUserByIdAndCompanyIdAsync(int userId, int companyId);
        Task<UserResponseDto> CreateUserAsync(RegisterUserDto newUser);
        Task<UserResponseDto> UpdateUserAsync(int userId, int companyId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string idCCNit);
        Task<string?> GetPasswordByUserIdAsync(int userId);
    }
}
