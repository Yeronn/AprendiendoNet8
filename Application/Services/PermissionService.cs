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

        public async Task<IEnumerable<PermissionEntity>?> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllPermissionsAsync();
            if (!permissions.Any())
                return null;
            return permissions.ToList();
        }

        public async Task<PermissionEntity?> GetPermissionByIdAsync(int id)
        {
            return await _permissionRepository.GetPermissionByIdAsync(id);
        }

        public async Task<RegistrationResponse> CreatePermissionAsync(PermissionDto permissionDto)
        {
            bool nameAvalible = await _permissionRepository.VerifyUniquePermissionNameAsync(permissionDto.Name);
            if (nameAvalible == false)
                return new RegistrationResponse("El nombre del permiso ya se encuentra en uso");

            PermissionEntity permission = permissionDto.ToEntity();
            int idCreatedPermission = await _permissionRepository.CreatePermissionAsync(permission);
            return new RegistrationResponse("El permiso se creo correctamente", idCreatedPermission);
        }

        public async Task<UpdateResponse> UpdatePermissionAsync(int id, PermissionDto permissionDto)
        {
            var permissionExist = await _permissionRepository.ExistPermissionByIdAsync(id);
            if (permissionExist == false)
                return new UpdateResponse("El permiso no existe");

            permissionDto.Id = id;
            var permission = permissionDto.ToEntity();

            var updatedPermission = await _permissionRepository.UpdatePermissionAsync(permission);
            if (updatedPermission == false)
                return new UpdateResponse("El permiso no se pudo actualizar");

            return new UpdateResponse("El permiso se actualizó correctamente", permission.Name!);
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            return await _permissionRepository.DeletePermissionAsync(id);
        }

        public async Task<IEnumerable<PermissionEntity>?> GetAllPermissionsByRoleIdAsync(int roleId)
        {
            var permissionByRol = await _permissionRepository.GetAllPermissionsByRoleIdAsync(roleId);
            if (!permissionByRol.Any())
                return null;
            return permissionByRol.ToList();
        }
    }
}
