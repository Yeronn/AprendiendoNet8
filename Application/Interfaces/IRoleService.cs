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
        Task<RoleResponse> CreateRoleAsync(CreateRolDto createRole);
        Task<RoleResponse> UpdateRoleAsync(int id, UpdateRolDto updateRole);
        Task<RoleResponse> DeleteRoleAsync(int id);
        Task<RoleWithoutPermissionsResponseDto?> GetRoleByIdAsync(int id);
        Task<RoleDto?> GetRoleWithTheirPermissionsByIdAsync(int id);
        Task<IEnumerable<RoleWithoutPermissionsResponseDto>?> GetAllRolesAsync();
        Task<IEnumerable<RoleDto>?> GetAllRolesWithTheirPermissionsAsync();
    }

}
