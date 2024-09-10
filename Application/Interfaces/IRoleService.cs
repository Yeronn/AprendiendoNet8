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
        Task<RoleEntity?> GetRoleByIdAsync(int id);
        Task<IEnumerable<RoleEntity>?> GetAllRolesAsync();
        Task<RoleResponse> CreateRoleAsync(RoleDto roleDto);
        Task<RoleResponse> UpdateRoleAsync(int id, RoleDto roleDto);
        Task<RoleResponse> DeleteRoleAsync(int id);
        Task<IEnumerable<RoleEntity>?> GetAllRolesWithTheirPermissionsAsync();
    }

}
