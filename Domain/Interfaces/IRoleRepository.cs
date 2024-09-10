using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<RoleEntity?> GetRoleByIdAsync(int id);
        Task<IEnumerable<RoleEntity>> GetAllRolesAsync();
        Task<bool> CreateRoleAsync(RoleEntity role);
        Task<bool> UpdateRoleAsync(RoleEntity role);
        Task<bool> DeleteRoleAsync(int id);
    }

}
