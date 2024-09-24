using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
        {
            var query = "SELECT * FROM [User]";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<UserEntity>(query);
                return users;
            }
        }


        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            var query = "SELECT * FROM [User] WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { Id = id });
            }
        }


        public async Task<UserEntity?> GetUserByUsernameAsync(string username)
        {
            var query = "SELECT * FROM [User] WHERE Username = @Username";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { Username = username });
            }
        }


        public async Task<UserEntity?> CreateUserAsync (UserEntity newUser)
        {
            var query = "INSERT INTO [User] (Username, Password, Fullname) VALUES (@Username, @Password, @Fullname)" +
                "SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new DynamicParameters();
            parameters.Add("Username", newUser.Username, DbType.String);
            parameters.Add("Password", newUser.Password, DbType.String);
            parameters.Add("Fullname", newUser.Fullname, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);

                var createdUser = new UserEntity
                {
                    Id = id,
                    Username = newUser.Username,
                    Fullname = newUser.Fullname,
                };

                return createdUser;
            }
        }


        public async Task<bool> UpdateFullnameAsync(int id, string fullname)
        {
            var query = "UPDATE [User] SET Fullname = @Fullname WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Fullname = fullname };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }


        public async Task<bool> UpdateUsernameAsync(int id, string username)
        {
            var query = "UPDATE [User] SET Username = @Username WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Username = username };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }


        public async Task<bool> UpdatePasswordAsync(int id, string password)
        {
            var query = "UPDATE [User] SET Password = @Password WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Id = id, Password = password };
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
        }


        public async Task UpdateUserJtiAsync(int userId, string jti)
        {
            var query = "UPDATE [User] SET LastJti = @Jti WHERE Id = @UserId";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Jti = jti, UserId = userId });
            }
        }


        public async Task<UserEntity?> GetUserByJtiAsync(string jti)
        {
            var query = "SELECT * FROM [User] WHERE LastJti = @Jti";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { Jti = jti });
            }
        }

    
        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Email = @Email";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Email = email });
                return count == 0; // Si es 0, el email es único
            }
        }


        public async Task<bool> IsFullnameUniqueAsync(string fullname)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Fullname = @Fullname";
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Fullname = fullname });
                return count == 0;
            }
        }


        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Username = @Username";
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Username = username });
                return count == 0;
            }
        }

        public async Task<bool> ExistUserByIdAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
            }
        }

        public async Task<bool> ExistUserByFullNameAsync(string fullname)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Fullname = @Fullname";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Fullname = fullname });
                return count > 0;
            }
        }

        public async Task<bool> ExistUserByUsernameAsync(string username)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Username = @Username";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Username = username });
                return count > 0;
            }
        }


        public async Task<IEnumerable<UserEntity>> GetAllUsersWithRolesAsync()
        {
            var query = @"
                SELECT u.Id, u.Username, u.Fullname, r.Id AS RoleId, r.Id, r.Name, r.Description
                FROM [User] u
                LEFT JOIN UserRole ur ON u.Id = ur.UserId
                LEFT JOIN Role r ON ur.RoleId = r.Id;
            ";

            var userDictionary = new Dictionary<int, UserEntity>();

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<UserEntity, RoleEntity, UserEntity>(
                    query,
                    (user, role) =>
                    {
                        // * The user does not exist in the dictionary
                        if (!userDictionary.TryGetValue(user.Id, out var currentUser))
                        {
                            currentUser = user;
                            currentUser.Roles = new List<RoleEntity>();
                            userDictionary.Add(user.Id, currentUser);
                        }

                        if (role != null && role.Id != 0)
                            currentUser.Roles.Add(role);

                        return currentUser;
                    },
                    splitOn: "RoleId"
                );
            }

            return userDictionary.Values;
        }

        public async Task<bool> AddRolesToUserAsync(int userId, List<int> roleIds)
        {
            var query = "INSERT INTO UserRole (UserId, RoleId) VALUES (@UserId, @RoleId)";
            using (var connection = _context.CreateConnection())
            {
                var parameters = roleIds.Select(roleId => new { UserId = userId, RoleId = roleId }).ToList();

                await connection.ExecuteAsync(query, parameters);
            }

            return true;
        }
    }
}
