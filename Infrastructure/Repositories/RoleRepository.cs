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


        public async Task<IEnumerable<RoleEntity>> GetRolesAsync(int companyId)
        {
            const string query = @"
                SELECT 
                    r.Id AS Id,
                    r.Name AS Name,
                    r.Description AS Description,
                    r.Status AS Status,
                    r.CompanyId AS CompanyId,
                    p.Id AS PermissionId,
                    p.Id,
                    p.Name,
                    p.Description
                FROM 
                    Roles r
                LEFT JOIN 
                    RolePermissions rp ON r.Id = rp.RoleId
                LEFT JOIN 
                    Permissions p ON rp.PermissionId = p.Id
                WHERE 
                    r.CompanyId = @CompanyId"; 

            using (var connection = _context.CreateConnection())
            {
                var roleDictionary = new Dictionary<int, RoleEntity>();

                var result = await connection.QueryAsync<RoleEntity, PermissionEntity, RoleEntity>(
                    query,
                    (role, permission) =>
                    {
                        if (!roleDictionary.TryGetValue(role.Id, out var currentRole))
                        {
                            currentRole = new RoleEntity
                            {
                                Id = role.Id,
                                Name = role.Name,
                                Description = role.Description,
                                Status = role.Status,
                                CompanyId = role.CompanyId,
                                Permissions = new List<PermissionEntity>()
                            };
                            roleDictionary.Add(currentRole.Id, currentRole);
                        }

                        // Añadir permisos si no son nulos
                        if (permission != null && permission.Id != 0)
                        {
                            currentRole.Permissions.Add(new PermissionEntity
                            {
                                Id = permission.Id,
                                Name = permission.Name,
                                Description = permission.Description
                            });
                        }

                        return currentRole;
                    },
                    new { CompanyId = companyId }, // Parámetro de la consulta SQL
                    splitOn: "PermissionId"  // Divide la fila entre RoleEntity y PermissionEntity
                );

                return roleDictionary.Values;
            }
        }



        public async Task<RoleEntity?> GetRoleByIdAsync(int id)
        {
            var query = "SELECT * FROM Roles WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<RoleEntity>(query, new { Id = id });
            }
        }


        public async Task<int> CreateRoleAsync(RoleEntity role)
        {
            var query = "INSERT INTO Roles (Name, Description, Status, CompanyId) VALUES (@Name, @Description, @Status, @CompanyId); SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.ExecuteScalarAsync<int>(query, new { role.Name, role.Description, role.Status, role.CompanyId });
                return id > 0 ? id : 0;
            }
        }


        public async Task<bool> UpdateRoleAsync(RoleEntity role)
        {
            var query = "UPDATE Roles SET Name = @Name, Description = @Description, Status = @Status WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, role);
                return affectedRows > 0;
            }
        }


        public async Task<bool> UpdateRoleNameAsync(int id, string name)
        {
            var query = "UPDATE Roles SET Name = @Name WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Name = name };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }


        public async Task<bool> UpdateRoleDescriptionAsync(int id, string description)
        {
            var query = "UPDATE Roles SET Description = @Description WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Description = description };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }


        public async Task<bool> DeleteRoleAsync(int id)
        {
            var query = "DELETE FROM Roles WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }


        public async Task<bool> ExistRoleByIdAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM Roles WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
            }
        }


        public async Task<bool> CheckRoleNameAvailabilityAsync(string name, int companyId)
        {
            var query = @"
                SELECT COUNT(1) 
                FROM Roles 
                WHERE Name = @Name AND CompanyId = @CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Name = name, CompanyId = companyId });
                return count == 0;
            }
        }



        public async Task<bool> AddPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var query = "INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)";
            using (var connection = _context.CreateConnection())
            {
                var parameters = permissionIds.Select(permissionId => new { RoleId = roleId, PermissionId = permissionId }).ToList();
                await connection.ExecuteAsync(query, parameters);
            }
            return true;
        }


        public async Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var query = "DELETE FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId IN @PermissionIds";
            using (var connection = _context.CreateConnection())
            {
                var parameters = new { RoleId = roleId, PermissionIds = permissionIds };
                var rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
        }


        public async Task<IEnumerable<RoleEntity>> GetAllRolesByPermissionIdAsync(int permissionId)
        {
            var query = @"SELECT r.* FROM Roles r
                        JOIN RolePermissions rp ON r.Id = rp.RoleId
                        WHERE rp.PermissionId = @PermissionId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<RoleEntity>(query, new { PermissionId = permissionId });
            }
        }


        public async Task<RoleEntity?> GetRoleByUserIdAsync(int userId)
        {
            var query = @"SELECT r.* FROM Roles r
                        JOIN Users u ON r.Id = u.RoleId
                        WHERE u.Id = @UserId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<RoleEntity>(query, new { UserId = userId });
            }
        }


        public async Task<bool> IsRoleUnassignedAsync(int roleId)
        {
            var query = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM Users WHERE RoleId = @RoleId
                ) THEN 0 ELSE 1 END";

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>(query, new { RoleId = roleId });
            }
        }




    }
}
