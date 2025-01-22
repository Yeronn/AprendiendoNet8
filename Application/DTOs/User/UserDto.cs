namespace Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CCNumber { get; set; }
        public int CompanyId { get; set; }
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }


}
