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

        public static PermissionEntity ToEntity(this CreatePermissionDto createPermissionDto)
        {
            return new PermissionEntity
            {
                Name = createPermissionDto.Name,
                Description = createPermissionDto.Description
            };
        }

        public static PermissionEntity ToPermissionEntity(this UpdatePermissionDto updatePermissionDto)
        {
            return new PermissionEntity
            {
                Id = updatePermissionDto.Id ?? 0,
                Name = updatePermissionDto.Name,
                Description = updatePermissionDto.Description
            };
        }

        public static PermissionDto ToPermissionDto(this PermissionEntity entity)
        {
            return new PermissionDto
            {
                Id = entity.Id,
                Name = entity.Name!,
                Description = entity.Description!
            };
        }
    }
}
