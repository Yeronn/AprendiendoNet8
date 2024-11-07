using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponseDto> CreateRoleAsync(CreateRolDto createRole);
        Task<RoleResponseDto> UpdateRoleAsync(int id, CreateUpdateRolDto updateRoleDto);
        Task<RoleResponseDto> DeleteRoleAsync(int id);
        Task<RoleWithoutPermissionsDto?> GetRoleByIdAsync(int id);
        Task<RoleDto?> GetRoleWithPermissionsByRolIdAsync(int id);
        Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesAsync();
        Task<IEnumerable<RoleDto>?> GetAllRolesWithPermissionsAsync();
        Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<RoleResponseDto> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
        Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesByPermissionIdAsync(int permissionId);
        Task<IEnumerable<RoleEntity>> GetAllRolesByUserIdAsync(int userId);
        Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int id);
    }

}
