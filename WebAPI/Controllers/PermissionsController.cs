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
        public async Task<IActionResult> GetPermissions()
        {
            var permissions = await _permissionService.GetPermissionsAsync();
            if (permissions == null)
                return NotFound("No hay permisos creados");
            return Ok(permissions);
        }


        [HttpGet("{id}", Name = "GetPermission")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound($"El permiso con id {id} no se encuentra en el sistema");

            return Ok(permission);
        }


        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] CreateUpdatePermissionDto createPermission)
        {
            var result = await _permissionService.CreatePermissionAsync(createPermission);
            
            if (result.Success)
                return CreatedAtRoute("GetPermission", new { id = result.Permission!.Id}, result.Permission);
            else if (result.IsConflict)
                return Conflict(result.Message);
            else
                return BadRequest(result.Message);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] CreateUpdatePermissionDto permissionDto)
        {
            var updatedPermission = await _permissionService.UpdatePermissionAsync(id, permissionDto);
            
            if (updatedPermission.Success)
                return Ok(new
                {
                    updatedPermission.Success,
                    updatedPermission.Message,
                    updatedPermission.Permission
                });
            else if (updatedPermission.IsConflict)
                return Conflict(updatedPermission.Message);
            else if (updatedPermission.IsNotFound)
                return NotFound(updatedPermission.Message);
            else
                return BadRequest(updatedPermission.Message);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var result = await _permissionService.DeletePermissionAsync(id);
            if (result.Success)
                return NoContent();
            else if (result.IsConflict) 
                return Conflict(result.Message);
            else if (result.IsNotFound) 
                return NotFound(result.Message);
            return BadRequest(result.Message);
        }


        [HttpGet("permissionsByRol/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRoleId(int roleId)
        {
            var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            if (!permissions.Any())
                return NotFound($"El rol con el id {roleId} no existe o no tiene permisos");
            return Ok(permissions);
        }

        

    }
}
