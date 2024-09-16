using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UpdatePermissionDto
    {
        public int? Id { get; set; }
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}