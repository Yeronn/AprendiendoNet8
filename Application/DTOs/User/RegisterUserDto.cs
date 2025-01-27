using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class RegisterUserDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public int CCNumber { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public int RoleId { get; set; }
    }

}
