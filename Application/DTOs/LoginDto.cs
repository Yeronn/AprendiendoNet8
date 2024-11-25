using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class LoginDto
    {
        [Required]
        public int IdCardNit { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
