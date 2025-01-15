using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication
{
    public class LoginDto
    {
        [Required(ErrorMessage = "La cédula y el NIT son obligatorios")]
        public string IdCCNit { get; set; } = string.Empty;
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
