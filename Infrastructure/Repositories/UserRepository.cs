using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<UserEntity?> GetUserByIdAsync(int userId)
        {
            var query = "SELECT * FROM Users WHERE Id = @UserId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<UserEntity>(query, new { UserId = userId });
            }
        }


        public async Task<IEnumerable<UserEntity>> GetUsersByCompanyIdAsync(int companyId)
        {
            var query = "SELECT * FROM Users WHERE CompanyId = @CompanyId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<UserEntity>(query, new { CompanyId = companyId });
            }
        }


        public async Task<UserEntity?> GetUserByCCNumberAndCompanyIdAsync(int ccNumber, int companyId)
        {
            var query = "SELECT * FROM Users WHERE CCNumber = @CCNumber AND CompanyId = @CompanyId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { CCNumber = ccNumber, CompanyId = companyId });
            }
        }


        public async Task<UserEntity?> GetUserByIdAndCompanyIdAsync(int userId, int companyId)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id AND CompanyId = @CompanyId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { Id = userId, CompanyId = companyId });
            }
        }


        public async Task<int?> CreateUserAsync(UserEntity user)
        {
            var query = @"INSERT INTO Users (FirstName, LastName, Email, CCNumber, HashedPassword, RoleId, RegistrationDate, CompanyId)
                        OUTPUT INSERTED.Id  -- Devolver el ID generado
                        VALUES (@FirstName, @LastName, @Email, @CCNumber, @HashedPassword, @RoleId, @RegistrationDate, @CompanyId)";

            using (var connection = _context.CreateConnection())
            {
                var createdId = await connection.QuerySingleOrDefaultAsync<int?>(query, user);
                return createdId;
            }
        }


        public async Task<bool> UpdateUserAsync(UserEntity user)
        {
            var query = @"UPDATE Users 
                        SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                            CCNumber = @CCNumber, HashedPassword = @HashedPassword, 
                            RoleId = @RoleId, IsActive = @IsActive
                        WHERE Id = @Id AND CompanyId = @CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, user);
                return result > 0;
            }
        }


        public async Task<bool> SetUserInactiveAsync(int userId, int companyId)
        {
            var query = "UPDATE Users SET IsActive = 0 WHERE Id = @UserId AND CompanyId = @CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { UserId = userId, CompanyId = companyId });
                return result > 0;
            }
        }


        public async Task<bool> VerifyUserExistsByCCNumberAndCompanyIdAsync(int ccNumber, int companyId)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE CCNumber = @CCNumber AND CompanyId = @CompanyId";
            
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { CCNumber = ccNumber, CompanyId = companyId });
                return count > 0;
            }
        }


        public async Task<string?> GetPasswordByUserIdAsync(int userId)
        {
            var query = "SELECT HashedPassword FROM [Users] WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<string>(query, new { Id = userId });
            }
        }


        public async Task<bool> IsEmailAvailableInCompanyAsync(string email, int companyId)
        {
            var query = @"
                SELECT COUNT(1)
                FROM Users U
                WHERE U.Email = @Email AND U.CompanyId = @CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Email = email, CompanyId = companyId });
                return count == 0;
            }
        }


        public async Task<bool> CheckUserExistsByIdAsync(int userId)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE Id = @UserId";
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { UserId = userId });
                return count > 0;
            }
        }


    }
}
