using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CreateRoleDto
    {
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo Name es obligatorio.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "El campo Description es obligatorio.")]
        public string? Description { get; set; }
        [Required]
        public bool Status { get; set; } = true;
        [Required(ErrorMessage = "El campo CompanyId es obligatorio.")]
        public int CompanyId { get; set; }
        [Required]
        public List<int> PermissionsIds { get; set; } = [];
    }
}
