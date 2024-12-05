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
        Task<UserDto?> GetUserByLastJtiAsync(string lastJti);
        Task<string?> GetPasswordByIdCCNitAsync(int IdCCNit);
        Task<bool> UpdateLastJtiAsync(int IdCCNit, string lastJti);
        Task<bool> VerifyIdCCNitExistsAsync(int IdCCNit);
        Task<UserResponseDto> IsEmailAvailableInCompanyAsync(int IdCCNit, string email);
    }
}
