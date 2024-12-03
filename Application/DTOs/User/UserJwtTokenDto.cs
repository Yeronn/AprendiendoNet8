namespace Application.DTOs.User
{
    public class UserJwtTokenDto
    {
        public int IdCCNit { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}