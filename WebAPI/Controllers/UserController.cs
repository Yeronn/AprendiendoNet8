using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            if (users == null)
                return NotFound("No users found.");
            return Ok(users);
        }


        [Authorize(Roles = "User, Admin")]
        [HttpGet("getUser/{id}", Name ="getUser")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] UpdateUserDto updateUser)
        {
            var result = await _userService.UpdateUserAsync(id, updateUser);
            if (result.Success)
                return Ok(new
                {
                    result.Success,
                    result.Message,
                    result.User
                });
            else if (result.IsConflict)
                return Conflict(result.Message);
            else if (result.IsNotFound)
                return NotFound(result.Message);
            else
                return BadRequest(result.Message);
        }


        [HttpGet("getUserWithRoles/{userId}")]
        public async Task<IActionResult> GetUserWithRoles(int userId)
        {
            var user = await _userService.GetUserWithRolesByUserIdAsync(userId);
            if (user == null)
                return NotFound($"El usuario con Id {userId} no existe");
            return Ok(user);
        }


        [HttpGet("getUserWithRoles")]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            var users = await _userService.GetAllUsersWithRolesAsync();
            if (users == null)
                return NotFound($"No hay usuarios en el sistema");
            return Ok(users);
        }


        [HttpPost("{userId}/addRolesToUser")]
        public async Task<IActionResult> AddRolesToUser(int userId, [FromBody] List<int> roleIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos.");
            }

            var result = await _userService.AssignRolesToUserAsync(userId, roleIds);

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
    }
}
