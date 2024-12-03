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
        Task<LoginResponse> Login(LoginDto login);
        Task<string> GenerateJWTToken(UserJwtTokenDto user, string jti);
        Task<bool> ValidateToken(string token);
    }
}
