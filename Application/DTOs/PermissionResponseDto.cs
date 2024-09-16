using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record PermissionResponseDto(bool Success, string Message, PermissionDto? Role = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);
    
}