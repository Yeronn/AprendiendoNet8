using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LoginAuditEntity
    {
        public required string TokenId { get; set; }
        public required string IdCCNit { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public int TokenStatusId { get; set; }
        public int TokenTypeId { get; set; }
    }
}