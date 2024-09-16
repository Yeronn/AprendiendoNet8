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


        public async Task<RoleResponseDto> CreateRoleAsync(CreateRolDto createRole)
        {
            var roleEntity = createRole.ToRoleEntity();

            var validName = await CheckRoleNameAvailabilityAsync(createRole.Name!);
            if (!validName.Success)
                return validName;

            var success = await _roleRepository.CreateRoleAsync(roleEntity);
            return success
                ? new RoleResponseDto(true, "Rol creado exitosamente.", roleEntity.ToRoleWithoutPermissionsResponse())
                : new RoleResponseDto(false, "Error al crear el rol.");
        }

        public async Task<RoleResponseDto> UpdateRoleAsync(int id, UpdateRolDto updateRole)
        {
            var validateId = ValidateIdsMatch(id, updateRole.Id);
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
                var validName = await CheckRoleNameAvailabilityAsync(roleEntity.Name);
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

            return new RoleResponseDto(true, "Rol actualizado exitosamente.", roleEntity.ToRoleWithoutPermissionsResponse());
        }

        public async Task<RoleResponseDto> DeleteRoleAsync(int id)
        {
            var roleExist = await ValidateRoleExistsByIdAsync(id);
            if (!roleExist.Success)
                return roleExist;
            var success = await _roleRepository.DeleteRoleAsync(id);
            return success
                ? new RoleResponseDto(true, "Rol eliminado exitosamente.")
                : new RoleResponseDto(false, "Error al eliminar el rol.");
        }
     
        public async Task<RoleWithoutPermissionsDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                return null;
            return role.ToRoleWithoutPermissionsResponse();
        }

        public async Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            if (!roles.Any())
                return null;
            var rolesDto = roles.Select(role => role.ToRoleWithoutPermissionsResponse()).ToList();
            return rolesDto;
        }

        public async Task<RoleDto?> GetRoleWithPermissionsByIdAsync(int id)
        {
            var role = await GetRoleByIdAsync(id);
            if (role == null)
                return null;
            var roleDto = role.ToRoleDto();
            var permissions = await _permissionService.GetAllPermissionsByRoleIdAsync(role.Id);
            roleDto.Permissions = permissions.ToList();
            return roleDto;
        }

        public async Task<IEnumerable<RoleDto>?> GetAllRolesWithPermissionsAsync()
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

        public async Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
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
                return new RoleResponseDto(false, $"Los siguientes permisos no existen: {string.Join(", ", invalidPermissions)}", IsBadRequest: true);

            // Añadir permisos al rol
            var success = await _roleRepository.AddPermissionsToRoleAsync(roleId, permissionIds);
            return success
                ? new RoleResponseDto(true, "Permisos añadidos correctamente al rol.")
                : new RoleResponseDto(false, "Error al añadir permisos al rol.");
        }



        private async Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int id)
        {
            bool existingRole = await _roleRepository.ExistRoleByIdAsync(id);
            if (!existingRole)
                return new RoleResponseDto(false, "El rol no existe.", IsNotFound: true);

            return new RoleResponseDto(true, "El rol existe.");
        }

        private async Task<RoleResponseDto> CheckRoleNameAvailabilityAsync(string roleName)
        {
            bool existingRoleName = await _roleRepository.ExistRoleByNameAsync(roleName);
            if (existingRoleName)
                return new RoleResponseDto(false, "El Rol ya existe en el sistema", IsConflict: true);

            return new RoleResponseDto(true, "Rol válido.");
        }

        private RoleResponseDto ValidateIdsMatch(int urlId, int? bodyId)
        {
            if (bodyId == null || bodyId == 0)
                return new RoleResponseDto(true, "Asignar ID de la URL al objeto del body");

            if (bodyId != urlId)
                return new RoleResponseDto(false, "El Id de la URL y del cuerpo no coinciden.");

            return new RoleResponseDto(true, "Los Id son iguales");
        }

    }

}
