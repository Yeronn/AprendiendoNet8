using Application.DTOs;
using Application.DTOs.Role;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponseDto> GetRoleByIdAsync(int roleId, int companyId);
        Task<IEnumerable<RoleDto>> GetRolesAsync(int companyId);
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto createRole);
        Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto, int companyId);
        Task<RoleResponseDto> DeleteRoleAsync(int id);
        Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesWithoutPermissionsByPermissionIdAsync(int permissionId);
        Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<RoleResponseDto> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
        Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int id);
    }

}
