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
        Task<bool> CreatePermissionAsync(PermissionEntity permission);
        Task<bool> UpdatePermissionAsync(PermissionEntity permission);
        Task<bool> UpdatePermissionNameAsync(int id, string name);
        Task<bool> UpdatePermissionDescriptionAsync(int id, string description);
        Task<bool> DeletePermissionAsync(int id);
        Task<bool> ExistPermissionByIdAsync(int id);
        Task<bool> ExistPermissionsAsync(List<int> permissionIds);
        Task<bool> ExistPermissionByNameAsync(string name);
        Task<string?> GetPermissionNameAsync(int id);
        Task<IEnumerable<PermissionEntity>> GetAllPermissionsByRoleIdAsync(int roleId);
        Task<IEnumerable<PermissionEntity>> GetPermissionsByRoleIdsAsync(IEnumerable<int> roleIds);
        Task<bool> IsPermissionNotAssignedToAnyRoleAsync(int permissionId);
    }
}
