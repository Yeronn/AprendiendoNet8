using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<PermissionEntity>> GetAll()
        {
            return await _permissionRepository.GetAll();
        }

        public async Task<PermissionEntity?> GetById(int id)
        {
            return await _permissionRepository.GetById(id);
        }

        public async Task<int> Create(PermissionDto permissionDto)
        {
            var permission = new PermissionEntity
            {
                Name = permissionDto.Name,
                Description = permissionDto.Description
            };

            return await _permissionRepository.Create(permission);
        }

        public async Task<bool> Update(int id, PermissionDto permissionDto)
        {
            var permission = new PermissionEntity
            {
                Id = id,
                Name = permissionDto.Name,
                Description = permissionDto.Description
            };

            return await _permissionRepository.Update(permission);
        }

        public async Task<bool> Delete(int id)
        {
            return await _permissionRepository.Delete(id);
        }
    }
}
