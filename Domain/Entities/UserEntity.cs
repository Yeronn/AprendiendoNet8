namespace Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public string? LastJti { get; set; }
        public int RoleId { get; set; }
        public List<RoleEntity> Roles { get; set; } = [];
    }
}
