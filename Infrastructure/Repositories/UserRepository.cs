using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.VisualBasic;
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

        public async Task<IEnumerable<UserEntity>> GetAll()
        {
            var query = "SELECT * FROM [User]";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<UserEntity>(query);
            }
        }

        public async Task<UserEntity?> GetById(int id)
        {
            var query = "SELECT * FROM [User] WHERE Id = @Id";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryFirstAsync<UserEntity>(query, new { Id = id });
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<UserEntity?> Login(string email, string password)
        {
            var query = "SELECT * FROM [User] WHERE Email = @Email AND Password = @Password";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var user = await connection.QueryFirstAsync<UserEntity>(query, new { Email = email, Password = password });
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserEntity?> Create (UserEntity newUser)
        {
            var query = "INSERT INTO [User] (Username, Password, Fullname, Email, IdentityCard, Salary) VALUES (@Username, @Password, @Fullname, @Email, @IdentityCard, @Salary)" +
                "SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new DynamicParameters();
            parameters.Add("Username", newUser.Username, DbType.String);
            parameters.Add("Password", newUser.Password, DbType.String);
            parameters.Add("Fullname", newUser.Fullname, DbType.String);
            parameters.Add("Email", newUser.Email, DbType.String);
            parameters.Add("IdentityCard", newUser.IdentityCard, DbType.Int32);
            parameters.Add("Salary", newUser.Salary, DbType.Decimal);


            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var createdUser = new UserEntity
                {
                    Id = id,
                    Username = newUser.Username,
                    Fullname = newUser.Fullname,
                    Email = newUser.Email,
                    IdentityCard = newUser.IdentityCard,
                };

                return createdUser;
            }
        }
    }
}
