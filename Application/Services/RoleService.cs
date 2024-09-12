﻿using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
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

        public async Task<RoleEntity?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                return null;


            return role;
        }

        public async Task<IEnumerable<RoleEntity>?> GetAllRolesAsync() => await _roleRepository.GetAllRolesAsync(); //TODO: Hacer un DTO para que no muestre los permisos

        public async Task<RoleResponse> CreateRoleAsync(CreateRolDto createRole)
        {
            var roleEntity = createRole.ToRoleEntity();

            var validName = await ValidateRoleNameAsync(createRole.Name!);
            if (!validName.Success)
                return validName;

            var success = await _roleRepository.CreateRoleAsync(roleEntity);
            return success
                ? new RoleResponse(true, "Rol creado exitosamente.", roleEntity.ToRoleWithoutPermissionsResponse())
                : new RoleResponse(false, "Error al crear el rol.");
        }

        public async Task<RoleResponse> UpdateRoleAsync(int id, UpdateRolDto updateRole)
        {
            if (updateRole.Id == null || updateRole.Id == 0)
                updateRole.Id = id;
            else if (updateRole.Id != id)
                return new RoleResponse(false, "El Id de la URL y del cuerpo no son iguales");

            bool existingRole = await _roleRepository.ExistRoleByIdAsync(id);
            if (existingRole == false)
                return new RoleResponse(false, "El rol no existe.");

            var validName = await ValidateRoleNameAsync(updateRole.Name!);
            if (!validName.Success)
                return validName;

            //TODO: El usuario depronto solo quiera actualizar el nombre o la descripción, entonces toca tomar esto en cuenta
            var roleEntity = updateRole.ToRoleEntity();

            var success = await _roleRepository.UpdateRoleAsync(roleEntity);
            return success
                ? new RoleResponse(true, "Rol actualizado exitosamente.", roleEntity.ToRoleWithoutPermissionsResponse())
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

        private async Task<RoleResponse> ValidateRoleNameAsync(string roleName)
        {
            bool existingRoleName = await _roleRepository.ExistRoleByNameAsync(roleName);
            if (existingRoleName)
            {
                return new RoleResponse(false, "El Rol ya existe en el sistema", IsConflict: true);
            }

            return new RoleResponse(true, "Rol válido.");
        }
    }

}
