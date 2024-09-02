using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.VisualBasic;

namespace Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
        {
            var query = "SELECT * FROM Users";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<UserEntity>(query);
            }
        }

        public async Task<UserEntity> GetUserByIdAsync(int id)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstAsync<UserEntity>(query, new { Id = id});
            }
        }

        public async Task<UserEntity> LoginAsync(string username, string password)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var user = await connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { Username = username, Password = password });
                    return user;
                }
            }
            catch (Exception ex)
            {
                // Loguea el error o maneja la excepción según sea necesario
                // Por ejemplo: Logger.LogError(ex.Message);

                // Devuelve null en caso de cualquier excepción inesperada
                return null;
            }
        }

        // Implementación adicional de métodos para obtener, actualizar, eliminar usuarios...
    }
}
