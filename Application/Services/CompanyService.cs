using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.DTOs.Companies;

namespace Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        // Crear compañía
        public async Task<CompanyResponseDTO> CreateCompanyAsync(CompanyDto companyDto)
        {
            // Validación para verificar si el nombre de la compañía es único
            if (!await _companyRepository.IsCompanyNameUniqueAsync(companyDto.Name))
            {
                return new CompanyResponseDTO(false, "The company name is already in use.", IsConflict: true);
            }

            // Validación para verificar si el NIT de la compañía es único
            if (!await _companyRepository.IsCompanyNITUniqueAsync(companyDto.NIT))
            {
                return new CompanyResponseDTO(false, "The company NIT is already in use.", IsConflict: true);
            }

            // Convertir DTO a entidad
            var companyEntity = new CompanyEntity
            {
                Name = companyDto.Name,
                NIT = companyDto.NIT
            };

            // Crear la compañía
            var id = await _companyRepository.CreateCompanyAsync(companyEntity);

            // Verificar si la creación fue exitosa
            if (id <= 0)
            {
                return new CompanyResponseDTO(false, "Error while creating the company.", IsBadRequest: true);
            }

            // Actualizar el DTO con el ID generado
            companyDto.Id = id;

            // Retornar respuesta con éxito
            return new CompanyResponseDTO(true, "Company created successfully.", companyDto);
        }

        // Obtener compañía por ID
        public async Task<CompanyResponseDTO> GetCompanyByIdAsync(int id)
        {
            var companyEntity = await _companyRepository.GetCompanyByIdAsync(id);

            if (companyEntity == null)
            {
                return new CompanyResponseDTO(false, "Company not found.", IsNotFound: true);
            }

            // Convertir entidad a DTO
            var companyDto = new CompanyDto
            {
                Id = companyEntity.Id,
                Name = companyEntity.Name,
                NIT = companyEntity.NIT
            };

            return new CompanyResponseDTO(true, "Company retrieved successfully.", companyDto);
        }

        // Obtener todas las compañías
        public async Task<CompanyResponseDTO> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();
            var companyDtos = new List<CompanyDto>();

            foreach (var company in companies)
            {
                companyDtos.Add(new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    NIT = company.NIT
                });
            }

            return new CompanyResponseDTO(true, "Companies retrieved successfully.", companyDtos);
        }

        // Actualizar compañía
        public async Task<CompanyResponseDTO> UpdateCompanyAsync(CompanyDto companyDto)
        {
            if (companyDto.Id == null)
            {
                return new CompanyResponseDTO(false, "Company ID is required for update.", IsBadRequest: true);
            }

            var companyEntity = new CompanyEntity
            {
                Id = (int)companyDto.Id,
                Name = companyDto.Name,
                NIT = companyDto.NIT
            };

            var success = await _companyRepository.UpdateCompanyAsync(companyEntity);

            if (!success)
            {
                return new CompanyResponseDTO(false, "Error while updating the company.", IsBadRequest: true);
            }

            return new CompanyResponseDTO(true, "Company updated successfully.", companyDto);
        }

        // Eliminar compañía
        public async Task<CompanyResponseDTO> DeleteCompanyAsync(int id)
        {
            var success = await _companyRepository.DeleteCompanyAsync(id);

            if (!success)
            {
                return new CompanyResponseDTO(false, "Error while deleting the company.", IsBadRequest: true);
            }

            return new CompanyResponseDTO(true, "Company deleted successfully.");
        }
    }
}
