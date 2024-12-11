namespace Domain.Entities
{
    public class UserEntity
    {
        public required string IdCCNit { get; set; }
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required int CCIdentification { get; set; }
        public required string HashedPassword { get; set; }
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
