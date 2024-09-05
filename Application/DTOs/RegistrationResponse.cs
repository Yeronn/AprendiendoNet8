using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record RegistrationResponse(string Message = null!, int? Id = null, bool? internalServerError = false);
}
