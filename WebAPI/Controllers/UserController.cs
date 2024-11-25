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


        // GET: api/users/{idCardNit}
        [HttpGet("{idCardNit:int}")]
        public async Task<IActionResult> GetUserByIdCardNit(int idCardNit)
        {
            var user = await _userService.GetUserByIdCardNitAsync(idCardNit);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }
            return Ok(user);
        }

        // GET: api/users/by-id/{id}
        [HttpGet("by-id/{id:int}", Name = "GetUserById") ]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }
            return Ok(user);
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.CreateUserAsync(registerUserDto);
            return CreatedAtAction(nameof(GetUserByIdCardNit), new { idCardNit = createdUser!.IdCardNit }, createdUser);
        }

        // PUT: api/users/{idCardNit}
        [HttpPut("{idCardNit:int}")]
        public async Task<IActionResult> UpdateUser(int idCardNit, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateUserAsync(idCardNit, updateUserDto);
            if (updatedUser == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(updatedUser);
        }

        // DELETE: api/users/{idCardNit}
        [HttpDelete("{idCardNit:int}")]
        public async Task<IActionResult> DeleteUser(int idCardNit)
        {
            var result = await _userService.DeleteUserAsync(idCardNit);
            if (!result)
            {
                return NotFound(new { Message = "User not found." });
            }

            return NoContent();
        }

    }
}
