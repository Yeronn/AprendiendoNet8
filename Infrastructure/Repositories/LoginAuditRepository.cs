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

        
        public async Task CreateLoginAuditAsync(LoginAuditEntity loginAudit)
        {
            var query = @"
                INSERT INTO LoginAudit (TokenId, IdCCNit, IssuedAt, ExpiresAt, IPAddress, DeviceInfo, Status)
                VALUES (@TokenId, @IdCCNit, @IssuedAt, @ExpiresAt, @IPAddress, @DeviceInfo, @Status)";
            
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, loginAudit);
            }
        }

        
        public async Task RevokeTokenAsync(string tokenId)
        {
            var query = "UPDATE LoginAudit SET Status = 0 WHERE TokenId = @TokenId";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { TokenId = tokenId });
            }
        }

        
        public async Task<bool> IsTokenValidAsync(string tokenId)
        {
            var query = "SELECT Status FROM LoginAudit WHERE TokenId = @TokenId";

            using (var connection = _context.CreateConnection())
            {
                var status = await connection.QuerySingleOrDefaultAsync<bool?>(query, new { TokenId = tokenId });
                return status == true;
            }
        } 
    }
}