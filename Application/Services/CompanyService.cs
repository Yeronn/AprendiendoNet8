using Application.DTOs.Company;
using Application.Interfaces;
using Application.Mappers;
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


        public async Task<CompanyResponseDto> CreateCompanyAsync(CreateUpdateCompanyDto createDto)
        {
            var availableName = await CheckCompanyNameAvailabilityAsync(createDto.Name!);
            if (!availableName.Success)
                return availableName;

            var availableNit = await CheckCompanyNitAvailabilityAsync((int)createDto.Nit!);
            if (!availableNit.Success)
                return availableNit;

            var companyEntity = createDto.ToEntity();
            var id = await _companyRepository.CreateCompanyAsync(companyEntity);

            if (id <= 0)
                return new CompanyResponseDto(false, "No se pudo crear la empresa.", IsBadRequest: true);

            var createdCompany = await GetCompanyByIdAsync(id);
            return new CompanyResponseDto(true, "La empresa fue creada.", createdCompany);
        }


        public async Task<CompanyDto?> GetCompanyByIdAsync(int companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyByIdAsync(companyId);
            return companyEntity?.ToDto();
        }

        
        public async Task<CompanyResponseDto> UpdateCompanyAsync(int companyId, CreateUpdateCompanyDto companyDto)
        {
            var currentCompany = await GetCompanyByIdAsync(companyId);
            if (currentCompany == null)
                return new CompanyResponseDto(false, "La empresa no existe", IsNotFound: true);

            if (companyDto.Name != currentCompany.Name)
            {
                var updatedName = await UpdateCompanyNameAsync(companyId, companyDto.Name!);
                if (!updatedName.Success)
                    return updatedName;
            }

            if (companyDto.Nit != currentCompany.Nit)
            {
                var updatedNit = await UpdateCompanyNitAsync(companyId, (int)companyDto.Nit!);
                if (!updatedNit.Success)
                return updatedNit;
            }

            var updatedCompany = await GetCompanyByIdAsync(companyId);
            return new CompanyResponseDto(true, "Empresa actualizada correctamente.", updatedCompany);
        }


        public async Task<CompanyResponseDto> ValidateCompanyExistsByIdAsync(int companyId)
        {
            bool exists = await _companyRepository.CompanyExistsByIdAsync(companyId);
            if (!exists)
                return new CompanyResponseDto(false, "La empresa no existe.", IsNotFound: true);

            return new CompanyResponseDto(true, "La empresa existe.");
        }


        public async Task<CompanyDto?> GetCompanyByRoleIdAsync(int roleId)
        {
            var company = await _companyRepository.GetCompanyByRoleIdAsync(roleId);
            return company?.ToDto();
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
                return new CompanyResponseDto(true, "Nombre actualizado");
            return new CompanyResponseDto(false, "Error al actualizar el nombre");
        }


        private async Task<CompanyResponseDto> UpdateCompanyNitAsync(int companyId, int nit)
        {
            var availableNit = await CheckCompanyNitAvailabilityAsync(nit);
            if (!availableNit.Success)
                return availableNit;
            bool updatedCompanyNit = await _companyRepository.UpdateCompanyNitAsync(companyId, nit);
            if (updatedCompanyNit)
                return new CompanyResponseDto(true, "Nit actualizado");
            return new CompanyResponseDto(false, "Error al actualizar el NIT");
        }



    }
}
