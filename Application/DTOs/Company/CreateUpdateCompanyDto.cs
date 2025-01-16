using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Company
{
    public class CreateUpdateCompanyDto
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo Name es obligatorio.")]
        public string? Name { get; set; }
        
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo debe contener solo números")]
        [Required(ErrorMessage = "El campo Description es obligatorio.")]
        public int Nit { get; set; }
    }
}