using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class PermissionDto
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo 'Name' es obligatorio.")]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
