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
            var query = "SELECT * FROM Permission";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<PermissionEntity>(query);
            }
        }

        public async Task<PermissionEntity?> GetById(int id)
        {
            var query = "SELECT * FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<PermissionEntity>(query, new { Id = id });
            }
        }

        public async Task<int> Create(PermissionEntity permission)
        {
            var query = "INSERT INTO Permission (Name, Description) VALUES (@Name, @Description);" +
                        "SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(query, permission);
            }
        }

        public async Task<bool> Update(PermissionEntity permission)
        {
            var query = "UPDATE Permission SET Name = @Name, Description = @Description WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, permission);
                return affectedRows > 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var query = "DELETE FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<bool> ExistById(int id)
        {
            var query = "SELECT COUNT(1) FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
            }
        }

        public async Task<bool> VerifyUniqueName(string name)
        {
            var query = "SELECT COUNT(1) FROM Permission WHERE Name = @Name";
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Name = name });
                bool nameIsUnique = count == 0;
                return nameIsUnique;
            }
        }

        public async Task<string?> GetName(int id)
        {
            var query = "SELECT Name FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var permissionName = await connection.QuerySingleOrDefaultAsync<string>(query, new { Id = id });
                return permissionName!;
            }
        }
    }
}
