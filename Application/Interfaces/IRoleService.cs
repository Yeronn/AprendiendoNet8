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
        Task<RoleResponseDto> CreateRoleAsync(CreateRolDto createRole);
        Task<RoleResponseDto> UpdateRoleAsync(int id, UpdateRolDto updateRole);
        Task<RoleResponseDto> DeleteRoleAsync(int id);
        Task<RoleWithoutPermissionsDto?> GetRoleByIdAsync(int id);
        Task<RoleDto?> GetRoleWithPermissionsByIdAsync(int id);
        Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesAsync();
        Task<IEnumerable<RoleDto>?> GetAllRolesWithPermissionsAsync();
        Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
    }

}
