using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    public static class PermissionMappers
    {
        public static PermissionEntity ToEntity(this PermissionDto permissionDto)
        {
            return new PermissionEntity
            {
                Id = permissionDto.Id ?? 0,
                Name = permissionDto.Name,
                Description = permissionDto.Description
            };
        }

        public static PermissionDto ToDto(this PermissionEntity entity)
        {
            return new PermissionDto
            {
                Name = entity.Name!,
                Description = entity.Description!
            };
        }
    }
}
