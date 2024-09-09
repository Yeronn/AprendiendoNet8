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

        public async Task<IEnumerable<PermissionEntity>> GetAll()
        {
            return await _permissionRepository.GetAll();
        }

        public async Task<PermissionEntity?> GetById(int id)
        {
            return await _permissionRepository.GetById(id);
        }

        public async Task<RegistrationResponse> Create(PermissionDto permissionDto)
        {
            bool nameAvalible = await _permissionRepository.VerifyUniqueName(permissionDto.Name);
            if (nameAvalible == false)
                return new RegistrationResponse("El nombre del permiso ya se encuentra en uso");

            PermissionEntity permission = permissionDto.ToEntity();
            int idCreatedPermission = await _permissionRepository.Create(permission);
            return new RegistrationResponse("El permiso se creo correctamente", idCreatedPermission);
        }

        public async Task<UpdateResponse> Update(int id, PermissionDto permissionDto)
        {
            var permissionExist = await _permissionRepository.ExistById(id);
            if (permissionExist == false)
                return new UpdateResponse("El permiso no existe");

            permissionDto.Id = id;
            var permission = permissionDto.ToEntity();

            var updatedPermission = await _permissionRepository.Update(permission);
            if (updatedPermission == false)
                return new UpdateResponse("El permiso no se pudo actualizar");

            return new UpdateResponse("El permiso se actualizó correctamente", permission.Name!);
        }

        public async Task<bool> Delete(int id)
        {
            return await _permissionRepository.Delete(id);
        }
    }
}
