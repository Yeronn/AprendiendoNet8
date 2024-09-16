using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DapperContext _context;

        public RoleRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleEntity>> GetAllRolesAsync()
        {
            var query = "SELECT * FROM Role";
            using (var connection = _context.CreateConnection())
            {
                var roles = await connection.QueryAsync<RoleEntity>(query);
                return roles;
            }
        }

        public async Task<RoleEntity?> GetRoleByIdAsync(int id)
        {
            var query = "SELECT * FROM Role WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var role = await connection.QuerySingleOrDefaultAsync<RoleEntity>(query, new { Id = id });
                return role;
            }
        }

        public async Task<bool> CreateRoleAsync(RoleEntity role)
        {
            var query = "INSERT INTO Role (Name, Description) VALUES (@Name, @Description); SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.ExecuteScalarAsync<int>(query, new { role.Name, role.Description });
                if (id > 0)
                {
                    role.Id = id;
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> UpdateRoleAsync(RoleEntity role)
        {
            var query = "UPDATE Role SET Name = @Name, Description = @Description WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, role);
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateRoleNameAsync(int id, string name)
        {
            var query = "UPDATE Role SET Name = @Name WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Name = name };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateRoleDescriptionAsync(int id, string description)
        {
            var query = "UPDATE Role SET Description = @Description WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Description = description };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var query = "DELETE FROM Role WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<bool> ExistRoleByIdAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM Role WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
            }
        }

        public async Task<bool> ExistRoleByNameAsync(string name)
        {
            var query = "SELECT COUNT(1) FROM Role WHERE Name = @Name";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Name = name });
                return count > 0;
            }
        }

        public async Task<bool> AddPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var query = "INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)";
            using (var connection = _context.CreateConnection())
            {
                var parameters = permissionIds.Select(permissionId => new { RoleId = roleId, PermissionId = permissionId }).ToList();

                await connection.ExecuteAsync(query, parameters);
            }

            return true;
        }
    }
}
