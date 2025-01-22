using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System.ComponentModel.Design;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<UserEntity>> GetUsersAsync()
        {
            var query = "SELECT * FROM Users";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<UserEntity>(query);
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


        public async Task<UserEntity?> GetUserByIdAsync(int userId)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { Id = userId });
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


        public async Task<bool> UpdateUserAsync(UserEntity user) //*? Si actualiza la identificacion, es decir, la cédula, entonces hay que actualizar el IdCCNit
        {
            var query = @"UPDATE Users 
                        SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                            CCIdentification = @CCIdentification, HashedPassword = @HashedPassword, 
                            RoleId = @RoleId 
                        WHERE IdCCNit = @IdCCNit";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, user);
                return result > 0;
            }
        }


        public async Task<bool> DeleteUserAsync(string idCCNit)
        {
            var query = "DELETE FROM Users WHERE IdCCNit = @IdCCNit";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { IdCCNit = idCCNit });
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
                INNER JOIN Roles R ON U.RoleId = R.Id
                WHERE U.Email = @Email AND R.CompanyId = @CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Email = email, CompanyId = companyId });
                return count == 0;
            }
        }
    }
}
