namespace Application.DTOs
{
    public class RoleWithoutPermissionsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public int CompanyId { get; set; }
    }
}
