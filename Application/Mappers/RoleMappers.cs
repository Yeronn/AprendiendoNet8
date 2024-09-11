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
        public static RoleEntity ToRoleEntity(this RoleWithoutPermissionsDto role)
        {
            return new RoleEntity
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }

        public static RoleWithoutPermissionsDto ToRoleWithoutPermissions(this RoleEntity roleEntity)
        {
            return new RoleWithoutPermissionsDto
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name,
                Description = roleEntity.Description
            };
        }
    }
}
