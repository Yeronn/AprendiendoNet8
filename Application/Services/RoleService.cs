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
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleResponse> Create(RoleDto roleDto)
        {
            var roleEntity = new RoleEntity
            {
                Name = roleDto.Name,
                Permissions = roleDto.PermissionIds.Select(id => new PermissionEntity { Id = id }).ToList()
            };

            var success = await _roleRepository.Create(roleEntity);
            return success
                ? new RoleResponse(true, "Rol creado exitosamente.", roleEntity)
                : new RoleResponse(false, "Error al crear el rol.");
        }

        public async Task<RoleResponse> Update(int id, RoleDto roleDto)
        {
            var existingRole = await _roleRepository.GetById(id);
            if (existingRole == null)
            {
                return new RoleResponse(false, "El rol no existe.");
            }

            existingRole.Name = roleDto.Name;
            existingRole.Permissions = roleDto.PermissionIds.Select(id => new PermissionEntity { Id = id }).ToList();

            var success = await _roleRepository.Update(existingRole);
            return success
                ? new RoleResponse(true, "Rol actualizado exitosamente.", existingRole)
                : new RoleResponse(false, "Error al actualizar el rol.");
        }

        public async Task<RoleResponse> Delete(int id)
        {
            var success = await _roleRepository.Delete(id);
            return success
                ? new RoleResponse(true, "Rol eliminado exitosamente.")
                : new RoleResponse(false, "Error al eliminar el rol.");
        }

        public async Task<RoleEntity?> GetById(int id) => await _roleRepository.GetById(id);

        public async Task<IEnumerable<RoleEntity>> GetAll() => await _roleRepository.GetAll();
    }

}
