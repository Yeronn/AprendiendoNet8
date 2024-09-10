using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<PermissionEntity>> GetAllPermissionsAsync();
        Task<PermissionEntity?> GetPermissionByIdAsync(int id);
        Task<int> CreatePermissionAsync(PermissionEntity permission);
        Task<bool> UpdatePermissionAsync(PermissionEntity permission);
        Task<bool> DeletePermissionAsync(int id);
        Task<bool> ExistPermissionByIdAsync(int id);
        Task<bool> VerifyUniquePermissionNameAsync(string name);
        Task<string?> GetPermissionNameAsync(int id);
        Task<IEnumerable<PermissionEntity>> GetAllPermissionsByRoleIdAsync(int roleId);
    }
}
