using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
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


        // GET: api/users/by-id/{id}
        [HttpGet("/byId/{id}", Name = "GetUserById") ]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = "Usuario no encontrado." });
            return Ok(user);
        }


        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            if (!users.Any())
                return NotFound("No hay usuarios en el sistema");
            return Ok(users);
        }


        // [Authorize(Roles = "Admin")]
        [HttpPost("{companyNit}")]
        public async Task<IActionResult> CreateUser(int companyNit, [FromBody] RegisterUserDto newUser)
        {
            var createdResponse = await _userService.CreateUserAsync(newUser, companyNit);

            if (createdResponse.Success)
                return CreatedAtRoute("GetUserById", new { id = createdResponse.User!.Id }, createdResponse);
            else if (createdResponse.IsConflict) 
                return Conflict(createdResponse.Message);
            else
                return BadRequest(createdResponse.Message);
        }


        // PUT: api/users/{IdCCNit}
        [HttpPut("{IdCCNit}")]
        public async Task<IActionResult> UpdateUser(int IdCCNit, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateUserAsync(IdCCNit, updateUserDto);
            if (updatedUser == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(updatedUser);
        }


        // DELETE: api/users/{IdCCNit}
        [HttpDelete("{IdCCNit:int}")]
        public async Task<IActionResult> DeleteUser(int IdCCNit)
        {
            var result = await _userService.DeleteUserAsync(IdCCNit);
            if (!result)
                return NotFound(new { Message = "User not found." });

            return NoContent();
        }


        // GET: api/users/{IdCCNit}
        [HttpGet("{IdCCNit:int}")]
        public async Task<IActionResult> GetUserByIdCCNit(int IdCCNit)
        {
            var user = await _userService.GetUserByIdCCNitAsync(IdCCNit);
            if (user == null)
                return NotFound(new { Message = "User not found." });
            return Ok(user);
        }

    }
}
