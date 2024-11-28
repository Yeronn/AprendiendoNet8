using Application.DTOs.Tokens;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        Task<int> CreateTokenAsync(CreateTokenDto tokenDto);
        Task<TokenDto?> GetTokenByIdAsync(int id);
        Task<bool> DeleteTokenAsync(int id);
        Task<IEnumerable<TokenDto>> GetAllTokensAsync();
    }
}