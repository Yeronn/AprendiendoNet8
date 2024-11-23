namespace Domain.Entities
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public int CompanyId { get; set; }
        public List<PermissionEntity> Permissions { get; set; } = [];
    }
}
