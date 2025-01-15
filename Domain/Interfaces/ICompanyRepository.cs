using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<CompanyEntity?> GetCompanyByIdAsync(int id);
        Task<int> CreateCompanyAsync(CompanyEntity company);
        Task<bool> IsCompanyNameUniqueAsync(string name);
        Task<bool> IsCompanyNitUniqueAsync(int nit);
        Task<bool> CompanyExistsByIdAsync(int id);
        Task<bool> UpdateCompanyNameAsync(int companyId, string newName);
        Task<bool> UpdateCompanyNitAsync(int companyId, int newNIT);
        Task<CompanyEntity?> GetCompanyByRoleIdAsync(int roleId);
    }
}