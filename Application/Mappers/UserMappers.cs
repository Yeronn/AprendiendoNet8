using Application.DTOs.User;
using Domain.Entities;

namespace Application.Mappers
{
    public static class UserMappers
    {
        public static UsersDto ToUsersDto (this UserEntity userEntity)
        {
            return new UsersDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username!,
                Fullname = userEntity.Fullname!,
            };
        }

        public static UserDto ToUserDto(this UserEntity userEntity)
        {
            return new UserDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username!,
                Fullname = userEntity.Fullname!,
            };
        }

        public static UserEntity ToUserEntity(this RegisterUserDto registerUserDto)
        {
            return new UserEntity
            {
                Fullname = registerUserDto.Fullname,
                Username = registerUserDto.Username,
                Password = registerUserDto.Password,
                // Role = registerUserDto.Role!,
            };
        }
    }
}
