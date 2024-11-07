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


        public async Task<int?> CreateRoleAsync(RoleEntity role)
        {
            var query = "INSERT INTO Role (Name, Description) VALUES (@Name, @Description); SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.ExecuteScalarAsync<int>(query, new { role.Name, role.Description });
                if (id > 0)
                {
                    return id;
                }
                return null;
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


        public async Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var query = "DELETE FROM RolePermission WHERE RoleId = @RoleId AND PermissionId IN @PermissionIds";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new 
                { 
                    RoleId = roleId, 
                    PermissionIds = permissionIds 
                };

                var rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
        }
    

        public async Task<IEnumerable<RoleEntity>> GetAllRolesByPermissionIdAsync(int permissionId) 
        {
            var query = @"SELECT r.* FROM Role r
                         JOIN RolePermission rp ON r.Id = rp.RoleId
                         WHERE rp.PermissionId = @PermissionId";
            
            using (var connection = _context.CreateConnection())
            {
                var roles = await connection.QueryAsync<RoleEntity>(query, new { PermissionId = permissionId});
                return roles;
            }
        }


        public async Task<IEnumerable<RoleEntity>> GetAllRolesByUserIdAsync(int userId)
        {
            var query = @"SELECT r.* FROM Role r
                         JOIN UserRole ur ON r.Id = ur.RoleId
                         WHERE ur.UserId = @UserId";

            using (var connection = _context.CreateConnection())
            {
                var roles = await connection.QueryAsync<RoleEntity>(query, new { UserId = userId });
                return roles;
            }
        }


        public async Task<bool> IsRoleNotAssignedToAnyUserAsync(int roleId)
        {
            var query = "SELECT COUNT(1) FROM UserRole WHERE RoleId = @RoleId";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { RoleId = roleId });
                return count == 0;
            }
        }
    }
}
