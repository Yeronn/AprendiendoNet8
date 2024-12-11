using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetUsersAsync();
        Task<UserEntity?> GetUserByIdAsync(int id);
        Task<UserEntity?> GetUserByIdCCNitAsync(int idCCNit);
        Task<int?> CreateUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(UserEntity user);
        Task<bool> DeleteUserAsync(int id); 
        Task<bool> IdCCNitExistsAsync(int idCCNit);
        Task<string?> GetPasswordByIdCCNitAsync(int idCCNit);
        Task<bool> IsEmailAvailableInCompanyAsync(string email, int companyId);
    }
}
