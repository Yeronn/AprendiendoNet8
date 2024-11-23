using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateUpdatePermissionDto
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}