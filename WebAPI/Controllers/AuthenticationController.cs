using Application.DTOs.Authentication;
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
            var loginResponse = await _authService.Login(loginDto);

            if(loginResponse.Success)
                return Ok(new { message = loginResponse.Message, loginResponse.AccessToken, loginResponse.RefreshToken});

            else if (loginResponse.IsBadRequest)
                return BadRequest(loginResponse.Message);

            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado");

        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { Mesagge = "Refresh token no proporcionado"});

            var generatedTokens = await _authService.RefreshTokens(refreshToken);

            if (generatedTokens.Success)
                return Ok( new
                {
                    generatedTokens.Message,
                    generatedTokens.Token,
                    generatedTokens.RefreshToken,
                } );

            else if (generatedTokens.IsBadRequest)
                return BadRequest(generatedTokens.Message);
            
            return StatusCode(StatusCodes.Status500InternalServerError, $"Ocurrió un error inesperado: {generatedTokens.Message}");
        }
    }
}
