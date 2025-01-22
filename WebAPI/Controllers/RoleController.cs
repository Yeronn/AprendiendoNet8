using Application.Authorization;
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


        [Authorize(Policy = PermissionPolicy.Read)]
        [HttpGet("{roleId}", Name = "GetRole")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var role = await _roleService.GetRoleByIdAsync(roleId, int.Parse(companyId));

            if (role.Success)
                return Ok(role.Role);
            else if (role.IsNotFound)
                return NotFound(role.Message);
            else
                return BadRequest(role.Message);
        }


        [Authorize(Policy = PermissionPolicy.Read)]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var roles = await _roleService.GetRolesAsync(int.Parse(companyId));

            if (!roles.Any())
                return NotFound("No hay roles en el sistema");

            return Ok(roles);
        }


        [Authorize(Policy = "Write")]
        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] CreateRoleDto createRole)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            createRole.CompanyId = int.Parse(companyId);
            var result = await _roleService.CreateRoleAsync(createRole);

            if (result.Success)
                return CreatedAtRoute("GetRole", new { roleId = result.Role!.Id!}, result.Role);
            else if (result.IsConflict)
                return Conflict(result.Message);
            else
                return BadRequest(result.Message);
        }


        [Authorize(Policy = PermissionPolicy.Write)]
        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRol(int roleId, [FromBody] UpdateRoleDto updateRoleDto)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var result = await _roleService.UpdateRoleAsync(roleId, updateRoleDto, int.Parse(companyId));
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


        [Authorize(Policy = PermissionPolicy.Delete)]
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRol(int roleId)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var result = await _roleService.DeleteRoleAsync(roleId, int.Parse(companyId));
            if (result.Success)
                return StatusCode(StatusCodes.Status204NoContent, "Se eliminó el rol");

            else if (result.IsNotFound) 
                return NotFound(result.Message);

            return BadRequest(result.Message);
        }


        [HttpGet("rolesByPermission/{permissionId}")]
        public async Task<IActionResult> GetRolesByPermissionId(int permissionId)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var roles = await _roleService.GetRolesByPermissionIdAsync(permissionId, int.Parse(companyId));
            if (roles == null)
                return NotFound($"El permiso con el id {permissionId} no existe");
            return Ok(roles);
        }


    }
}
