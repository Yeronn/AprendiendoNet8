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


        // [HttpPost("refresh-token")]
        // public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var jwtToken = tokenHandler.ReadJwtToken(refreshToken);

        //     if (jwtToken.ValidTo < DateTime.UtcNow)
        //         return Unauthorized("Refresh token has expired");

        //     // Aquí generas un nuevo Access Token
        //     var user = ExtractUserFromToken(jwtToken); // Implementa esto según tu lógica
        //     var newAccessToken = await _authenticationService.GenerateJWTToken(user, true);

        //     return Ok(new { AccessToken = newAccessToken });
        // }


    }
}
