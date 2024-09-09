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
        Task<RoleEntity?> GetById(int id);
        Task<IEnumerable<RoleEntity>> GetAll();
        Task<bool> Create(RoleEntity role);
        Task<bool> Update(RoleEntity role);
        Task<bool> Delete(int id);
    }

}
