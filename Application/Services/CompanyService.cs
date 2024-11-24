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
            var availableName = await CheckCompanyNameAvailabilityAsync(companyDto.Name!);
            if (!availableName.Success)
                return availableName;

            var availableNit = await CheckCompanyNitAvailabilityAsync((int)companyDto.NIT!);
            if (!availableNit.Success)
                return availableNit;

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
            var currentCompany = await GetCompanyByIdAsync(companyId);
            if (currentCompany == null)
                return new CompanyResponseDto(false, "La empresa no se encuentra en el sistema", IsNotFound: true);

            if (companyDto.Name != currentCompany.Name)
            {
                var updatedName = await UpdateCompanyNameAsync(companyId, companyDto.Name!);
                if (!updatedName.Success)
                    return updatedName;
            }

            if (companyDto.NIT != currentCompany.NIT)
            {
                var updatedNit = await UpdateCompanyNitAsync(companyId, (int)companyDto.NIT!);
                if (!updatedNit.Success)
                return updatedNit;
            }

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


        public async Task<CompanyResponseDto> ValidateCompanyExistsByIdAsync(int id)
        {
            bool exists = await _companyRepository.CompanyExistsByIdAsync(id);
            if (!exists)
                return new CompanyResponseDto(false, "La empresa no existe.", IsNotFound: true);

            return new CompanyResponseDto(true, "La empresa existe.");
        }




        private async Task<CompanyResponseDto> CheckCompanyNameAvailabilityAsync(string companyName)
        {
            bool nameUnique = await _companyRepository.IsCompanyNameUniqueAsync(companyName);
            if (!nameUnique)
                return new CompanyResponseDto(false, "Nombre no disponible", IsConflict: true);

            return new CompanyResponseDto(true, "Nombre válido.");
        }


        private async Task<CompanyResponseDto> CheckCompanyNitAvailabilityAsync(int companyNit)
        {
            bool nameUnique = await _companyRepository.IsCompanyNitUniqueAsync(companyNit);
            if (!nameUnique)
                return new CompanyResponseDto(false, "Nit no disponible", IsConflict: true);

            return new CompanyResponseDto(true, "Nit válido.");
        }


        private async Task<CompanyResponseDto> UpdateCompanyNameAsync(int companyId, string name)
        {
            var availableName = await CheckCompanyNameAvailabilityAsync(name);
            if (!availableName.Success)
                return availableName;
            bool updatedCompanyName = await _companyRepository.UpdateCompanyNameAsync(companyId, name);
            if (updatedCompanyName)
                return new CompanyResponseDto(true, "Nombre de la empresa actualizado");
            return new CompanyResponseDto(false, "Error al actualizar el nombre de la empresa");
        }


        private async Task<CompanyResponseDto> UpdateCompanyNitAsync(int companyId, int nit)
        {
            var availableNit = await CheckCompanyNitAvailabilityAsync(nit);
            if (!availableNit.Success)
                return availableNit;
            bool updatedCompanyNit = await _companyRepository.UpdateCompanyNitAsync(companyId, nit);
            if (updatedCompanyNit)
                return new CompanyResponseDto(true, "Nit de la empresa actualizado");
            return new CompanyResponseDto(false, "Error al actualizar el NIT de la empresa");
        }

    }
}
