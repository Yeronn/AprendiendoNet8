using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class RegisterUserDto
    {
        public int IdCardNit { get; set; }  // Clave primaria proporcionada por el usuario
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

}
