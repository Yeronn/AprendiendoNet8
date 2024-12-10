using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LoginAuditEntity
    {
        public string TokenId { get; set; } = null!;
        public int IdCCNit { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public bool Status { get; set; } = true;
    }
}