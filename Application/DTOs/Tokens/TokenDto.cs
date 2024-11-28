namespace Application.DTOs.Tokens
{
    public class TokenDto
    {
        public int Id { get; set; }
        public string? RecoveryToken { get; set; }
        public string? Jti { get; set; }
        public int IdCCNit { get; set; }
        public DateTime DateCreated { get; set; }
    }
}