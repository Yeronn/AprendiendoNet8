using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string? Username { get; set; } 
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public int IdentityCard { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
        public string? LastJti { get; set; }
    }
}
