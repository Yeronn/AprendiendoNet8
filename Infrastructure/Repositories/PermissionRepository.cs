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

        public async Task<IEnumerable<PermissionEntity>> GetAllPermissionsAsync()
        {
            var query = "SELECT * FROM Permission";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<PermissionEntity>(query);
            }
        }

        public async Task<PermissionEntity?> GetPermissionByIdAsync(int id)
        {
            var query = "SELECT * FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<PermissionEntity>(query, new { Id = id });
            }
        }

        public async Task<bool> CreatePermissionAsync(PermissionEntity permission)
        {
            var query = "INSERT INTO Permission (Name, Description) VALUES (@Name, @Description);" +
                        "SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, permission);
                if (id > 0)
                {
                    permission.Id = id;
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> UpdatePermissionAsync(PermissionEntity permission)
        {
            var query = "UPDATE Permission SET Name = @Name, Description = @Description WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, permission);
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdatePermissionNameAsync(int id, string name)
        {
            var query = "UPDATE Permission SET Name = @Name WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Name = name };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdatePermissionDescriptionAsync(int id, string description)
        {
            var query = "UPDATE Permission SET Description = @Description WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Description = description };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            var query = "DELETE FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<bool> ExistPermissionByIdAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
            }
        }

        public async Task<bool> ExistPermissionByNameAsync(string name)
        {
            var query = "SELECT COUNT(1) FROM Permission WHERE Name = @Name";
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Name = name });
                return count > 0;
            }
        }

        public async Task<string?> GetPermissionNameAsync(int id)
        {
            var query = "SELECT Name FROM Permission WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var permissionName = await connection.QuerySingleOrDefaultAsync<string>(query, new { Id = id });
                return permissionName!;
            }
        }

        public async Task<IEnumerable<PermissionEntity>> GetAllPermissionsByRoleIdAsync(int roleId)
        {
            var query = @"SELECT p.* FROM Permission p
                         JOIN RolePermission rp ON p.Id = rp.PermissionId
                         WHERE rp.RoleId = @RoleId";

            using (var connection = _context.CreateConnection())
            {
                var permissions = await connection.QueryAsync<PermissionEntity>(query, new { RoleId = roleId });
                return permissions;
            }
        }
    }
}
