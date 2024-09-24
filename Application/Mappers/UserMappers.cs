using Application.DTOs;
using Application.DTOs.User;
using Domain.Entities;

namespace Application.Mappers
{
    public static class UserMappers
    {
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

        public static UserEntity ToUserEntity(this UpdateUserDto updateUserDto)
        {
            return new UserEntity
            {
                Fullname = updateUserDto.Fullname,
                Username = updateUserDto.Username,
                Password = updateUserDto.Password,
                // Role = registerUserDto.Role!,
            };
        }

        public static UserWithoutRolesDto ToUserWithoutRolesDto(this UserEntity userEntity)
        {
            return new UserWithoutRolesDto
            {
                Id = userEntity.Id,
                Fullname = userEntity.Fullname,
                Username = userEntity.Username,
            };
        }
    }
}
