using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<CompanyEntity>> GetCompaniesAsync();
        Task<CompanyEntity?> GetCompanyByIdAsync(int id);
        Task<int> CreateCompanyAsync(CompanyEntity company);
        Task<bool> UpdateCompanyAsync(CompanyEntity company);
        Task<bool> DeleteCompanyAsync(int id);
        Task<bool> IsCompanyNameUniqueAsync(string name);
        Task<bool> IsCompanyNitUniqueAsync(int nit);
        Task<bool> CompanyExistsByIdAsync(int id);
        Task<bool> UpdateCompanyNameAsync(int companyId, string newName);
        Task<bool> UpdateCompanyNitAsync(int companyId, int newNIT);
    }
}