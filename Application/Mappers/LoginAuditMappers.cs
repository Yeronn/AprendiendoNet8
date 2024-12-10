using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.LoginAudit;
using Domain.Entities;

namespace Application.Mappers
{
    public static class LoginAuditMappers
    {
        public static LoginAuditEntity ToEntity(this LoginAuditDto loginAudit)
        {
            return new LoginAuditEntity
            {
                TokenId = loginAudit.TokenId,
                IdCCNit = loginAudit.IdCCNit,
                IssuedAt = loginAudit.IssuedAt,
                ExpiresAt = loginAudit.ExpiresAt,   
                IPAddress = loginAudit.IPAddress,
                DeviceInfo = loginAudit.DeviceInfo
            };
        }
    }
}