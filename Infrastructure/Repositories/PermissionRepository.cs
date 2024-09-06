using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DapperContext _context;

        public PermissionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionEntity>> GetAll()
        {
            var query = "SELECT * FROM Permissions";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<PermissionEntity>(query);
            }
        }

        public async Task<PermissionEntity?> GetById(int id)
        {
            var query = "SELECT * FROM Permissions WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<PermissionEntity>(query, new { Id = id });
            }
        }

        public async Task<int> Create(PermissionEntity permission)
        {
            var query = "INSERT INTO Permissions (Name, Description) VALUES (@Name, @Description);" +
                        "SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(query, permission);
            }
        }

        public async Task<bool> Update(PermissionEntity permission)
        {
            var query = "UPDATE Permissions SET Name = @Name, Description = @Description WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, permission);
                return affectedRows > 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var query = "DELETE FROM Permissions WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }
    }
}
