using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    public static class UserMappers
    {
        public static UserDto ToUserDto (this UserEntity userEntity)
        {
            return new UserDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username,
                Fullname = userEntity.Fullname,
                Email = userEntity.Email,
                //IdentityCard = userEntity.identityCard,
                //Salary = userEntity.salary
            };
        }

        public static UserEntity ToUserEntity(this RegisterUserDto registerUserDto)
        {
            return new UserEntity
            {
                Fullname = registerUserDto.Fullname,
                Email = registerUserDto.Email,
                IdentityCard = registerUserDto.IdentityCard,
                Username = registerUserDto.Username,
                Password = registerUserDto.Password,
                Salary = 0.0m
            };
        }
    }
}
