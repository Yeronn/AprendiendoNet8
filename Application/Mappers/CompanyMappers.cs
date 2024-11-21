using Application.DTOs.Companies;
using Domain.Entities;

namespace Application.Mappers
{
    public class CompanyMappers
    {
        public static CompanyEntity ToEntity(CompanyDto dto)
        {
            return new CompanyEntity
            {
                Id = dto.Id ?? 0, 
                Name = dto.Name ?? string.Empty,
                NIT = dto.NIT ?? string.Empty
            };
        }

        
        public static CompanyDto ToDto(CompanyEntity company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                NIT = company.NIT
            };
        }
    }
}