namespace Application.DTOs.User
{
    public class UserJwtTokenDto
    {
        public string IdCCNit { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}