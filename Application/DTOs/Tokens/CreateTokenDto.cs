using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Antiforgery;

namespace Application.DTOs.Tokens
{
    public class CreateTokenDto
    {
        public string RecoveryToken { get; set; } = string.Empty;
        public string Jti { get; set; } = string.Empty;
        [Required]
        public int IdCCNit { get; set; }
    }

}