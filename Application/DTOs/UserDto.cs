namespace Domain.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        //public int IdentityCard { get; set; }
        //public decimal Salary { get; set; }
    }
}
