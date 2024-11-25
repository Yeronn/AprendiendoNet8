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
        Task<UserEntity?> GetUserByLastJtiAsync(string lastJti);
        Task<bool> IdCardNitExistsAsync(int idCardNit);
        Task<string?> GetPasswordByIdCardNitAsync(int idCardNit);
        Task<bool> UpdateLastJtiAsync(int idCardNit, string lastJti);
    }
}
