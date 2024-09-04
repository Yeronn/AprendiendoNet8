using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserEntity?> GetById(int id);
        Task<UserEntity?> Login(string email, string password);
        Task<UserEntity?> Create(RegisterUserDto newUser);
        string GenerateJWTToken(UserEntity user);
    }
}
