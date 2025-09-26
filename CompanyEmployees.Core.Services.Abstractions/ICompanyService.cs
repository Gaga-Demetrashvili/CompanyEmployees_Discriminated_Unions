using CompanyEmployees.Core.Domain.Responses;
using OneOf;
using OneOf.Types;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges, CancellationToken ct = default);
    Task<OneOf<CompanyDto, CompanyNotFoundResponse>> GetCompanyAsync(Guid companyId, bool trackChanges, CancellationToken ct = default);
    Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company, CancellationToken ct = default);
    Task<OneOf<Success, CompanyNotFoundResponse>> UpdateCompanyAsync(Guid companyid, CompanyForUpdateDto companyForUpdate, 
        bool trackChanges, CancellationToken ct = default);
}