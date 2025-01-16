using Application.DTOs.Company;
using Domain.Entities;

namespace Application.Mappers
{
    public static class CompanyMappers
    {
        public static CompanyEntity ToEntity(this CreateUpdateCompanyDto dto)
        {
            return new CompanyEntity
            {
                Name = dto.Name,
                Nit = dto.Nit
            };
        }


        public static CompanyDto ToDto(this CompanyEntity company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Nit = company.Nit
            };
        }

    }
}