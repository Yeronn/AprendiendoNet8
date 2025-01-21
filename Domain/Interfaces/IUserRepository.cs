using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetUsersAsync();
        Task<UserEntity?> GetUserByIdAsync(int id);
        Task<UserEntity?> GetUserByCCNumberAndCompanyIdAsync(int ccNumber, int companyId);
        Task<int?> CreateUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(UserEntity user);
        Task<bool> DeleteUserAsync(string idCCNit); 
        Task<bool> IdCCNitExistsAsync(string idCCNit);
        Task<string?> GetPasswordByIdCCNitAsync(string idCCNit);
        Task<bool> IsEmailAvailableInCompanyAsync(string email, int companyId);
    }
}
