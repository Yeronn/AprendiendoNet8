using Application.DTOs.Company;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }


        public async Task<CompanyResponseDto> CreateCompanyAsync(CreateUpdateCompanyDto companyDto)
        {
            bool isCompanyNameUnique = await _companyRepository.IsCompanyNameUniqueAsync(companyDto.Name!);
            if (!isCompanyNameUnique)
                return new CompanyResponseDto(false, "El nombre de la empresa ya se encuentra en uso.", IsConflict: true);

            bool isCompanyNITUnique = await _companyRepository.IsCompanyNITUniqueAsync((int)companyDto.NIT!);
            if (!isCompanyNITUnique)
                return new CompanyResponseDto(false, "El nit de la empresa ya se encuentra usado.", IsConflict: true);

            var companyEntity = companyDto.ToEntity();
            var id = await _companyRepository.CreateCompanyAsync(companyEntity);

            if (id <= 0)
                return new CompanyResponseDto(false, "Error al intentar crear la empresa.", IsBadRequest: true);

            var createdCompany = await GetCompanyByIdAsync(id);
            return new CompanyResponseDto(true, "Company created successfully.", createdCompany);
        }


        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            var companyEntity = await _companyRepository.GetCompanyByIdAsync(id);
            if (companyEntity == null)
                return null;
            return companyEntity.ToDto();
        }


        public async Task<IEnumerable<CompanyDto>?> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();
            if (!companies.Any())
                return null;
            var companyDtos = companies.Select(company => company.ToDto());
            return companyDtos;
        }

        
        public async Task<CompanyResponseDto> UpdateCompanyAsync(int companyId, CreateUpdateCompanyDto companyDto)
        {
            var companyExists = await ValidateCompanyExistsByIdAsync(companyId);
            if (!companyExists.Success)
                return companyExists;

            bool isCompanyNameUnique = await _companyRepository.IsCompanyNameUniqueAsync(companyDto.Name!);
            if (!isCompanyNameUnique)
                return new CompanyResponseDto(false, "El nombre de la empresa ya se encuentra en uso.", IsConflict: true);

            bool isCompanyNITUnique = await _companyRepository.IsCompanyNITUniqueAsync((int)companyDto.NIT!);
            if (!isCompanyNITUnique)
                return new CompanyResponseDto(false, "El nit de la empresa ya se encuentra usado.", IsConflict: true);

            var companyEntity = companyDto.ToEntity();
            companyEntity.Id = companyId;
            var success = await _companyRepository.UpdateCompanyAsync(companyEntity);
            if (!success)
                return new CompanyResponseDto(false, "Error mientras se actualiza la empresa.", IsBadRequest: true);

            var updatedCompany = await GetCompanyByIdAsync(companyId);
            return new CompanyResponseDto(true, "Empresa actualizada correctamente.", updatedCompany);
        }

        // Eliminar compañía
        public async Task<CompanyResponseDto> DeleteCompanyAsync(int id)
        {
            var companyExists = await ValidateCompanyExistsByIdAsync(id);
            if (!companyExists.Success)
                return companyExists;

            var success = await _companyRepository.DeleteCompanyAsync(id);

            if (!success)
                return new CompanyResponseDto(false, "Error while deleting the company.", IsBadRequest: true);

            return new CompanyResponseDto(true, "Company deleted successfully.");
        }




        private async Task<CompanyResponseDto> ValidateCompanyExistsByIdAsync(int id)
        {
            bool exists = await _companyRepository.CompanyExistsByIdAsync(id);
            if (!exists)
                return new CompanyResponseDto(false, "La empresa no existe.", IsNotFound: true);

            return new CompanyResponseDto(true, "La empresa existe.");
        }

    }
}
