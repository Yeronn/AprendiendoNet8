namespace Domain.Entities
{
    public class UserEntity
    {
        public int IdCardNit { get; set; }
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Identification { get; set; }
        public required string Password { get; set; }
        public required string PasswordSalt { get; set; }
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? LastJti { get; set; }
    }
}
