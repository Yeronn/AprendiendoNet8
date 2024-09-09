using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class RegisterUserDto
    {
        [Required]
        public string Fullname { get; set; } = string.Empty;
        [Required]
        public int? IdentityCard { get; set; }
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression("^(Admin|User)$", ErrorMessage = "El rol debe ser 'Admin' o 'User'.")]
        public string Role { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required, Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}
