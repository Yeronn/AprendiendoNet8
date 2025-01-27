namespace Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public int CCNumber { get; set; }
        public int CompanyId { get; set; }
        public required string HashedPassword { get; set; }
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
