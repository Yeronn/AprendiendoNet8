using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<RoleEntity?> GetRoleByIdAsync(int id);
        Task<IEnumerable<RoleEntity>> GetAllRolesAsync();
        Task<int?> CreateRoleAsync(RoleEntity role);
        Task<bool> UpdateRoleAsync(RoleEntity role);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> ExistRoleByIdAsync(int id);
        Task<bool> ExistRoleByNameAsync(string name);
        Task<bool> UpdateRoleNameAsync(int id, string name);
        Task<bool> UpdateRoleDescriptionAsync(int id, string description);
        Task<bool> AddPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
        Task<IEnumerable<RoleEntity>> GetAllRolesByPermissionIdAsync(int permissionId);
        Task<bool> IsRoleNotAssignedToAnyUserAsync(int roleId);
        Task<IEnumerable<RoleEntity>> GetAllRolesByUserIdAsync(int userId);
    }

}
