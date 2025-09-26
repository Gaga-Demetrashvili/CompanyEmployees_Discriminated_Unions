using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges, CancellationToken ct = default);
    Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges, CancellationToken ct = default);
    Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company, CancellationToken ct = default);
    Task UpdateCompanyAsync(Guid companyid, CompanyForUpdateDto companyForUpdate, 
        bool trackChanges, CancellationToken ct = default);
}