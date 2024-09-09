using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<PermissionEntity>> GetAll();
        Task<PermissionEntity?> GetById(int id);
        Task<int> Create(PermissionEntity permission);
        Task<bool> Update(PermissionEntity permission);
        Task<bool> Delete(int id);
        Task<bool> ExistById(int id);
        Task<string?> GetName(int id);
        Task<bool> VerifyUniqueName(string name);
    }
}
