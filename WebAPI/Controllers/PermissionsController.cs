using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            if (permissions == null)
                return NotFound("No hay permisos creados");
            return Ok(permissions);
        }

        [HttpGet("{id}", Name = "GetPermissionById")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound($"El permiso con id {id} no se encuentra en el sistema");

            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDto permissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }

            var newPermission = await _permissionService.CreatePermissionAsync(permissionDto);
            if (newPermission.Id == null)
                return BadRequest(newPermission.Message);
            return CreatedAtAction(nameof(GetPermissionById), new { id = newPermission.Id }, newPermission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] PermissionDto permissionDto)
        {
            if (!ModelState.IsValid) //TODO: Investigar porque cuando se hace una peticion con datos faltantes no entra al controlador si no que de una da error
            {
                var errorMessages = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage);

                var fullErrorMessage = "Datos inválidos: " + string.Join("; ", errorMessages);

                return BadRequest(fullErrorMessage);
            }
            var updatedPermission = await _permissionService.UpdatePermissionAsync(id, permissionDto);
            if (updatedPermission.Name == null)
                return NotFound(updatedPermission.Message);

            return Ok(new { message = updatedPermission.Message, permmisionName = updatedPermission.Name } );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var deleted = await _permissionService.DeletePermissionAsync(id);
            if (!deleted)
                return NotFound($"El permiso con id {id} no se encuentra en el sistema");

            return NoContent();
        }

        [HttpGet("permissionsByRol/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRoleId(int roleId)
        {
            var permissions = await _permissionService.GetAllPermissionsByRoleIdAsync(roleId);
            if (permissions == null)
                return NotFound($"El rol con el id {roleId} no existe");
            return Ok(permissions);
        }
    }
}
