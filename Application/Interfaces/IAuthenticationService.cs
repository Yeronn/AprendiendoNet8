using Application.DTOs;
using Application.DTOs.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponse> RegisterUser(RegisterUserDto newUser);
        Task<LoginResponse> Login(string email, string password);
        string GenerateJWTToken(UserEntity user, string jti);
        Task<bool> ValidateToken(string token);
    }
}
