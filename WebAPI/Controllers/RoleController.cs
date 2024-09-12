﻿using Application.DTOs;
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
        public async Task<IActionResult> Create([FromBody] CreateRolDto createRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }
            var result = await _roleService.CreateRoleAsync(createRole);
            if (result.Success)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRolDto updateRole)
        {
            var result = await _roleService.UpdateRoleAsync(id, updateRole);
            if (result.Success)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (result.Success)
                return Ok(result);
            return NotFound(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound("Rol no encontrado.");
            return Ok(role);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("rolesAndPermissions")]
        public async Task<IActionResult> GetAllRolesWithTheirPermissions()
        {
            var roles = await _roleService.GetAllRolesWithTheirPermissionsAsync();
            return Ok(roles);
        }

        //TODO: Hacer un controlador que obtenga un rol con sus permisos por id
    }
}
