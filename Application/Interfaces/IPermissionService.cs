using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionEntity>?> GetAllPermissionsAsync();
        Task<PermissionEntity?> GetPermissionByIdAsync(int id);
        Task<RegistrationResponse> CreatePermissionAsync(PermissionDto permissionDto);
        Task<UpdateResponse> UpdatePermissionAsync(int id, PermissionDto permissionDto);
        Task<bool> DeletePermissionAsync(int id);
        Task<IEnumerable<PermissionEntity>?> GetAllPermissionsByRoleIdAsync(int roleId);
    }
}
