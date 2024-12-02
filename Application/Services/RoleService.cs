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


        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var exists = await ValidateRoleExistsByIdAsync(id);
            if (!exists.Success)
                return null;
            var role = await _roleRepository.GetRoleByIdAsync(id);
            var roleDto = role!.ToDto();
            var permissions = await _permissionService.GetPermissionsByRoleIdAsync(id);
            roleDto.Permissions = permissions.ToList();
            return roleDto;
        }


        public async Task<IEnumerable<RoleDto>> GetRolesAsync(int companyId)
        {
            var roles = await _roleRepository.GetRolesAsync( companyId );
            var rolesDto = roles.Select(r => r.ToDto()).ToList();
            return rolesDto;
        }


        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto createRole)
        {
            var nameAvalible = await CheckRoleNameAvailabilityAsync(createRole.Name!, createRole.CompanyId!);
            if (!nameAvalible.Success)
                return nameAvalible;

            var companyExists = await _companyService.ValidateCompanyExistsByIdAsync(createRole.CompanyId);
            if(companyExists.Success == false)
                return new RoleResponseDto(false, "La empresa no es válida");

            var newRoleId = await _roleRepository.CreateRoleAsync(createRole.ToEntity());

            if(newRoleId == 0)
                return new RoleResponseDto(false, "Error al crear el rol");

            var assignedPermissions = await UpdateRolePermissions(newRoleId, createRole.CompanyId, createRole.PermissionsIds);
            if(!assignedPermissions.Success)
            {
                var deletedRole = await DeleteRoleAsync(newRoleId);
                if (deletedRole.Success)
                    return new RoleResponseDto(false, "No se pudo crear el rol: " + assignedPermissions.Message);
                return new RoleResponseDto(false, "Se creó el rol, pero ocurrió un error al asignarle permisos: " + assignedPermissions.Message);
            }
            var createdRole = await GetRoleByIdAsync(newRoleId);
            return new RoleResponseDto(true, "Rol creado exitosamente.", createdRole);
        }


        public async Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto)
        {
            var currentRole = await GetRoleWithoutPermissionsByIdAsync(roleId);
            if (currentRole == null)
                return new RoleResponseDto(false, "El rol no existe.", IsNotFound: true);

            var updateRoleEntity = updateRoleDto.ToEntity();
            updateRoleEntity.Id = roleId;
            updateRoleEntity.CompanyId = currentRole.CompanyId;
            
            var availableName = await CheckRoleNameAvailabilityAsync(updateRoleDto.Name!, updateRoleEntity.CompanyId!);
            if (!availableName.Success && (updateRoleEntity.CompanyId != currentRole.CompanyId || updateRoleDto.Name != currentRole.Name) )
                return availableName;

            var companyExists = await _companyService.ValidateCompanyExistsByIdAsync(updateRoleEntity.CompanyId);
            if(companyExists.Success == false)
                return new RoleResponseDto(false, "La empresa no es válida");


            bool updatedRole = await _roleRepository.UpdateRoleAsync(updateRoleEntity);
            if(!updatedRole)
                return new RoleResponseDto(false, "Error al actualizar el rol");
                
            var updatedPermissions = await UpdateRolePermissions(roleId, updateRoleEntity.CompanyId, updateRoleDto.PermissionsIds);
            if(!updatedPermissions.Success)
            {
                await _roleRepository.UpdateRoleAsync(currentRole.ToEntity());
                return new RoleResponseDto(false, "No se pudo actualizar el rol: " + updatedPermissions.Message);
            }
            var updatedRol = await GetRoleByIdAsync(roleId);
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

            var rolePermissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            if (rolePermissions.Any())
            {
                var rolePermissionsIds = rolePermissions.Select(rp => (int)rp.Id!).ToList();
                var removedRolePermissionRecords = await RemovePermissionsFromRoleAsync(roleId, rolePermissionsIds);

                if (!removedRolePermissionRecords.Success)
                    return removedRolePermissionRecords;
            }

            var success = await _roleRepository.DeleteRoleAsync(roleId);
            return success
                ? new RoleResponseDto(true, "Rol eliminado exitosamente.")
                : new RoleResponseDto(false, "Error al eliminar el rol.");
        }


        public async Task<RoleWithoutPermissionsDto?> GetRoleWithoutPermissionsByIdAsync(int roleId)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId);
            if (!roleExists.Success)
                return null;
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            return role!.ToRoleWithoutPermissionsDto();
        }


        public async Task<RoleResponseDto> AssignPermissionsToRoleAsync(int roleId, List<int> newPermissionIds)
        {
            var roleExists = await GetRoleByIdAsync(roleId);
            if (roleExists == null)
                return new RoleResponseDto(false, "El rol no existe", IsNotFound: true);

            if(newPermissionIds.Count == 0)
                return new RoleResponseDto(false, "Está tratando de añadir permisos al rol, pero no envió los permisos", IsBadRequest: true);
            
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


        public async Task<RoleResponseDto> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var roleExists = await ValidateRoleExistsByIdAsync(roleId);
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


        public async Task<IEnumerable<RoleWithoutPermissionsDto>?> GetAllRolesWithoutPermissionsByPermissionIdAsync(int permissionId)
        {
            var permissionExists = await _permissionService.ValidatePermissionExistsByIdAsync(permissionId);
            if(!permissionExists.Success)
                return null;
            var rolesByPermission = await _roleRepository.GetAllRolesByPermissionIdAsync(permissionId);
            //TODO: Decidir si tambien traiga los permisos de cada rol
            var rolesByPermissionDto = rolesByPermission.Select(role => role.ToRoleWithoutPermissionsDto());
            return rolesByPermissionDto;
        }


        public async Task<RoleResponseDto> ValidateRoleExistsByIdAsync(int id)
        {
            bool roleExists = await _roleRepository.ExistRoleByIdAsync(id);
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
                var permissionsRemoved = await RemovePermissionsFromRoleAsync(roleId, permissionsToRemove);
                if (!permissionsRemoved.Success)
                    return permissionsRemoved;
            }

            var permissionsToAdd = permissionsIds.Except(currentPermissionsIds).ToList();
            if (permissionsToAdd.Count != 0)
            {
                var permissionsAssigned = await AssignPermissionsToRoleAsync(roleId, permissionsToAdd);
                if (!permissionsAssigned.Success)
                    return permissionsAssigned;
            }
            return new RoleResponseDto(true, "Permisos del rol actualizados");
        }


        public async Task<RoleResponseDto> ValidateUniquePermissionsCombinationAsync(int companyId, List<int> permissionsIds)
        {
            var roles = await GetRolesAsync(companyId);
            foreach (var role in roles)
            {
                var rolePermissionIds = role.Permissions.Select(p => (int)p.Id!).ToList();
                if (rolePermissionIds.OrderBy(p => p).SequenceEqual(permissionsIds.OrderBy(p => p)))
                    return new RoleResponseDto(false, "Esta combinación de permisos ya la tiene otro rol", IsConflict: true);
            }
            return new RoleResponseDto(true, "Combinación de permisos válida");
        }






    }
}
