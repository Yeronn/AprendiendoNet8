namespace Application.DTOs
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CCNumber { get; set; } 
        public int CompanyId { get; set; }
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

}