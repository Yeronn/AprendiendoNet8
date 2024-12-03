using Application.DTOs.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }

            var loginResponse = await _authService.Login(loginDto);

            if (loginResponse.IsNotFound)
                return NotFound(new { message = loginResponse.Message });
            else if (loginResponse.IsBadRequest)
                return BadRequest(loginResponse.Message);

            return Ok(new { message = loginResponse.Message, Token = loginResponse.token });
        }

    }
}
