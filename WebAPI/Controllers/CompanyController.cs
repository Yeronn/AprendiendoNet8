using Application.DTOs.Company;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateCompanyAsync([FromBody] CreateUpdateCompanyDto companyDto)
        {
            var response = await _companyService.CreateCompanyAsync(companyDto);
            if (response.Success)
                return CreatedAtRoute("GetCompanyById", new { companyId = response.Company!.Id}, response.Company);
            else if (response.IsConflict)
                return Conflict(response.Message);
            else
                return BadRequest(response.Message);
        }


        [HttpGet("{companyId}", Name = "GetCompanyById")]
        public async Task<IActionResult> GetCompanyByIdAsync(int companyId)
        {
            var response = await _companyService.GetCompanyByIdAsync(companyId);
            if (response != null)
                return Ok(response);

            return NotFound();
        }

        
        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompanyAsync(int companyId, [FromBody] CreateUpdateCompanyDto companyDto)
        {
            var response = await _companyService.UpdateCompanyAsync(companyId, companyDto);
            if (response.Success)
                return Ok(new
                {
                    response.Success,
                    response.Message,
                    response.Company
                });
            else if (response.IsConflict)
                return Conflict(response.Message);
            else if (response.IsNotFound)
                return NotFound(response.Message);
            else
                return BadRequest(response.Message);
        }
    }
}