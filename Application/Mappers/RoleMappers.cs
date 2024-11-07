using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    public static class RoleMappers
    {
        public static RoleEntity ToRoleEntity(this CreateRolDto createRol)
        {
            return new RoleEntity
            {
                Name = createRol.Name,
                Description = createRol.Description,
            };
        }

        public static RoleEntity ToRoleEntity(this CreateUpdateRolDto role)
        {
            return new RoleEntity
            {
                Name = role.Name,
                Description = role.Description
            };
        }


        public static RoleWithoutPermissionsDto ToRoleWithoutPermissionsDto(this RoleEntity roleEntity)
        {
            return new RoleWithoutPermissionsDto
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name,
                Description = roleEntity.Description
            };
        }

        public static RoleDto ToRoleDto(this RoleEntity roleEntity)
        {
            return new RoleDto
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name,
                Description = roleEntity.Description,
                Permissions = roleEntity.Permissions
            };
        }

        public static RoleDto ToRoleDto(this RoleWithoutPermissionsDto roleWithoutPermissionsDto)
        {
            return new RoleDto
            {
                Id = roleWithoutPermissionsDto.Id,
                Name = roleWithoutPermissionsDto.Name,
                Description = roleWithoutPermissionsDto.Description
            };
        }
    }
}
