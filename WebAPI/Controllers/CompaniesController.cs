using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompaniesService _companiesService;

        public CompaniesController(ICompaniesService companiesService)
        {
            _companiesService = companiesService;
        }

        // GET: api/companies
        [HttpGet]
        public async Task<ActionResult<CompanyResponseDTO>> GetAllAsync()
        {
            var response = await _companiesService.GetAllAsync();
            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        // GET: api/companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyResponseDTO>> GetByIdAsync(int id)
        {
            var response = await _companiesService.GetByIdAsync(id);
            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        // POST: api/companies
        [HttpPost]
        public async Task<ActionResult<CompanyResponseDTO>> CreateAsync([FromBody] CompanyDto companyDto)
        {
            var response = await _companiesService.CreateAsync(companyDto);
            if (response.Success)
            {
                return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Company?.Id }, response);
            }

            return BadRequest(response);
        }

        // PUT: api/companies/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyResponseDTO>> UpdateAsync(int id, [FromBody] CompanyDto companyDto)
        {
            var response = await _companiesService.UpdateAsync(id, companyDto);
            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        // DELETE: api/companies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CompanyResponseDTO>> DeleteAsync(int id)
        {
            var response = await _companiesService.DeleteAsync(id);
            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }
    }
}