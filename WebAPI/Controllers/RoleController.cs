using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        [HttpGet("getRoleAndPermissions/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound("El rol no existe en el sistema");
            return Ok(role);
        }


        [HttpGet("getRolesAndPermissions")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetRolesAsync();
            if (!roles.Any())
                return NotFound("No hay roles en el sistema");
            return Ok(roles);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] CreateUpdateRoleDto createRole)
        {
            var result = await _roleService.CreateRoleAsync(createRole);

            if (result.Success)
                return Ok(new
                {
                    result.Success,
                    result.Message,
                    result.Role
                });
            else if (result.IsConflict)
                return Conflict(result.Message);
            else
                return BadRequest(result.Message);
            
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] CreateUpdateRoleDto updateRoleDto)
        {
            var result = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            if (result.Success)
                return Ok(new
                {
                    result.Success,
                    result.Message,
                    result.Role
                });
            else if (result.IsConflict)
                return Conflict(result.Message);
            else if (result.IsNotFound)
                return NotFound(result.Message);
            else
                return BadRequest(result.Message);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (result.Success)
                return NoContent();
            else if (result.IsNotFound) 
                return NotFound(result.Message);
            return BadRequest(result.Message);
        }


        [HttpPost("{roleId}/addPermissionsToRole")]
        public async Task<IActionResult> AddPermissionsToRole(int roleId, [FromBody] List<int> permissionIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos.");
            }

            var result = await _roleService.AssignPermissionsToRoleAsync(roleId, permissionIds);

            if (result.Success)
                return Ok(new
                {
                    result.Success,
                    result.Message,
                });
            else if (result.IsNotFound)
                return NotFound(result.Message);
            else if (result.IsBadRequest)
                return BadRequest(result.Message);

            return StatusCode(500, result.Message);
        }


        [HttpDelete("{roleId}/removePermissionsFromRole")]
        public async Task<IActionResult> RemovePermissionsFromRole(int roleId, [FromBody] List<int> permissionIds)
        {
            var result = await _roleService.RemovePermissionsFromRoleAsync(roleId, permissionIds);

            if (result.Success)
                return Ok(new
                {
                    result.Success,
                    result.Message,
                });
            else if (result.IsNotFound)
                return NotFound(result.Message);
            else if (result.IsBadRequest)
                return BadRequest(result.Message);

            return StatusCode(500, result.Message);
        }


        [HttpGet("rolesByPermission/{permissionId}")]
        public async Task<IActionResult> GetRolesByPermissionId(int permissionId)
        {
            var roles = await _roleService.GetAllRolesWithoutPermissionsByPermissionIdAsync(permissionId);
            if (roles == null)
                return NotFound($"El permiso con el id {permissionId} no existe");
            return Ok(roles);
        }
    }
}
