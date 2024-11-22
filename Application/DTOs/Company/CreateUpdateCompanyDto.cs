using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Company
{
    public class CreateUpdateCompanyDto
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El campo Name es obligatorio.")]
        public string? Name { get; set; }
        
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo debe contener solo números")]
        [Required(ErrorMessage = "El campo Description es obligatorio.")]
        public int? NIT { get; set; }
    }
}