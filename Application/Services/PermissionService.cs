using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }


        public async Task<IEnumerable<PermissionDto>?> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllPermissionsAsync();
            if (!permissions.Any())
                return null;
            var permissionsDto = permissions.Select(permission => permission.ToPermissionDto());
            return permissionsDto.ToList();
        }


        public async Task<PermissionEntity?> GetPermissionByIdAsync(int id)
        {
            return await _permissionRepository.GetPermissionByIdAsync(id);
        }


        public async Task<PermissionResponseDto> CreatePermissionAsync(CreatePermissionDto createPermission)
        {
            var nameAvalible = await CheckPermissionNameAvailabilityAsync(createPermission.Name!);
            if (!nameAvalible.Success)
                return nameAvalible;

            PermissionEntity permissionEntity = createPermission.ToEntity();

            bool success = await _permissionRepository.CreatePermissionAsync(permissionEntity);
            return success
                    ? new PermissionResponseDto(true, "Permiso creado exitosamente.", permissionEntity.ToPermissionDto())
                    : new PermissionResponseDto(false, "Error al crear el permiso");
        }


        public async Task<PermissionResponseDto> UpdatePermissionAsync(int id, UpdatePermissionDto updatePermission)
        {
            var validateId = ValidateIdsMatch(id, updatePermission.Id);
            if (!validateId.Success)
                return validateId;
            else
                updatePermission.Id = id;

            var permissionExist = await ValidatePermissionExistsByIdAsync(id);
            if (!permissionExist.Success)
                return permissionExist;

            var currentPermission = await _permissionRepository.GetPermissionByIdAsync(id);
            var updatePermissionEntity = updatePermission.ToPermissionEntity();

            if (!string.IsNullOrEmpty(updatePermissionEntity.Name) && updatePermissionEntity.Name != currentPermission!.Name)
            {
                var availableName = await CheckPermissionNameAvailabilityAsync(updatePermissionEntity.Name);
                if (!availableName.Success)
                    return availableName;
                await _permissionRepository.UpdatePermissionNameAsync(id, updatePermissionEntity.Name);
            }
            else
                updatePermissionEntity.Name = currentPermission!.Name;

            if (!string.IsNullOrEmpty(updatePermissionEntity.Description) && updatePermissionEntity.Description != currentPermission?.Description)
                await _permissionRepository.UpdatePermissionDescriptionAsync(id, updatePermissionEntity.Description);
            else
                updatePermissionEntity.Description = currentPermission?.Description;
            
            return new PermissionResponseDto(true, "Permiso actualizado exitosamente.", updatePermissionEntity.ToPermissionDto());
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


        public async Task<IEnumerable<PermissionEntity>> GetAllPermissionsByRoleIdAsync(int roleId)
        {
            //TODO: Validar que el role exista
            var permissionsByRol = await _permissionRepository.GetAllPermissionsByRoleIdAsync(roleId);
            return permissionsByRol;
        }


        public async Task<IEnumerable<PermissionEntity>> GetUniquePermissionsByRoleIdsAsync(IEnumerable<int> roleIds)
        {
            var permissions = await _permissionRepository.GetPermissionsByRoleIdsAsync(roleIds);

            //* verify that permissions are unique
            var uniquePermissions = permissions
                .GroupBy(permission => permission.Id)
                .Select(group => group.First())
                .ToList();

            return uniquePermissions;
        }


        public async Task<PermissionResponseDto> ValidatePermissionExistsByIdAsync(int id)
        {
            bool existingPermission = await _permissionRepository.ExistPermissionByIdAsync(id);
            if (!existingPermission)
                return new PermissionResponseDto(false, "El permiso no existe.", IsNotFound: true);

            return new PermissionResponseDto(true, "El permiso existe.");
        }




        private async Task<PermissionResponseDto> CheckPermissionNameAvailabilityAsync(string permissionName)
        {
            bool existingPermissionName = await _permissionRepository.ExistPermissionByNameAsync(permissionName);
            if (existingPermissionName)
                return new PermissionResponseDto(false, "El Permiso ya existe en el sistema", IsConflict: true);

            return new PermissionResponseDto(true, "Permiso válido.");
        }


        private PermissionResponseDto ValidateIdsMatch(int urlId, int? bodyId)
        {
            if (bodyId == null || bodyId == 0)
                return new PermissionResponseDto(true, "Asignar ID de la URL al objeto del body");

            if (bodyId != urlId)
                return new PermissionResponseDto(false, "El Id de la URL y del cuerpo no coinciden.");

            return new PermissionResponseDto(true, "Los Id son iguales");
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
