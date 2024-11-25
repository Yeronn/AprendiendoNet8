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
    }
}
