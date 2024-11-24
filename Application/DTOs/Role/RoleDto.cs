using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RoleDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public int CompanyId { get; set; }
        public List<PermissionDto> Permissions { get; set; } = [];
    }
}
