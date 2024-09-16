using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static RoleEntity ToRoleEntity(this UpdateRolDto role)
        {
            return new RoleEntity
            {
                Id = role.Id ?? 0,
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
