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
        private readonly IPermissionService _permissionService;

        public RoleService(IRoleRepository roleRepository, IPermissionService permissionService)
        {
            _roleRepository = roleRepository;
            _permissionService = permissionService;
        }

        public async Task<RoleEntity?> GetRoleByIdAsync(int id) => await _roleRepository.GetRoleByIdAsync(id);

        public async Task<IEnumerable<RoleEntity>?> GetAllRolesAsync() => await _roleRepository.GetAllRolesAsync(); //TODO: Hacer un DTO para que no muestre los permisos

        public async Task<RoleResponse> CreateRoleAsync(RoleDto roleDto)
        {
            var roleEntity = new RoleEntity
            {
                Name = roleDto.Name,
                Permissions = roleDto.PermissionIds.Select(id => new PermissionEntity { Id = id }).ToList()
            };

            var success = await _roleRepository.CreateRoleAsync(roleEntity);
            return success
                ? new RoleResponse(true, "Rol creado exitosamente.", roleEntity)
                : new RoleResponse(false, "Error al crear el rol.");
        }

        public async Task<RoleResponse> UpdateRoleAsync(int id, RoleDto roleDto)
        {
            var existingRole = await _roleRepository.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return new RoleResponse(false, "El rol no existe.");
            }

            existingRole.Name = roleDto.Name;
            existingRole.Permissions = roleDto.PermissionIds.Select(id => new PermissionEntity { Id = id }).ToList();

            var success = await _roleRepository.UpdateRoleAsync(existingRole);
            return success
                ? new RoleResponse(true, "Rol actualizado exitosamente.", existingRole)
                : new RoleResponse(false, "Error al actualizar el rol.");
        }

        public async Task<RoleResponse> DeleteRoleAsync(int id)
        {
            var success = await _roleRepository.DeleteRoleAsync(id);
            return success
                ? new RoleResponse(true, "Rol eliminado exitosamente.")
                : new RoleResponse(false, "Error al eliminar el rol.");
        }


        public async Task<IEnumerable<RoleEntity>?> GetAllRolesWithTheirPermissionsAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync(); //TODO: Si realizo validaciones en el servicio que obtengo todos los roles, toca cambiar esta linea y en lugar de usar el repo use la funcion de este servicio

            if (roles == null)
                return null; //No obtuvo los roles

            foreach (var role in roles)
            {
                var permissions = await _permissionService.GetAllPermissionsByRoleIdAsync(role.Id);
                role.Permissions = permissions.ToList();
            }
            return roles;

        }
    }

}
