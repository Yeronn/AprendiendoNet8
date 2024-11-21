using Application.DTOs.Companies;

namespace Application.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyResponseDTO> CreateCompanyAsync(CompanyDto companyDto);
        Task<CompanyResponseDTO> GetCompanyByIdAsync(int id);
        Task<CompanyResponseDTO> GetAllCompaniesAsync();
        Task<CompanyResponseDTO> UpdateCompanyAsync(CompanyDto companyDto);
        Task<CompanyResponseDTO> DeleteCompanyAsync(int id);
    }
}