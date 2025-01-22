using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetUsersAsync();
        Task<UserEntity?> GetUserByIdAsync(int userId);
        Task<UserEntity?> GetUserByCCNumberAndCompanyIdAsync(int ccNumber, int companyId);
        Task<int?> CreateUserAsync(UserEntity user);
        Task<bool> UpdateUserAsync(UserEntity user);
        Task<bool> DeleteUserAsync(string idCCNit); 
        Task<bool> VerifyUserExistsByCCNumberAndCompanyIdAsync(int ccNumber, int companyId);
        Task<string?> GetPasswordByUserIdAsync(int userId);
        Task<bool> IsEmailAvailableInCompanyAsync(string email, int companyId);
    }
}
