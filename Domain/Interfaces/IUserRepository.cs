using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetAll();
        Task<UserEntity?> GetById(int id);
        Task<UserEntity?> GetByEmail(string email);
        Task<UserEntity?> Login(string email, string password);
        Task<UserEntity?> Create(UserEntity newUser);
    }
}
