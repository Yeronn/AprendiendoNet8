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
        Task<RoleResponse> Create(RoleDto roleDto);
        Task<RoleResponse> Update(int id, RoleDto roleDto);
        Task<RoleResponse> Delete(int id);
        Task<RoleEntity?> GetById(int id);
        Task<IEnumerable<RoleEntity>> GetAll();
    }

}
