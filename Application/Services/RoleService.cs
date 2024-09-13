using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;

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
            var validateId = ValidateUrlIdWithBodyId(id, updateRole.Id);
            if (!validateId.Success)
                return validateId;
            else
                updateRole.Id = id;

            var roleExist = await ValidateRoleExistsByIdAsync(id);
            if (!roleExist.Success)
                return roleExist;

            var currentRole = await _roleRepository.GetRoleByIdAsync(id);
            var roleEntity = updateRole.ToRoleEntity();

            if (!string.IsNullOrEmpty(roleEntity.Name) && roleEntity.Name != currentRole!.Name)
            {
                var validName = await ValidateRoleNameAsync(roleEntity.Name);
                if (!validName.Success)
                    return validName;

                await _roleRepository.UpdateRoleNameAsync(id, roleEntity.Name);
            }
            else
                roleEntity.Name = currentRole!.Name;

            if (!string.IsNullOrEmpty(roleEntity.Description) && roleEntity.Description != currentRole?.Description)
            {
                await _roleRepository.UpdateRoleDescriptionAsync(id, roleEntity.Description);
            }
            else
                roleEntity.Description = currentRole?.Description;

            return new RoleResponse(true, "Rol actualizado exitosamente.", roleEntity.ToRoleWithoutPermissionsResponse());
        }

        public async Task<RoleResponse> DeleteRoleAsync(int id)
        {
            var roleExist = await ValidateRoleExistsByIdAsync(id);
            if (!roleExist.Success)
                return roleExist;
            var success = await _roleRepository.DeleteRoleAsync(id);
            return success
                ? new RoleResponse(true, "Rol eliminado exitosamente.")
                : new RoleResponse(false, "Error al eliminar el rol.");
        }
     
        public async Task<RoleWithoutPermissionsResponseDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                return null;
            return role.ToRoleWithoutPermissionsResponse();
        }

        public async Task<IEnumerable<RoleWithoutPermissionsResponseDto>?> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            if (!roles.Any())
                return null;
            var rolesDto = roles.Select(role => role.ToRoleWithoutPermissionsResponse()).ToList();
            return rolesDto;
        }

        public async Task<RoleDto?> GetRoleWithTheirPermissionsByIdAsync(int id)
        {
            var role = await GetRoleByIdAsync(id);
            if (role == null)
                return null;
            var roleDto = role.ToRoleDto();
            var permissions = await _permissionService.GetAllPermissionsByRoleIdAsync(role.Id);
            roleDto.Permissions = permissions.ToList();
            return roleDto;
        }

        public async Task<IEnumerable<RoleDto>?> GetAllRolesWithTheirPermissionsAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            if (!roles.Any())
                return null;

            foreach (var role in roles)
            {
                var permissions = await _permissionService.GetAllPermissionsByRoleIdAsync(role.Id);
                role.Permissions = permissions.ToList();
            }
            return roles.Select(role => role.ToRoleDto());
        }

        public async Task<RoleResponse> AddPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId);
            if (!roleExists.Success)
                return roleExists;

            // Obtener los permisos que no existen
            var invalidPermissions = new List<int>();
            foreach (var permissionId in permissionIds)
            {
                if (!await _permissionService.ValidatePermissionExistsByIdAsync(permissionId))
                    invalidPermissions.Add(permissionId);
            }

            if (invalidPermissions.Any())
                return new RoleResponse(false, $"Los siguientes permisos no existen: {string.Join(", ", invalidPermissions)}", IsBadRequest: true);

            // Añadir permisos al rol
            var success = await _roleRepository.AddPermissionsToRoleAsync(roleId, permissionIds);
            return success
                ? new RoleResponse(true, "Permisos añadidos correctamente al rol.")
                : new RoleResponse(false, "Error al añadir permisos al rol.");
        }



        private async Task<RoleResponse> ValidateRoleExistsByIdAsync(int id)
        {
            bool existingRole = await _roleRepository.ExistRoleByIdAsync(id);
            if (!existingRole)
                return new RoleResponse(false, "El rol no existe.", IsNotFound: true);

            return new RoleResponse(true, "El rol existe.");
        }

        private async Task<RoleResponse> ValidateRoleNameAsync(string roleName)
        {
            bool existingRoleName = await _roleRepository.ExistRoleByNameAsync(roleName);
            if (existingRoleName)
                return new RoleResponse(false, "El Rol ya existe en el sistema", IsConflict: true);

            return new RoleResponse(true, "Rol válido.");
        }

        private RoleResponse ValidateUrlIdWithBodyId(int urlId, int? bodyId)
        {
            if (bodyId == null || bodyId == 0)
                return new RoleResponse(true, "Asignar ID de la URL al objeto del body");

            if (bodyId != urlId)
                return new RoleResponse(false, "El Id de la URL y del cuerpo no coinciden.");

            return new RoleResponse(true, "Los Id son iguales");
        }

    }

}
