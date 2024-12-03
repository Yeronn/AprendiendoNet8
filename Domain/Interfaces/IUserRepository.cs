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
        Task<UserEntity?> GetUserByIdCCNitAsync(int idCCNit);
        Task<int?> CreateUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(UserEntity user);
        Task<bool> DeleteUserAsync(int id); 
        Task<UserEntity?> GetUserByLastJtiAsync(string lastJti);
        Task<bool> IdCCNitExistsAsync(int idCCNit);
        Task<string?> GetPasswordByIdCCNitAsync(int idCCNit);
        Task<bool> IsEmailAvailableInCompanyAsync(string email, int companyId);
        Task<bool> UpdateLastJtiAsync(int idCCNit, string lastJti);
    }
}
