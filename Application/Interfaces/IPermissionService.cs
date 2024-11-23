using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>?> GetAllPermissionsAsync();
        Task<PermissionDto?> GetPermissionByIdAsync(int id);
        Task<PermissionResponseDto> CreatePermissionAsync(CreateUpdatePermissionDto createPermissionDto);
        Task<PermissionResponseDto> UpdatePermissionAsync(int id, CreateUpdatePermissionDto updatePermissionDto);
        Task<PermissionResponseDto> DeletePermissionAsync(int id);
        Task<IEnumerable<PermissionDto>> GetAllPermissionsByRoleIdAsync(int roleId);
        Task<PermissionResponseDto> ValidatePermissionExistsByIdAsync(int id);
        Task<PermissionResponseDto> ValidatePermissionsExistAsync(List<int> permissionIds);
    }
}
