using Application.DTOs;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto> CreateUserAsync(RegisterUserDto newUser);
        Task<UserResponseDto> UpdateUserAsync(int idCCNit, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int IdCCNit);
        Task<UserDto?> GetUserByIdCCNitAsync(int IdCCNit);
        Task<string?> GetPasswordByIdCCNitAsync(int IdCCNit);
        Task<bool> VerifyIdCCNitExistsAsync(int IdCCNit);
        Task<UserResponseDto> IsEmailAvailableInCompanyAsync(int IdCCNit, string email);
    }
}
