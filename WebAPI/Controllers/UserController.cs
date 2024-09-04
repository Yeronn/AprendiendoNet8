using Application.DTOs;
using Application.Mappers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpGet("getUsers")]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _userRepository.GetAll();
            var usersDto = users.Select(u => u.ToUserDto());
            return Ok(usersDto);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.Login(loginDto.Email, loginDto.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
                );
            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            var userDto = user.ToUserDto();
            return Ok(new { message = "Login successful", user = userDto, Token = tokenValue });
        }

        [Authorize]
        [HttpGet("getUser/{id}", Name ="getUser")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
                return NotFound();
            return Ok(user.ToUserDto());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userEntity = newUser.ToUserEntity();

            try
            {
                var createdUser = await _userRepository.Create(userEntity);
                return CreatedAtRoute("getUser", new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message );
            }
        }
    }
}
