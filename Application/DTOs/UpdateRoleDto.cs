using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CreateUpdateRoleDto
    {
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public required string Description { get; set; }
        [Required]
        public List<int> PermissionsIds { get; set; } = [];
    }
}
