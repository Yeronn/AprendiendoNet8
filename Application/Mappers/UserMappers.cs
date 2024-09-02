using Domain.DTOs;
using Domain.Entities;

namespace Infrastructure.Mappers
{
    public static class UserMappers
    {
        public static UserDto ToUserDto(this UserEntity userEntity)
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
    }
}
