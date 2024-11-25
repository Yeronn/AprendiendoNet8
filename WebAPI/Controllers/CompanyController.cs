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

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _companyService.GetCompaniesAsync();
            if (response != null)
                return Ok(response);
            
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateCompanyDto companyDto)
        {
            var response = await _companyService.CreateCompanyAsync(companyDto);
            if (response.Success)
                return CreatedAtRoute("GetCompany", new { id = response.Company!.Id}, response.Company);
            else if (response.IsConflict)
                return Conflict(response.Message);
            else
                return BadRequest(response.Message);
        }


        [HttpGet("{id}", Name = "GetCompany")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var response = await _companyService.GetCompanyByIdAsync(id);
            if (response != null)
                return Ok(response);

            return NotFound();
        }

        // PUT: api/companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateUpdateCompanyDto companyDto)
        {
            var response = await _companyService.UpdateCompanyAsync(id, companyDto);
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

        // DELETE: api/companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _companyService.DeleteCompanyAsync(id);
            if (response.Success)
                return NoContent();
            else if (response.IsNotFound)
                return NotFound(response.Message);
            else
                return StatusCode(500, response.Message);
        }
    }
}