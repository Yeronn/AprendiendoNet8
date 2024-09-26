using Application.DTOs;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserWithoutRolesDto>?> GetAllUsersAsync();
        Task<UserWithoutRolesDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<UserDto?> GetUserWithRolesByUserIdAsync(int userId);
        Task<IEnumerable<UserDto>?> GetAllUsersWithRolesAsync();
        Task<UserResponseDto> AssignRolesToUserAsync(int userId, List<int> rolesIds);
        Task<UserResponseDto> RemoveRoleFromUserAsync(int userId, List<int> roleIds);
    }
}
