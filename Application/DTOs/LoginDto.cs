using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class LoginDto
    {
        [Required]
        public int IdCCNit { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
