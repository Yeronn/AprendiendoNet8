using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DapperContext _context;

        public RoleRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<RoleEntity?> GetById(int id)
        {
            var query = "SELECT * FROM Role WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var role = await connection.QuerySingleOrDefaultAsync<RoleEntity>(query, new { Id = id });
                //if (role != null)
                //{
                //    var permissionQuery = "SELECT p.* FROM Permission p JOIN RolePermission rp ON p.Id = rp.PermissionId WHERE rp.RoleId = @RoleId";
                //    var permissions = await connection.QueryAsync<PermissionEntity>(permissionQuery, new { RoleId = role.Id });
                //    role.Permissions = permissions.ToList();
                //}
                return role;
            }
        }

        public async Task<IEnumerable<RoleEntity>> GetAll()
        {
            var query = "SELECT * FROM Role";
            using (var connection = _context.CreateConnection())
            {
                var roles = await connection.QueryAsync<RoleEntity>(query);
                //foreach (var role in roles)
                //{
                //    var permissionQuery = "SELECT p.* FROM Permission p JOIN RolePermission rp ON p.Id = rp.PermissionId WHERE rp.RoleId = @RoleId";
                //    var permissions = await connection.QueryAsync<PermissionEntity>(permissionQuery, new { RoleId = role.Id });
                //    role.Permissions = permissions.ToList();
                //}
                return roles;
            }
        }

        public async Task<bool> Create(RoleEntity role)
        {
            var query = "INSERT INTO Role (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.ExecuteScalarAsync<int>(query, new { role.Name });
                if (id > 0)
                {
                    role.Id = id;
                    var insertPermissionsQuery = "INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)";
                    foreach (var permission in role.Permissions)
                    {
                        await connection.ExecuteAsync(insertPermissionsQuery, new { RoleId = role.Id, PermissionId = permission.Id });
                    }
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> Update(RoleEntity role)
        {
            var query = "UPDATE Role SET Name = @Name WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, role);
                if (affectedRows > 0)
                {
                    var deletePermissionsQuery = "DELETE FROM RolePermission WHERE RoleId = @RoleId";
                    await connection.ExecuteAsync(deletePermissionsQuery, new { RoleId = role.Id });

                    var insertPermissionsQuery = "INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)";
                    foreach (var permission in role.Permissions)
                    {
                        await connection.ExecuteAsync(insertPermissionsQuery, new { RoleId = role.Id, PermissionId = permission.Id });
                    }
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var query = "DELETE FROM Role WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }
    }
}
