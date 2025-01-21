using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication
{
    public class LoginDto
    {
        [Required(ErrorMessage = "La cédula es obligatorios")]
        public int CcNumber { get; set; }
        [Required(ErrorMessage = "El NIT es obligatorios")]
        public int Nit { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
