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
            var permissions = await _permissionService.GetAll();
            return Ok(permissions);
        }

        [HttpGet("{id}", Name = "GetPermissionById")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _permissionService.GetById(id);
            if (permission == null)
                return NotFound($"El usuario con id {id} no se encuentra en el sistema");

            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDto permissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }

            var newPermission = await _permissionService.Create(permissionDto);
            return CreatedAtAction(nameof(GetPermissionById), new { id = newPermission.Id }, newPermission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] PermissionDto permissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }
            var updatedPermission = await _permissionService.Update(id, permissionDto);
            if (updatedPermission.Name == null)
                return NotFound(updatedPermission.Message);

            return Ok(new { message = updatedPermission.Message, permmisionName = updatedPermission.Name } );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var deleted = await _permissionService.Delete(id);
            if (!deleted)
                return NotFound($"El permiso con id {id} no se encuentra en el sistema");

            return NoContent();
        }
    }
}
