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
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto createPermission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }

            var result = await _permissionService.CreatePermissionAsync(createPermission);
            
            if (result.Success)
                return Ok(new
                {
                    result.Success,
                    result.Message,
                    result.Permission
                });
            else if (result.IsConflict)
                return Conflict(result.Message);
            else
                return BadRequest(result.Message);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionDto permissionDto)
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
            var permissions = await _permissionService.GetAllPermissionsByRoleIdAsync(roleId);
            if (permissions == null)
                return NotFound($"El rol con el id {roleId} no existe");
            return Ok(permissions);
        }


        [HttpPost("get-permissions-by-roles")]
        public async Task<IActionResult> GetPermissionsByRoles([FromBody] List<int> roleIds)
        {
            if (roleIds == null || roleIds.Count == 0)
                return BadRequest("La lista de IDs de roles no puede estar vacía.");

            var permissions = await _permissionService.GetUniquePermissionsByRoleIdsAsync(roleIds);

            if (permissions == null || !permissions.Any())
                return NotFound("No se encontraron permisos para los roles proporcionados.");

            return Ok(permissions);
        }

    }
}
