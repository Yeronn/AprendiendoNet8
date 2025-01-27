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
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Email = registerUserDto.Email,
                CCNumber = registerUserDto.CCNumber,
                HashedPassword = registerUserDto.Password,
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
                CCNumber = updateUserDto.CCNumber,
                HashedPassword = updateUserDto.Password,
                RoleId = updateUserDto.RoleId
            };
        }


        public static UserDto ToUserDto(this UserEntity userEntity)
        {
            return new UserDto
            {
                Id = userEntity.Id,
                FirstName = userEntity.FirstName!,
                LastName = userEntity.LastName!,
                Email = userEntity.Email!,
                CCNumber = userEntity.CCNumber!,
                CompanyId = userEntity.CompanyId,
                RoleId = userEntity.RoleId,
                RegistrationDate = userEntity.RegistrationDate
            };
        }


        public static UserJwtTokenDto ToUserJwtTokenDto(this UserDto userDto)
        {
            return new UserJwtTokenDto
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                RoleId = userDto.RoleId,
                CompanyId= userDto.CompanyId,
                UserId = userDto.Id
            };
        }
        
    }


}
