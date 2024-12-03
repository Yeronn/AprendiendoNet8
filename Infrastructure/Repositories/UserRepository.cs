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


        public async Task<IEnumerable<UserEntity>> GetUsersAsync()
        {
            var query = "SELECT * FROM Users";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<UserEntity>(query);
            }
        }


        public async Task<UserEntity?> GetUserByIdCCNitAsync(int idCCNit)
        {
            var query = "SELECT * FROM Users WHERE IdCCNit = @IdCCNit";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { IdCCNit = idCCNit });
            }
        }


        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { Id = id });
            }
        }


        public async Task<int?> CreateUserAsync(UserEntity user)
        {
            var query = @"INSERT INTO Users (IdCCNit, FirstName, LastName, Email, CCIdentification, HashedPassword, RoleId, RegistrationDate)
                        OUTPUT INSERTED.Id  -- Devolver el ID generado
                        VALUES (@IdCCNit, @FirstName, @LastName, @Email, @CCIdentification, @HashedPassword, @RoleId, @RegistrationDate)";

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


        public async Task<bool> DeleteUserAsync(int IdCCNit)
        {
            var query = "DELETE FROM Users WHERE IdCCNit = @IdCCNit";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { IdCCNit = IdCCNit });
                return result > 0;
            }
        }


        public async Task<UserEntity?> GetUserByLastJtiAsync(string lastJti)
        {
            var query = "SELECT * FROM Users WHERE LastJti = @LastJti";
            
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { LastJti = lastJti });
            }
        }


        public async Task<bool> IdCCNitExistsAsync(int idCCNit)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE IdCCNit = @IdCCNit";
            
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { IdCCNit = idCCNit });
                return count > 0;
            }
        }


        public async Task<string?> GetPasswordByIdCCNitAsync(int idCCNit)
        {
            var query = "SELECT HashedPassword FROM [Users] WHERE IdCCNit = @IdCCNit";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<string>(query, new { IdCCNit = idCCNit });
            }
        }


        public async Task<bool> UpdateLastJtiAsync(int idCCNit, string lastJti)
        {
            var query = "UPDATE [Users] SET LastJti = @LastJti WHERE IdCCNit = @IdCCNit";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, new { IdCCNit = idCCNit, LastJti = lastJti });
                return affectedRows > 0;  // Devuelve true si se actualizó al menos una fila
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
