using Application.DTOs.Company;

namespace Application.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyResponseDto> CreateCompanyAsync(CreateUpdateCompanyDto companyDto);
        Task<CompanyDto?> GetCompanyByIdAsync(int companyId);
        Task<CompanyResponseDto> UpdateCompanyAsync(int companyId, CreateUpdateCompanyDto companyDto);
        Task<CompanyResponseDto> ValidateCompanyExistsByIdAsync(int companyId);
        Task<CompanyDto?> GetCompanyByRoleIdAsync(int roleId);
    }
}