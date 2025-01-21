using Application.DTOs;
using Application.DTOs.Role;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionService _permissionService;
        private readonly ICompanyService _companyService;

        public RoleService(IRoleRepository roleRepository, IPermissionService permissionService, ICompanyService companyService)
        {
            _roleRepository = roleRepository;
            _permissionService = permissionService;
            _companyService = companyService;
        }


        public async Task<RoleResponseDto> GetRoleByIdAsync(int roleId, int companyId)
        {
            if (companyId <= 0)
                return new RoleResponseDto(false, "El identificador de la empresa no es válido.", IsBadRequest: true);

            var role = await _roleRepository.GetRoleByIdAsync(roleId, companyId);

            if (role == null)
                return new RoleResponseDto(false, $"El rol con el id {roleId} no existe en el sistema", IsNotFound: true);

            var roleDto = role!.ToDto();
            var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            roleDto.Permissions = permissions.ToList();
            return new RoleResponseDto(true, "", roleDto);
        }


        public async Task<IEnumerable<RoleDto>> GetRolesAsync(int companyId)
        {
            var roles = await _roleRepository.GetRolesAsync( companyId );
            var rolesDto = roles.Select(r => r.ToDto()).ToList();
            return rolesDto;
        }


        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto createRole)
        {
            if (createRole.PermissionsIds.Count == 0)
                return new RoleResponseDto(false, "No envió los permisos", IsBadRequest: true);

            var companyExists = await _companyService.ValidateCompanyExistsByIdAsync(createRole.CompanyId);
            if(companyExists.Success == false)
                return new RoleResponseDto(false, companyExists.Message);

            var nameAvalible = await CheckRoleNameAvailabilityAsync(createRole.Name, createRole.CompanyId);
            if (!nameAvalible.Success)
                return nameAvalible;

            var newRoleId = await _roleRepository.CreateRoleAsync(createRole.ToEntity());

            if(newRoleId == 0)
                return new RoleResponseDto(false, "Error al crear el rol");

            var assignedPermissions = await UpdateRolePermissions(newRoleId, createRole.CompanyId, createRole.PermissionsIds);
            if(!assignedPermissions.Success)
            {
                var deletedRole = await DeleteRoleAsync(newRoleId, createRole.CompanyId);
                if (deletedRole.Success)
                    return assignedPermissions;
                return new RoleResponseDto(false, "Se creó el rol, pero ocurrió un error: " + assignedPermissions.Message);
            }
            var createdRole = await GetRoleByIdAsync(newRoleId, createRole.CompanyId);
            return new RoleResponseDto(true, "Rol creado exitosamente.", createdRole.Role);
        }


        public async Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto, int companyId)
        {
            if (updateRoleDto.PermissionsIds.Count == 0)
                return new RoleResponseDto(false, "No envió los permisos", IsBadRequest: true);

            var companyExists = await _companyService.ValidateCompanyExistsByIdAsync(companyId);
            if (companyExists.Success == false)
                return new RoleResponseDto(false, companyExists.Message);

            var currentRoleResponse = await GetRoleByIdAsync(roleId, companyId);
            if (!currentRoleResponse.Success)
                return currentRoleResponse;

            var currentRole = currentRoleResponse.Role!;

            var availableName = await CheckRoleNameAvailabilityAsync(updateRoleDto.Name!, currentRole.CompanyId);
            if (!availableName.Success && updateRoleDto.Name != currentRole.Name )
                return availableName;

            var updateRoleEntity = updateRoleDto.ToEntity();
            updateRoleEntity.Id = roleId;
            updateRoleEntity.CompanyId = companyId;

            bool updatedRole = await _roleRepository.UpdateRoleAsync(updateRoleEntity);
            if(!updatedRole)
                return new RoleResponseDto(false, "Error al actualizar el rol");

            var currentRolePermissionIds = currentRole.Permissions.Select(p => (int)p.Id!).ToList();
            if (!currentRolePermissionIds.OrderBy(p => p).SequenceEqual(updateRoleDto.PermissionsIds.OrderBy(p => p)))
            {
                var updatedPermissions = await UpdateRolePermissions(roleId, updateRoleEntity.CompanyId, updateRoleDto.PermissionsIds);
                if (!updatedPermissions.Success)
                {
                    await _roleRepository.UpdateRoleAsync(currentRole.ToEntity());
                    return new RoleResponseDto(false, "No se pudo actualizar el rol: " + updatedPermissions.Message);
                }
            }
            var updatedRol = await GetRoleByIdAsync(roleId, companyId);
            return new RoleResponseDto(true, "Rol actualizado exitosamente.", updatedRol.Role);
        }


        public async Task<RoleResponseDto> DeleteRoleAsync(int roleId, int companyId)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId, companyId);
            if (!roleExists.Success)
                return roleExists;

            var isNotAssignedUser = await ValidateRoleNotAssignedToAnyUserAsync(roleId);
            if (!isNotAssignedUser.Success)
                return isNotAssignedUser;

            var rolePermissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            if (rolePermissions.Any())
            {
                var rolePermissionsIds = rolePermissions.Select(rp => (int)rp.Id!).ToList();
                var removedRolePermissionRecords = await RemovePermissionsFromRoleAsync(roleId, rolePermissionsIds, companyId);

                if (!removedRolePermissionRecords.Success)
                    return removedRolePermissionRecords;
            }

            var success = await _roleRepository.DeleteRoleAsync(roleId);
            return success
                ? new RoleResponseDto(true, "Rol eliminado exitosamente.")
                : new RoleResponseDto(false, "Error al eliminar el rol.");
        }


        public async Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> newPermissionIds, int companyId)
        {
            if(newPermissionIds.Count == 0)
                return new RoleResponseDto(false, "Está tratando de añadir permisos al rol, pero no envió los permisos", IsBadRequest: true);

            var roleExists = await ValidateRoleExistsByIdAsync(roleId, companyId);
            if (!roleExists.Success)
                return roleExists;

            var permissions = await _permissionService.GetPermissionsAsync();
            var permissionIds = permissions!.Select(p => p.Id).ToHashSet();
            var invalidPermissions = newPermissionIds.Where(pId => !permissionIds.Contains(pId)).ToList();
            if (invalidPermissions.Count != 0)
                return new RoleResponseDto(false, $"Los siguientes permisos no existen: {string.Join(", ", invalidPermissions)}", IsBadRequest: true);

            var newPermissions = await GetNewPermissionsAsync(roleId, newPermissionIds);
            if (newPermissions.Count == 0)
                return new RoleResponseDto(false, "Todos los permisos ya están asignados al rol.", IsBadRequest: true);

            var success = await _roleRepository.AddPermissionsToRoleAsync(roleId, newPermissions);
            return success
                ? new RoleResponseDto(true, "Permisos añadidos correctamente al rol.")
                : new RoleResponseDto(false, "Error al añadir permisos al rol.");
        }


        public async Task<RoleResponseDto> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds, int companyId)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId, companyId);
            if (!roleExists.Success)
                return roleExists;

            if (!permissionIds.Any())
                return new RoleResponseDto(false, "No se han enviado permisos para eliminar.", IsBadRequest: true);

            var currentPermissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            var currentPermissionIds = currentPermissions.Select(cp => cp.Id).ToHashSet();

            var invalidPermissions = permissionIds.Where(pId => !currentPermissionIds.Contains(pId)).ToList();
            if (invalidPermissions.Any())
                return new RoleResponseDto(false, $"El rol no tiene los siguientes permisos: {string.Join(", ", invalidPermissions)}", IsBadRequest: true);

            var success = await _roleRepository.RemovePermissionsFromRoleAsync(roleId, permissionIds);
            return success
                ? new RoleResponseDto(true, "Permisos eliminados correctamente del rol.")
                : new RoleResponseDto(false, "Error al eliminar permisos del rol.");
        }


        public async Task<IEnumerable<RoleDto>?> GetRolesByPermissionIdAsync(int permissionId, int companyId)
        {
            var permissionExists = await _permissionService.ValidatePermissionExistsByIdAsync(permissionId);
            if(!permissionExists.Success)
                return null;
            var rolesByPermission = await _roleRepository.GetAllRolesByPermissionIdAsync(permissionId, companyId);
            //TODO: Decidir si tambien traiga los permisos de cada rol
            var rolesByPermissionDto = rolesByPermission.Select(role => role.ToDto());
            return rolesByPermissionDto;
        }


        public async Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int roleId, int companyId)
        {
            bool roleExists = await _roleRepository.ExistRoleByIdAsync(roleId, companyId);
            if (!roleExists)
                return new RoleResponseDto(false, "El rol no existe.", IsNotFound: true);

            return new RoleResponseDto(true, "El rol existe.");
        }



        
        private async Task<RoleResponseDto> CheckRoleNameAvailabilityAsync(string roleName, int companyId)
        {
            bool nameIsUnique = await _roleRepository.CheckRoleNameAvailabilityAsync(roleName, companyId);
            if (!nameIsUnique)
                return new RoleResponseDto(false, "Nombre no disponible", IsConflict: true);

            return new RoleResponseDto(true, "Nombre válido.");
        }


        private async Task<List<int>> GetNewPermissionsAsync(int roleId, List<int> permissionIds)
        {
            var existingPermissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            return permissionIds.Except(existingPermissions.Select(p => (int)p.Id!)).ToList();
        }


        private async Task<RoleResponseDto> ValidateRoleNotAssignedToAnyUserAsync(int roleId)
        {
            var isNotAssigned = await _roleRepository.IsRoleUnassignedAsync(roleId); 

            if (isNotAssigned)
                return new RoleResponseDto(true, "El rol no está asociado a ningún usuario.");
            else
                return new RoleResponseDto(false, "El rol está asociado a un usuario.", IsConflict: true);
        }


        private async Task<RoleResponseDto> UpdateRolePermissions(int roleId, int companyId, List<int> permissionsIds) 
        {
            var response = await ValidateUniquePermissionsCombinationAsync(companyId, permissionsIds);
            if (!response.Success)
                return response;

            var currentPermissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            var currentPermissionsIds = currentPermissions.Select(p => (int)p.Id!).ToList();

            var permissionsToRemove = currentPermissionsIds.Except(permissionsIds).ToList();
            if (permissionsToRemove.Count != 0)
            {
                var permissionsRemoved = await RemovePermissionsFromRoleAsync(roleId, permissionsToRemove, companyId);
                if (!permissionsRemoved.Success)
                    return permissionsRemoved;
            }

            var permissionsToAdd = permissionsIds.Except(currentPermissionsIds).ToList();
            if (permissionsToAdd.Count != 0)
            {
                var permissionsAssigned = await AssignPermissionsToRoleAsync(roleId, permissionsToAdd, companyId);
                if (!permissionsAssigned.Success)
                    return permissionsAssigned;
            }
            return new RoleResponseDto(true, "Permisos del rol actualizados");
        }


        public async Task<RoleResponseDto> ValidateUniquePermissionsCombinationAsync(int companyId, List<int> permissionsIds)
        {
            //TODO: Crear metodo en el repo que me traiga solo los ids de los permisos en la tabla RolePermissions y que los agrupe en una lista segun el id del rol al que estan asignados
            var roles = await GetRolesAsync(companyId);

            foreach (var role in roles)
            {
                var rolePermissionIds = role.Permissions.Select(p => (int)p.Id!).ToList();
                if (rolePermissionIds.OrderBy(p => p).SequenceEqual(permissionsIds.OrderBy(p => p)))
                    return new RoleResponseDto(false, "La combinación de permisos ya la tiene otro rol", IsConflict: true);
            }
            return new RoleResponseDto(true, "Combinación de permisos válida");
        }


    }
}
