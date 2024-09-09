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
        public async Task<ActionResult<IEnumerable<UsersDto>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAll();
                if (users == null || !users.Any())
                {
                    return NotFound("No users found.");
                }
                return Ok(users);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, $"Ha ocurrido un error obteniendo los usuarios: {ex.Message}");
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("getUser/{id}", Name ="getUser")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
    }
}
