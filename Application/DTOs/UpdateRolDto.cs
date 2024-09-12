using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UpdateRolDto
    {
        public int? Id { get; set; }
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
