using Application.DTOs.Tokens;
using Domain.Entities;

namespace Application.Mappers
{
    public static class TokenMapper
    {
        public static TokenDto ToDto (this TokenEntity tokenEntity)
        {
            return new TokenDto
            {
                Id = tokenEntity.Id,
                RecoveryToken = tokenEntity.RecoveryToken,
                Jti = tokenEntity.Jti,
                IdCCNit = tokenEntity.IdCCNit,
                DateCreated = tokenEntity.DateCreated
            };
        }


        public static TokenEntity ToEntity (this CreateTokenDto createToken)
        {
            return new TokenEntity
            {
                RecoveryToken = createToken.RecoveryToken,
                Jti = createToken.Jti,
                IdCCNit = createToken.IdCCNit
            };
        }
    }
}