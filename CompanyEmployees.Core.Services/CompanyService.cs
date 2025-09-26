using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges, CancellationToken ct = default)
    {
        var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges, ct);

        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        return companiesDto;
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges, 
        CancellationToken ct = default)
    {
        var company = await GetCompanyAndCheckIfItExists(id, trackChanges, ct);

        var companyDto = _mapper.Map<CompanyDto>(company);

        return companyDto;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company, CancellationToken ct = default)
    {
        var companyEntity = _mapper.Map<Company>(company);

        _repository.Company.CreateCompany(companyEntity);
        await _repository.SaveAsync(ct);

        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

        return companyToReturn;
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, 
        bool trackChanges, CancellationToken ct = default)
    {
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges, ct);

        _mapper.Map(companyForUpdate, company);
        await _repository.SaveAsync(ct);
    }

    private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges, 
        CancellationToken ct)
    {
        var company = await _repository.Company.GetCompanyAsync(id, trackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(id);

        return company;
    }
}
