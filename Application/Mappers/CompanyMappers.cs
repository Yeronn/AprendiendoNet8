using Application.DTOs.Company;
using Domain.Entities;

namespace Application.Mappers
{
    public static class CompanyMappers
    {
        public static CompanyEntity ToEntity(this CompanyDto dto)
        {
            return new CompanyEntity
            {
                Id = dto.Id ?? 0, 
                Name = dto.Name,
                NIT = dto.NIT
            };
        }


        public static CompanyEntity ToEntity(this CreateUpdateCompanyDto dto)
        {
            return new CompanyEntity
            {
                Name = dto.Name,
                NIT = dto.NIT
            };
        }


        public static CompanyDto ToDto(this CompanyEntity company)
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