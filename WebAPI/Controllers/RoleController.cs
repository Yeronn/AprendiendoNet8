using Application.DTOs;
using Application.DTOs.Role;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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


        [Authorize(Policy = "prueba")]
        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound("El rol no existe");
            return Ok(role);
        }


        [Authorize(Policy = "Read")]
        [HttpGet("RoleByCompany/{companyId}")]
        public async Task<IActionResult> GetAllRoles(int companyId)
        {
            var roles = await _roleService.GetRolesAsync(companyId);
            if (!roles.Any())
                return NotFound("No hay roles en el sistema");
            return Ok(roles);
        }


        [Authorize(Policy = "Write")]
        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] CreateRoleDto createRole)
        {
            var result = await _roleService.CreateRoleAsync(createRole);

            if (result.Success)
                return CreatedAtRoute("GetRole", new { id = result.Role!.Id}, result.Role);
            else if (result.IsConflict)
                return Conflict(result.Message);
            else
                return BadRequest(result.Message);
        }


        [Authorize(Policy = "Write")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] UpdateRoleDto updateRoleDto)
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


        [Authorize(Policy = "Delete")]
        [HttpDelete("{id}")]
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
