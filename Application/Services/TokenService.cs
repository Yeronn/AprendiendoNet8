using Application.DTOs.Tokens;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }


        public async Task<int> CreateTokenAsync(CreateTokenDto tokenDto)
        {
            var tokenEntity = tokenDto.ToEntity();

            return await _tokenRepository.CreateTokenAsync(tokenEntity);
        }


        public async Task<TokenDto?> GetTokenByIdAsync(int id)
        {
            var tokenEntity = await _tokenRepository.GetTokenByIdAsync(id);
            if (tokenEntity == null)
                return null;

            return tokenEntity.ToDto();
        }


        public async Task<IEnumerable<TokenDto>> GetAllTokensAsync()
        {
            var tokens = await _tokenRepository.GetAllTokensAsync();
            return tokens.Select(token => token.ToDto());
        }


        public async Task<bool> DeleteTokenAsync(int id)
        {
            return await _tokenRepository.DeleteTokenAsync(id);
        }
    }
}