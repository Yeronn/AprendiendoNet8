using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<CompanyEntity?> GetCompanyByIdAsync(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<CompanyEntity>(query, new { Id = id });
            }
        }


        public async Task<int> CreateCompanyAsync(CompanyEntity company)
        {
            var query = "INSERT INTO Companies (Name, NIT) VALUES (@Name, @NIT);" +
                        "SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _context.CreateConnection())
            {
                var parameters = new { Name = company.Name, NIT = company.Nit };
                int id = await connection.QuerySingleAsync<int>(query, parameters);
                return id > 0 ? id : 0;
            }
        }


        public async Task<bool> IsCompanyNameUniqueAsync(string name)
        {
            var query = "SELECT COUNT(1) FROM Companies WHERE Name = @Name";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Name = name });
                return count == 0;
            }
        }


        public async Task<bool> IsCompanyNitUniqueAsync(int nit)
        {
            var query = "SELECT COUNT(1) FROM Companies WHERE NIT = @NIT";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { NIT = nit });
                return count == 0;
            }
        }


        public async Task<bool> CompanyExistsByIdAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM Companies WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
            }
        }


        public async Task<bool> UpdateCompanyNameAsync(int companyId, string newName)
        {
            var query = "UPDATE Companies SET Name = @NewName WHERE Id = @CompanyId";
            using (var connection = _context.CreateConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(query, new { NewName = newName, CompanyId = companyId });
                return rowsAffected > 0; 
            }
        }


        public async Task<bool> UpdateCompanyNitAsync(int companyId, int newNIT)
        {
            var query = "UPDATE Companies SET NIT = @NewNIT WHERE Id = @CompanyId";
            using (var connection = _context.CreateConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(query, new { NewNIT = newNIT, CompanyId = companyId });
                return rowsAffected > 0;
            }
        }


        public async Task<CompanyEntity?> GetCompanyByRoleIdAsync(int roleId)
        {
            var query = @"
                SELECT c.* 
                FROM Companies c
                INNER JOIN Roles r ON c.Id = r.CompanyId
                WHERE r.Id = @RoleId";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<CompanyEntity>(query, new { RoleId = roleId });
                
            }

        }
    }
}
