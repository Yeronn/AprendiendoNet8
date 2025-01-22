namespace Application.DTOs.User
{
    public class UserJwtTokenDto
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = "Unknown";
    }
}