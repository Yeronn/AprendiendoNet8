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
            var nameAvalible = await CheckRoleNameAvailabilityAsync(createRole.Name!);
            if (!nameAvalible.Success)
                return nameAvalible;
         
            var roleEntity = createRole.ToRoleEntity();

            var success = await _roleRepository.CreateRoleAsync(roleEntity); //TODO: Hacer que devuelva el id
            //TODO: Añadir permisos al crear el rol
            return success
                ? new RoleResponseDto(true, "Rol creado exitosamente.", roleEntity.ToRoleDto())
                : new RoleResponseDto(false, "Error al crear el rol.");
        }


        public async Task<RoleResponseDto> UpdateRoleAsync(int roleId, CreateUpdateRolDto updateRoleDto)
        {
            var currentRole = await GetRoleByIdAsync(roleId);
            if (currentRole == null)
                return new RoleResponseDto(false, "El rol no existe.", IsNotFound: true);

            if (updateRoleDto.Name != currentRole!.Name)
            {
                var availableName = await CheckRoleNameAvailabilityAsync(updateRoleDto.Name!);
                if (!availableName.Success)
                    return availableName;
                await _roleRepository.UpdateRoleNameAsync(roleId, updateRoleDto.Name!);
            }

            if (updateRoleDto.Description != currentRole.Description)
                await _roleRepository.UpdateRoleDescriptionAsync(roleId, updateRoleDto.Description!);

            var updatedPermissions = await UpdatePermissions(roleId, updateRoleDto.PermissionsIds);
            if(!updatedPermissions.Success)
                return updatedPermissions;

            var updatedRol = await GetRoleWithPermissionsByRolIdAsync(roleId);
            return new RoleResponseDto(true, "Rol actualizado exitosamente.", updatedRol);
        }


        public async Task<RoleResponseDto> DeleteRoleAsync(int roleId)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId);
            if (!roleExists.Success)
                return roleExists;
            
            var isNotAssignedUser = await ValidateRoleNotAssignedToAnyUserAsync(roleId);
            if (!isNotAssignedUser.Success)
                return isNotAssignedUser;

            // * Get role permissions to delete records in RolePermission table
            var rolePermissions = await _permissionService.GetAllPermissionsByRoleIdAsync(roleId);

            if (rolePermissions.Any())
            {
                List<int> rolePermissionsIds = rolePermissions.Select(rolePermission => rolePermission.Id).ToList();

                var removedRolePermissionRecords = await RemovePermissionsFromRoleAsync(roleId, rolePermissionsIds);

                if (!removedRolePermissionRecords.Success)
                    return removedRolePermissionRecords;
            }

            var success = await _roleRepository.DeleteRoleAsync(roleId);
            return success
                ? new RoleResponseDto(true, "Rol eliminado exitosamente.")
                : new RoleResponseDto(false, "Error al eliminar el rol.");
        }


        public async Task<RoleWithoutPermissionsDto?> GetRoleByIdAsync(int roleId)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId);
            if (!roleExists.Success)
                return null;
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            return role!.ToRoleWithoutPermissionsDto();
        }


        public async Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            if (!roles.Any())
                return null;
            var rolesDto = roles.Select(role => role.ToRoleWithoutPermissionsDto()).ToList();
            return rolesDto;
        }


        public async Task<RoleDto?> GetRoleWithPermissionsByRolIdAsync(int id)
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

            if(!permissionIds.Any())
                return new RoleResponseDto(false, "Está tratando de añadir permisos al rol, pero no envió los permisos", IsBadRequest: true);
            
            // Obtener los permisos que no existen
            var invalidPermissions = await GetInvalidPermissionsAsync(permissionIds);
            if (invalidPermissions.Any())
                return new RoleResponseDto(false, $"Los siguientes permisos no existen: {string.Join(", ", invalidPermissions)}", IsBadRequest: true);

            var newPermissions = await GetNewPermissionsAsync(roleId, permissionIds);
            if (!newPermissions.Any())
                return new RoleResponseDto(false, "Todos los permisos ya están asignados al rol.", IsBadRequest: true);

            var success = await _roleRepository.AddPermissionsToRoleAsync(roleId, newPermissions);
            return success
                ? new RoleResponseDto(true, "Permisos añadidos correctamente al rol.")
                : new RoleResponseDto(false, "Error al añadir permisos al rol.");
        }


        public async Task<RoleResponseDto> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId);
            if (!roleExists.Success)
                return roleExists;

            if(!permissionIds.Any())
                return new RoleResponseDto(false, "Está tratando de eliminar permisos del rol, pero no envió los permisos", IsBadRequest: true);
            
            var currentPermissions = await _permissionService.GetAllPermissionsByRoleIdAsync(roleId);

            // * Verify that role have the permissions to delete
            var invalidPermissions = permissionIds.Except(currentPermissions.Select(p => p.Id)).ToList();
            if (invalidPermissions.Any())
            {
                return new RoleResponseDto(false, $"El rol no tiene los siguientes permisos: {string.Join(", ", invalidPermissions)}", IsBadRequest: true);
            }

            var success = await _roleRepository.RemovePermissionsFromRoleAsync(roleId, permissionIds);
            return success
                ? new RoleResponseDto(true, "Permisos eliminados correctamente del rol.")
                : new RoleResponseDto(false, "Error al eliminar permisos del rol.");
        }


        public async Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesByPermissionIdAsync(int permissionId)
        {
            var permissionExists = await _permissionService.ValidatePermissionExistsByIdAsync(permissionId);
            if(!permissionExists.Success)
                return null;
            var rolesByPermission = await _roleRepository.GetAllRolesByPermissionIdAsync(permissionId);
            //TODO: Decidir si tambien traiga los permisos de cada rol
            var rolesByPermissionDto = rolesByPermission.Select(role => role.ToRoleWithoutPermissionsDto());
            return rolesByPermissionDto;
        }


        public async Task<IEnumerable<RoleEntity>> GetAllRolesByUserIdAsync(int userId)
        {
            //TODO: Validar que el usuario existe
            var rolesByUser = await _roleRepository.GetAllRolesByUserIdAsync(userId);
            //TODO: Decidir si tambien mostrar los permisos de los roles del usuario
            return rolesByUser;
        }


        public async Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int id)
        {
            bool roleExists = await _roleRepository.ExistRoleByIdAsync(id);
            if (!roleExists)
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


        private async Task<List<int>> GetInvalidPermissionsAsync(List<int> permissionIds)
        {
            var invalidPermissions = new List<int>();
            foreach (var permissionId in permissionIds)
            {
                var permissionExists = await _permissionService.ValidatePermissionExistsByIdAsync(permissionId);
                if (!permissionExists.Success)
                    invalidPermissions.Add(permissionId);
            }
            return invalidPermissions;
        }


        private async Task<List<int>> GetNewPermissionsAsync(int roleId, List<int> permissionIds)
        {
            var existingPermissions = await _permissionService.GetAllPermissionsByRoleIdAsync(roleId);
            return permissionIds.Except(existingPermissions.Select(p => p.Id)).ToList();
        }


        private async Task<RoleResponseDto> ValidateRoleNotAssignedToAnyUserAsync(int roleId)
        {
            var isNotAssigned = await _roleRepository.IsRoleNotAssignedToAnyUserAsync(roleId);

            if (isNotAssigned)
                return new RoleResponseDto(true, "El rol no está asociado a ningún rol");
            else
                return new RoleResponseDto(false, "El rol está asociado a algún rol", IsConflict: true);
        }


        private async Task<RoleResponseDto> UpdatePermissions(int roleId, List<int> permissionsIds) {
            
            var currentPermissions = await _permissionService.GetAllPermissionsByRoleIdAsync(roleId);
            var currentPermissionsIds = currentPermissions.Select(p => p.Id).ToList();

            var permissionsToRemove = currentPermissionsIds.Except(permissionsIds).ToList();
            if (permissionsToRemove.Any())
            {
                var permissionsRemoved = await RemovePermissionsFromRoleAsync(roleId, permissionsToRemove);
                if (!permissionsRemoved.Success)
                    return permissionsRemoved;
            }

            var permissionsToAdd = permissionsIds.Except(currentPermissionsIds).ToList();
            if (permissionsToAdd.Any())
            {
                var permissionsAssigned = await AssignPermissionsToRoleAsync(roleId, permissionsToAdd);
                if (!permissionsAssigned.Success)
                    return permissionsAssigned;
            }
            return new RoleResponseDto(true, "Roles actualizados");
        }
    }

}
