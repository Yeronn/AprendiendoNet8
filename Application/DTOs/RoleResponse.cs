﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record RoleResponse(bool Success, string Message, RoleWithoutPermissionsDto? Role = null);

}
