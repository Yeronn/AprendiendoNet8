using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITokenRepository
    {
        Task<int> CreateTokenAsync(TokenEntity  token);
        Task<TokenEntity?> GetTokenByIdAsync(int id);
        Task<IEnumerable<TokenEntity>> GetAllTokensAsync();
        Task<bool> DeleteTokenAsync(int id);
    }
}