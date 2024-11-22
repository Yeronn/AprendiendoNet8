using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<CompanyEntity>> GetAllCompaniesAsync();
        Task<CompanyEntity?> GetCompanyByIdAsync(int id);
        Task<int> CreateCompanyAsync(CompanyEntity company);
        Task<bool> UpdateCompanyAsync(CompanyEntity company);
        Task<bool> DeleteCompanyAsync(int id);
        Task<bool> IsCompanyNameUniqueAsync(string name);
        Task<bool> IsCompanyNITUniqueAsync(int nit);
        Task<bool> CompanyExistsByIdAsync(int id);
    }
}