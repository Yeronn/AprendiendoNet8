using Application.DTOs.Authentication;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                            ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "Unknown";

            string deviceInfo = Request.Headers["User-Agent"].FirstOrDefault() 
                                ?? "Unknown";

            var loginResponse = await _authService.Login(loginDto, ipAddress, deviceInfo);

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
                return BadRequest(new { Message = "Refresh token no proporcionado" });

            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "Unknown";

            var deviceInfo = Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtRefreshToken;

            try
            {
                jwtRefreshToken = tokenHandler.ReadJwtToken(refreshToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "El token proporcionado es inválido", Error = ex.Message });
            }

            var idCCNit = jwtRefreshToken.Claims.FirstOrDefault(c => c.Type == "IdCCNIT")?.Value;
            if (string.IsNullOrWhiteSpace(idCCNit))
                return BadRequest(new { Message = "El refresh token no tiene el claim 'IdCCNIT'" });

            var generatedTokens = await _authService.RefreshTokens(idCCNit, ipAddress, deviceInfo);

            if (generatedTokens.Success)
            {
                return Ok(new
                {
                    generatedTokens.Message,
                    generatedTokens.Token,
                    generatedTokens.RefreshToken,
                });
            }

            if (generatedTokens.IsBadRequest)
                return BadRequest(generatedTokens.Message);

            return StatusCode(StatusCodes.Status500InternalServerError, $"Ocurrió un error inesperado: {generatedTokens.Message}");
        }
    }
}
