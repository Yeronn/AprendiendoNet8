using Application.DTOs;
using Application.DTOs.Role;
using Domain.Entities;

namespace Application.Mappers
{
    public static class RoleMappers
    {
        public static RoleEntity ToEntity(this CreateRoleDto createRoleDto)
        {
            return new RoleEntity
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                Status = createRoleDto.Status,
                CompanyId = createRoleDto.CompanyId,
                Permissions = []
            };
        }


        public static RoleEntity ToEntity(this UpdateRoleDto createRoleDto)
        {
            return new RoleEntity
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                Status = createRoleDto.Status,
                Permissions = []
            };
        }


        public static RoleEntity ToEntity(this RoleWithoutPermissionsDto role)
        {
            return new RoleEntity
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Status = role.Status,
                CompanyId = role.CompanyId,
                Permissions = []
            };
        }


        public static RoleWithoutPermissionsDto ToRoleWithoutPermissionsDto(this RoleEntity roleEntity)
        {
            return new RoleWithoutPermissionsDto
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name,
                Description = roleEntity.Description,
                Status = roleEntity.Status,
                CompanyId = roleEntity.CompanyId,
            };
        }


        public static RoleDto ToDto(this RoleEntity roleEntity)
        {
            return new RoleDto
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name,
                Description = roleEntity.Description,
                Status = roleEntity.Status,
                CompanyId = roleEntity.CompanyId,
                Permissions = roleEntity.Permissions.Select(p => p.ToDto()).ToList()
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
