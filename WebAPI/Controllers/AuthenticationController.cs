using Application.DTOs.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

            var loginResponse = await _authService.Login(loginDto.Email, loginDto.Password);

            if (loginResponse.token == null)
            {
                return Unauthorized(new { message = loginResponse.Message });
            }

            return Ok(new { message = loginResponse.Message, Token = loginResponse.token });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos: " + ModelState);
            }

            try
            {
                var registrationResponse = await _authService.RegisterUser(newUser);

                if (registrationResponse.Id == null)
                    return BadRequest(registrationResponse.Message);

                return CreatedAtRoute("getUser", new { id = registrationResponse.Id }, registrationResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrio un error no esperado al crear usuario: {ex.Message}");
            }
        }
    }
}
