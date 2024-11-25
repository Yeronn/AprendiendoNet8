using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetUsersAsync();
        Task<UserEntity?> GetUserByIdAsync(int id);
        Task<UserEntity?> GetUserByIdCardNitAsync(int idCardNit);
        Task<int?> CreateUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(UserEntity user);
        Task<bool> DeleteUserAsync(int id); 

        // Task<UserEntity?> GetUserByUsernameAsync(string username);
        // Task UpdateUserJtiAsync(int userId, string jti);
        // Task<UserEntity?> GetUserByJtiAsync(string jti);
        // Task<bool> IsEmailUniqueAsync(string email);
        // Task<bool> IsFullnameUniqueAsync(string fullname);
        // Task<bool> IsUsernameUniqueAsync(string username);
        // Task<bool> UpdatePasswordAsync(int id, string password);
        // Task<bool> ExistUserByIdAsync(int id);
        // Task<bool> ExistUserByFullNameAsync(string fullname);
        // Task<bool> ExistUserByUsernameAsync(string username);
        // Task<IEnumerable<UserEntity>> GetAllUsersWithRolesAsync();
        // Task<bool> AddRolesToUserAsync(int userId, List<int> rolesIds);
        // Task<bool> RemoveRolesFromUserAsync(int userId, List<int> roleIds);
        // Task<IEnumerable<UserEntity>> GetAllUsersByRoleIdAsync(int roleId);
    }
}
