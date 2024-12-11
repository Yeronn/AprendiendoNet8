namespace Application.DTOs.User
{
    public class UserDto
    {
        public string IdCCNit { get; set; } = string.Empty;
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CCIdentification { get; set; }
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }


}
