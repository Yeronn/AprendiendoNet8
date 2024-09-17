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
        Task<IEnumerable<PermissionDto>?> GetAllPermissionsAsync();
        Task<PermissionEntity?> GetPermissionByIdAsync(int id);
        Task<PermissionResponseDto> CreatePermissionAsync(CreatePermissionDto permissionDto);
        Task<PermissionResponseDto> UpdatePermissionAsync(int id, UpdatePermissionDto updatePermission);
        Task<bool> DeletePermissionAsync(int id);
        Task<IEnumerable<PermissionEntity>> GetAllPermissionsByRoleIdAsync(int roleId);
        Task<PermissionResponseDto> ValidatePermissionExistsByIdAsync(int id);
    }
}
