using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }


        public async Task<IEnumerable<PermissionDto>?> GetPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetPermissionsAsync();
            if (!permissions.Any())
                return null;
            var permissionsDto = permissions.Select(permission => permission.ToDto());
            return permissionsDto.ToList();
        }


        public async Task<PermissionDto?> GetPermissionByIdAsync(int id)
        {
            var permissionExists = await ValidatePermissionExistsByIdAsync(id);
            if (!permissionExists.Success)
                return null;
            var permission = await _permissionRepository.GetPermissionByIdAsync(id);
            return permission!.ToDto();
        }


        public async Task<PermissionResponseDto> CreatePermissionAsync(CreateUpdatePermissionDto createPermissionDto)
        {
            var nameAvalible = await CheckPermissionNameAvailabilityAsync(createPermissionDto.Name!);
            if (!nameAvalible.Success)
                return nameAvalible;

            PermissionEntity permissionEntity = createPermissionDto.ToEntity();

            bool success = await _permissionRepository.CreatePermissionAsync(permissionEntity);
            return success
                    ? new PermissionResponseDto(true, "Permiso creado exitosamente.", permissionEntity.ToDto())
                    : new PermissionResponseDto(false, "Error al crear el permiso");
        }


        public async Task<PermissionResponseDto> UpdatePermissionAsync(int id, CreateUpdatePermissionDto updatePermissionDto)
        {
            var permissionExist = await ValidatePermissionExistsByIdAsync(id);
            if (!permissionExist.Success)
                return permissionExist;

            var currentPermission = await _permissionRepository.GetPermissionByIdAsync(id);
            var updatePermissionEntity = updatePermissionDto.ToEntity();
            updatePermissionEntity.Id = id;

            if (updatePermissionEntity.Name != currentPermission!.Name)
            {
                var availableName = await CheckPermissionNameAvailabilityAsync(updatePermissionEntity.Name!);
                if (!availableName.Success)
                    return availableName;
                await _permissionRepository.UpdatePermissionNameAsync(id, updatePermissionEntity.Name!);
            }

            if (!updatePermissionEntity.Description.IsNullOrEmpty() && updatePermissionEntity.Description != currentPermission?.Description)
                await _permissionRepository.UpdatePermissionDescriptionAsync(id, updatePermissionEntity.Description!);
            
            var updatedPermission = await GetPermissionByIdAsync(id);
            return new PermissionResponseDto(true, "Permiso actualizado exitosamente.", updatedPermission);
        }


        public async Task<PermissionResponseDto> DeletePermissionAsync(int permissionId)
        {
            var permissionExist = await ValidatePermissionExistsByIdAsync(permissionId);
            if (!permissionExist.Success)
                return permissionExist;

            // * Permission does not have to be assigned to a role 
            var isNotAssignedRole = await ValidatePermissionNotAssignedToAnyRoleAsync(permissionId);
            if (!isNotAssignedRole.Success)
                return isNotAssignedRole;

            bool success = await _permissionRepository.DeletePermissionAsync(permissionId);
            return success
                ? new PermissionResponseDto(true, "Permiso eliminado exitosamente.")
                : new PermissionResponseDto(false, "Error al eliminar el rol.");
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var permissionsByRol = await _permissionRepository.GetAllPermissionsByRoleIdAsync(roleId);
            return permissionsByRol.Select(p => p.ToDto());
        }


        public async Task<PermissionResponseDto> ValidatePermissionExistsByIdAsync(int id)
        {
            bool validatedPermission = await _permissionRepository.ExistPermissionByIdAsync(id);
            if (!validatedPermission)
                return new PermissionResponseDto(false, "El permiso no existe.", IsNotFound: true);

            return new PermissionResponseDto(true, "El permiso existe.");
        }


        public async Task<PermissionResponseDto> ValidatePermissionsExistAsync(List<int> permissionIds)
        {
            bool validatedPermissions = await _permissionRepository.ExistPermissionsAsync(permissionIds);
            if (!validatedPermissions)
                return new PermissionResponseDto(false, "Un permiso no es válido");
            return new PermissionResponseDto(true, "Los permisos son válidos");
        }


        private async Task<PermissionResponseDto> CheckPermissionNameAvailabilityAsync(string permissionName)
        {
            bool existingPermissionName = await _permissionRepository.ExistPermissionByNameAsync(permissionName);
            if (existingPermissionName)
                return new PermissionResponseDto(false, "El Permiso ya existe en el sistema", IsConflict: true);

            return new PermissionResponseDto(true, "Permiso válido.");
        }


        private async Task<PermissionResponseDto> ValidatePermissionNotAssignedToAnyRoleAsync(int permissionId)
        {
            var isNotAssigned = await _permissionRepository.IsPermissionNotAssignedToAnyRoleAsync(permissionId);

            if (isNotAssigned)
                return new PermissionResponseDto(true, "El permiso no está asociado a ningún rol");
            else
                return new PermissionResponseDto(false, "El permiso está asociado a algún rol", IsConflict: true);
        }


    }
}
