using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class LoginDto
    {
        [Required]
        public string IdCCNit { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
