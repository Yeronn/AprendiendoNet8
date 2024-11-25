using Application.DTOs;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> CreateUserAsync(RegisterUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int idCardNit, UpdateUserDto updateUserDto);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserByIdCardNitAsync(int idCardNit);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<bool> DeleteUserAsync(int idCardNit);
        Task<bool> IsIdCardNitUniqueAsync(int idCardNit);
        Task<UserDto?> GetUserByLastJtiAsync(string lastJti);
        Task<string?> GetPasswordByIdCardNitAsync(int idCardNit);
        Task<bool> UpdateLastJtiAsync(int idCardNit, string lastJti);
        Task<bool> VerifyIdCardNitExistsAsync(int idCardNit);
        Task<UserJwtTokenDto> GetUserJwtTokenByIdCardNitAsync(int idCardNit);
    }
}
