using Application.DTOs;
using Application.DTOs.User;
using Domain.Entities;

namespace Application.Mappers
{
    public static class UserMappers
    {
        public static UserEntity ToUserEntity(this RegisterUserDto registerUserDto)
        {
            return new UserEntity
            {
                IdCardNit = registerUserDto.CompanyNit,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Email = registerUserDto.Email,
                Identification = registerUserDto.Identification,
                Password = registerUserDto.Password,
                PasswordSalt = registerUserDto.PasswordSalt,
                RoleId = registerUserDto.RoleId,
                RegistrationDate = DateTime.UtcNow 
            };
        }


        public static UserEntity ToUserEntity(this UpdateUserDto updateUserDto)
        {
            return new UserEntity
            {
                FirstName = updateUserDto.FirstName,
                LastName = updateUserDto.LastName,
                Email = updateUserDto.Email,
                Identification = updateUserDto.Identification,
                Password = updateUserDto.Password,
                PasswordSalt = updateUserDto.PasswordSalt,
                RoleId = updateUserDto.RoleId
            };
        }


        public static UserDto ToUserDto(this UserEntity userEntity)
        {
            return new UserDto
            {
                IdCardNit = userEntity.IdCardNit,
                Id = userEntity.Id,
                FirstName = userEntity.FirstName!,
                LastName = userEntity.LastName!,
                Email = userEntity.Email!,
                Identification = userEntity.Identification!,
                RoleId = userEntity.RoleId,
                RegistrationDate = userEntity.RegistrationDate
            };
        }


        public static UserJwtTokenDto ToUserJwtTokenDto(this UserDto userDto)
        {
            return new UserJwtTokenDto
            {
                IdCardNit = userDto.IdCardNit,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                RoleId = userDto.RoleId
            };
        }
        
    }


}
