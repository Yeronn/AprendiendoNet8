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
        Task<RoleResponseDto> DeleteRoleAsync(int roleId, int companyId);
        Task<IEnumerable<RoleDto>?> GetRolesByPermissionIdAsync(int permissionId, int companyId);
        Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, int companyId);
        Task<RoleResponseDto> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds, int companyId);
        Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int roleId, int companyId);
    }

}
