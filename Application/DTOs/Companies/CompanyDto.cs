using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Companies
{
    public class CompanyDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? NIT { get; set; } = string.Empty;
    }
}