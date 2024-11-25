namespace Application.DTOs
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }  // Se mantiene la fecha original
    }

}