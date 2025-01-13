using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class LoginAuditRepository : ILoginAuditRepository
    {
        private readonly DapperContext _context;

        public LoginAuditRepository(DapperContext context)
        {
            _context = context;
        }

    
        public async Task<LoginAuditEntity?> GetLoginAuditByTokenIdAsync(string tokenId)
        {
            var query = "SELECT * FROM LoginAudits WHERE TokenId = @TokenId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<LoginAuditEntity>(query, new { TokenId = tokenId });
            }
        }


        public async Task<bool> CreateLoginAuditAsync(LoginAuditEntity loginAudit)
        {
            var query = @"
                INSERT INTO LoginAudits (TokenId, IdCCNit, IssuedAt, ExpiresAt, IPAddress, DeviceInfo, Status, IsAccessToken)
                VALUES (@TokenId, @IdCCNit, @IssuedAt, @ExpiresAt, @IPAddress, @DeviceInfo, @Status, @IsAccessToken)";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, loginAudit);
                return result > 0; 
            }
        }

        
        public async Task<bool> RevokeAllTokensAsync(string idCCNit)
        {
            var query = "UPDATE LoginAudits SET Status = 0 WHERE IdCCNit = @IdCCNit AND Status = 1";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { IdCCNit = idCCNit });
                return result > 0;
            }
        }


        public async Task<bool> RevokeTokenAsync(string tokenId)
        {
            var query = "UPDATE LoginAudits SET Status = 0 WHERE TokenId = @TokenId";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { TokenId = tokenId });
                return result > 0;
            }
        }


        public async Task<bool> IsTokenValidAsync(string tokenId)
        {
            var query = "SELECT Status FROM LoginAudits WHERE TokenId = @TokenId";

            using (var connection = _context.CreateConnection())
            {
                var status = await connection.QuerySingleOrDefaultAsync<bool?>(query, new { TokenId = tokenId });
                return status == true;
            }
        }


        public async Task<bool> HasActiveTokensAsync(string idCCNit)
        {
            var query = "SELECT COUNT(1) FROM LoginAudits WHERE IdCCNit = @IdCCNit AND Status = 1";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { IdCCNit = idCCNit });
                return count > 0;
            }
        }

    }
}