using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionEntity>> GetAll();
        Task<PermissionEntity?> GetById(int id);
        Task<RegistrationResponse> Create(PermissionDto permissionDto);
        Task<UpdateResponse> Update(int id, PermissionDto permissionDto);
        Task<bool> Delete(int id);
    }
}
