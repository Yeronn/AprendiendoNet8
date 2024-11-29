using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleEntity>> GetRolesAsync(int companyId);
        Task<RoleEntity?> GetRoleByIdAsync(int id);
        Task<int> CreateRoleAsync(RoleEntity role);
        Task<bool> UpdateRoleAsync(RoleEntity role);
        Task<bool> UpdateRoleNameAsync(int id, string name);
        Task<bool> UpdateRoleDescriptionAsync(int id, string description);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> ExistRoleByIdAsync(int id);
        Task<bool> CheckRoleNameAvailabilityAsync(string name, int companyId);
        Task<bool> AddPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
        Task<IEnumerable<RoleEntity>> GetAllRolesByPermissionIdAsync(int permissionId);
        Task<RoleEntity?> GetRoleByUserIdAsync(int userId);
        Task<bool> IsRoleUnassignedAsync(int roleId); 
    }

}
