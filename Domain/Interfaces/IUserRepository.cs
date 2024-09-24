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
        Task<IEnumerable<UserEntity>> GetAllUsersAsync();
        Task<UserEntity?> GetUserByIdAsync(int id);
        Task<UserEntity?> GetUserByUsernameAsync(string username);
        Task<UserEntity?> CreateUserAsync(UserEntity newUser);
        Task UpdateUserJtiAsync(int userId, string jti);
        Task<UserEntity?> GetUserByJtiAsync(string jti);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsFullnameUniqueAsync(string fullname);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> UpdateFullnameAsync(int id, string fullname);
        Task<bool> UpdateUsernameAsync(int id, string username);
        Task<bool> UpdatePasswordAsync(int id, string password);
        Task<bool> ExistUserByIdAsync(int id);
        Task<bool> ExistUserByFullNameAsync(string fullname);
        Task<bool> ExistUserByUsernameAsync(string username);
    }
}
