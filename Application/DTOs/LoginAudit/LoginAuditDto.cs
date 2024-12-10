namespace Application.DTOs.LoginAudit
{
    public class LoginAuditDto
    {
        public string TokenId { get; set; } = null!;
        public int IdCCNit { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }
    }
}