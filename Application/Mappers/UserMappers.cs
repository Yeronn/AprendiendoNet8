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
                Username = userEntity.Username,
                Fullname = userEntity.Fullname,
                Email = userEntity.Email,
                //IdentityCard = userEntity.IdentityCard,
                //Salary = userEntity.Salary,

            };
        }

        public static UserDto ToUserDto(this UserEntity userEntity)
        {
            return new UserDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username!,
                Fullname = userEntity.Fullname!,
                Email = userEntity.Email!,
                IdentityCard = userEntity.IdentityCard,
                Salary = userEntity.Salary,
                Role = userEntity.Role!
            };
        }

        public static UserEntity ToUserEntity(this RegisterUserDto registerUserDto)
        {
            return new UserEntity
            {
                Fullname = registerUserDto.Fullname,
                Email = registerUserDto.Email,
                IdentityCard = registerUserDto.IdentityCard ?? 0,
                Username = registerUserDto.Username,
                Password = registerUserDto.Password,
                Role = registerUserDto.Role!,
                Salary = 0.0m
            };
        }
    }
}
