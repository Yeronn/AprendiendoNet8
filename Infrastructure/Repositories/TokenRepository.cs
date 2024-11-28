using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly DapperContext _context;

        public TokenRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<int> CreateTokenAsync(TokenEntity token)
        {
            var query = @"INSERT INTO Tokens (RecoveryToken, Jti, IdCCNit)
                        VALUES (@RecoveryToken, @Jti, @IdCCNit);
                        SELECT SCOPE_IDENTITY();";

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(query, token);
            }
        }


        public async Task<TokenEntity?> GetTokenByIdAsync(int id)
        {
            var query = "SELECT * FROM Tokens WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<TokenEntity>(query, new { Id = id });
            }
        }


        public async Task<IEnumerable<TokenEntity>> GetAllTokensAsync()
        {
            var query = "SELECT * FROM Tokens";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<TokenEntity>(query);
            }
        }


        public async Task<bool> DeleteTokenAsync(int id)
        {
            var query = "DELETE FROM Tokens WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }
    }
}