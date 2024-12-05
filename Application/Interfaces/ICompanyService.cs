using Application.DTOs.Company;

namespace Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>?> GetCompaniesAsync();
        Task<CompanyResponseDto> CreateCompanyAsync(CreateUpdateCompanyDto companyDto);
        Task<CompanyDto?> GetCompanyByIdAsync(int id);
        Task<CompanyResponseDto> UpdateCompanyAsync(int companyId, CreateUpdateCompanyDto companyDto);
        Task<CompanyResponseDto> DeleteCompanyAsync(int id);
        Task<CompanyResponseDto> ValidateCompanyExistsByIdAsync(int id);
        Task<CompanyDto?> GetCompanyByRoleIdAsync(int roleId);
    }
}