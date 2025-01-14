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

            if (loginResponse.IsBadRequest)
                return BadRequest(loginResponse.Message);

            return Ok(new { message = loginResponse.Message, loginResponse.Token, loginResponse.RefreshTokenToken});
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { Mesagge = "Refresh token no proporcionado"});

            var generatedTokens = await _authService.RefreshTokens(refreshToken);
            if (!generatedTokens.Success)
                return Unauthorized(new { generatedTokens.Message });

            return Ok( new
            {
                generatedTokens.Message,
                generatedTokens.Token,
                generatedTokens.RefreshToken,
            } );
        }


    }
}
