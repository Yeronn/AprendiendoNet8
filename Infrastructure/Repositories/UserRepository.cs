using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repository;
using Infrastructure.Data;
using Microsoft.VisualBasic;

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

        public async Task<UserEntity> GetById(int id)
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

        public async Task<UserEntity> Login(string email, string password)
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

        public async Task Create (UserEntity newUser)
        {
            var query = "INSERT INTO [User] (Username, Password, Fullname, Email, IdentityCard, Salary) " +
                        "VALUES (@Username, @Password, @Fullname, @Email, @IdentityCard, @Salary);";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new
                {
                    newUser.Username,
                    newUser.Password,
                    newUser.Fullname,
                    newUser.Email,
                    newUser.IdentityCard,
                    newUser.Salary
                };

                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
