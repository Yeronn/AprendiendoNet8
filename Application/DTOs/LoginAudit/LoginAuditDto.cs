using System.ComponentModel.DataAnnotations;
using Application.Validators.Attributes;

namespace Application.DTOs.LoginAudit
{
    public class LoginAuditDto
    {
        [Required(ErrorMessage = "El TokenId es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El TokenId no puede exceder los 50 caracteres.")]
        public string TokenId { get; set; } = null!;

        [Required(ErrorMessage = "El IdCCNit es obligatorio.")]
        public string IdCCNit { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        public DateTime IssuedAt { get; set; }

        [Required(ErrorMessage = "La fecha de expiración es obligatoria.")]
        [CompareDates(nameof(IssuedAt), ErrorMessage = "La fecha de expiración debe ser posterior a la fecha de emisión.")]
        public DateTime ExpiresAt { get; set; }

        [Required(ErrorMessage = "La dirección IP es obligatoria.")]
        [MaxLength(45, ErrorMessage = "La dirección IP no puede exceder los 45 caracteres.")]
        public string? IPAddress { get; set; }

        [Required(ErrorMessage = "La información del dispositivo es obligatoria.")]
        [MaxLength(255, ErrorMessage = "La información del dispositivo no puede exceder los 255 caracteres.")]
        public string? DeviceInfo { get; set; }

        public bool Status { get; set; } = true;
        public bool IsAccessToken { get; set; } = true;
    }
}