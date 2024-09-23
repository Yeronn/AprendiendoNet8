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


        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] CreateRolDto createRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }
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
        public async Task<IActionResult> UpdateRol(int id, [FromBody] UpdateRolDto updateRole)
        {
            var result = await _roleService.UpdateRoleAsync(id, updateRole);
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


        [HttpGet("show/{id}")]
        public async Task<IActionResult> GetRolById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound("Rol no encontrado.");
            return Ok(role);
        }


        [HttpGet("show")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            if (roles == null)
                return NotFound("No hay roles en el sistema");
            return Ok(roles);
        }


        [HttpGet("showRoleAndPermissions/{id}")]
        public async Task<IActionResult> GetRoleWithTheirPermissions(int id)
        {
            var role = await _roleService.GetRoleWithPermissionsByRolIdAsync(id);
            if (role == null)
                return NotFound("El rol no existe en el sistema");
            return Ok(role);
        }


        [HttpGet("showRolesAndPermissions")]
        public async Task<IActionResult> GetAllRolesWithTheirPermissions()
        {
            var roles = await _roleService.GetAllRolesWithPermissionsAsync();
            if (roles == null)
                return NotFound("No hay roles en el sistema");
            return Ok(roles);
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
            var roles = await _roleService.GetAllRolesByPermissionIdAsync(permissionId);
            if (roles == null)
                return NotFound($"El rol con el id {permissionId} no existe");
            return Ok(roles);
        }


        [HttpGet("rolesByUser/{userId}")]
        public async Task<IActionResult> GetRolesByUserId(int userId)
        {
            var permissions = await _roleService.GetAllRolesByUserIdAsync(userId);
            if (permissions == null)
                return NotFound($"El user con el id {userId} no existe");
            return Ok(permissions);
        }
    }
}
