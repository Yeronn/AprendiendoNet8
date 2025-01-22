using Application.Authorization;
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


        [HttpGet("/byId/{userId}", Name = "GetUserById") ]
        public async Task<IActionResult> GetUserById(int userId)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var user = await _userService.GetUserByIdAndCompanyIdAsync(userId, int.Parse(companyId));
            if (user == null)
                return NotFound(new { Message = "Usuario no encontrado." });
            return Ok(user);
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var users = await _userService.GetUsersByCompanyIdAsync(int.Parse(companyId));
            if (!users.Any())
                return NotFound("No hay usuarios en el sistema");
            return Ok(users);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto newUser)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            newUser.CompanyId = int.Parse(companyId);

            var createdResponse = await _userService.CreateUserAsync(newUser);

            if (createdResponse.Success)
                return CreatedAtRoute("GetUserById", new { userId = createdResponse.User!.Id }, createdResponse.User);
            else if (createdResponse.IsConflict) 
                return Conflict(createdResponse.Message);
            else
                return BadRequest(createdResponse.Message);
        }


        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
            string companyClaimName = UserClaims.CompanyId.ToString();
            var companyId = HttpContext.User.FindFirst(companyClaimName)?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return BadRequest(new { Message = $"El access token no tiene el claim {companyClaimName}" });

            var updatedUser = await _userService.UpdateUserAsync(userId, int.Parse(companyId), updateUserDto);

            if (updatedUser.Success)
                return Ok(updatedUser.User);

            else if (updatedUser.IsNotFound)
                return NotFound(updatedUser.Message);

            else if (updatedUser.IsConflict)
                return Conflict(updatedUser.Message);

            return BadRequest(updatedUser.Message);
        }


        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> DeleteUser(string IdCCNit)
        {
            var result = await _userService.DeleteUserAsync(IdCCNit);
            if (!result)
                return NotFound(new { Message = "User not found." });

            return NoContent();
        }

    }
}
