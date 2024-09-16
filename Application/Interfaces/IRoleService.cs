using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<PermissionResponseDto> CreateRoleAsync(CreateRolDto createRole);
        Task<PermissionResponseDto> UpdateRoleAsync(int id, UpdateRolDto updateRole);
        Task<PermissionResponseDto> DeleteRoleAsync(int id);
        Task<RoleWithoutPermissionsDto?> GetRoleByIdAsync(int id);
        Task<RoleDto?> GetRoleWithPermissionsByIdAsync(int id);
        Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesAsync();
        Task<IEnumerable<RoleDto>?> GetAllRolesWithPermissionsAsync();
        Task<PermissionResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
    }

}
