using Application.DTOs;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto> CreateUserAsync(RegisterUserDto newUser);
        Task<UserResponseDto> UpdateUserAsync(string idCCNit, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string idCCNit);
        Task<UserDto?> GetUserByIdCCNitAsync(string idCCNit);
        Task<string?> GetPasswordByIdCCNitAsync(string idCCNit);
        Task<bool> VerifyIdCCNitExistsAsync(string idCCNit);
        Task<UserResponseDto> IsEmailAvailableInCompanyAsync(string email, int companyId);
    }
}
