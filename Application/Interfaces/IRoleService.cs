using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponseDto> CreateRoleAsync(CreateUpdateRoleDto createRole);
        Task<RoleResponseDto> UpdateRoleAsync(int id, CreateUpdateRoleDto updateRoleDto);
        Task<RoleResponseDto> DeleteRoleAsync(int id);
        Task<RoleDto?> GetRoleByRolIdAsync(int id);
        Task<IEnumerable<RoleDto>?> GetAllRolesAsync();
        Task<RoleWithoutPermissionsDto?> GetRoleWithoutPermissionsByIdAsync(int id);
        Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesWithoutPermissionsAsync();
        Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesWithoutPermissionsByPermissionIdAsync(int permissionId);
        Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<RoleResponseDto> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
        Task<IEnumerable<RoleEntity>> GetAllRolesByUserIdAsync(int userId);
        Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int id);
    }

}
