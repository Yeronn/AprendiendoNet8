using Dapper;
using Domain.Entities;
using Domain.Enums;
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
                INSERT INTO LoginAudits (TokenId, UserId, IssuedAt, ExpiresAt, IPAddress, DeviceInfo, TokenStatusId, TokenTypeId, CompanyId)
                VALUES (@TokenId, @UserId, @IssuedAt, @ExpiresAt, @IPAddress, @DeviceInfo, @TokenStatusId, @TokenTypeId, @CompanyId)";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, loginAudit);
                return result > 0;
            }
        }


        public async Task<bool> RevokeAllTokensAsync(int userId)
        {
            var query = "UPDATE LoginAudits SET TokenStatusId = @RevokedStatus WHERE UserId = @UserId AND TokenStatusId = @ActiveStatus";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new
                {
                    UserId = userId,
                    ActiveStatus = (int)TokenStatus.Valid,
                    RevokedStatus = (int)TokenStatus.Revoked
                });

                return result > 0;
            }
        }



        public async Task<bool> RevokeTokenAsync(string tokenId)
        {
            var query = "UPDATE LoginAudits SET TokenStatusId = @TokenStatusId WHERE TokenId = @TokenId";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { TokenStatusId = (int)TokenStatus.Revoked, TokenId = tokenId });
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


        public async Task<bool> HasActiveTokensAsync(int userId)
        {
            var query = "SELECT COUNT(1) FROM LoginAudits WHERE UserId = @UserId AND TokenStatusId = @TokenStatusId";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { UserId = userId, TokenStatusId = (int)TokenStatus.Valid });
                return count > 0;
            }
        }


        public async Task<bool> DeleteRefreshTokensAsync(int userId)
        {
            var query = "DELETE FROM LoginAudits WHERE UserId = @UserId AND TokenTypeId = @RefreshToken";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new
                {
                    UserId = userId,
                    RefreshToken = (int)TokenType.Refresh
                });

                return result > 0;
            }
        }


        public async Task<bool> HasActiveRefreshTokensAsync(int userId)
        {
            var query = "SELECT COUNT(1) FROM LoginAudits WHERE UserId = @UserId AND TokenTypeId = @RefreshToken";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new
                {
                    UserId = userId,
                    RefreshToken = (int)TokenType.Refresh
                });

                return count > 0;
            }
        }



    }
}