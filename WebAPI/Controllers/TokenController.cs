using Application.DTOs.Tokens;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokensController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] CreateTokenDto tokenDto)
        {
            var createdId = await _tokenService.CreateTokenAsync(tokenDto);
            return CreatedAtAction(nameof(GetTokenById), new { id = createdId }, tokenDto);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTokenById(int id)
        {
            var token = await _tokenService.GetTokenByIdAsync(id);
            if (token == null)
                return NotFound();

            return Ok(token);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTokens()
        {
            var tokens = await _tokenService.GetAllTokensAsync();
            return Ok(tokens);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToken(int id)
        {
            var success = await _tokenService.DeleteTokenAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}