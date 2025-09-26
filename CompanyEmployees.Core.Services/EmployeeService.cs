using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Domain.Responses;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;
using OneOf;
using Shared.DataTransferObjects;
using CompanyEmployees.Core.Services.Extensions;
using CompanyEmployees.Core.Domain.OneOfTypes;

namespace CompanyEmployees.Core.Services;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<EmployeePatchTupleOrCompanyNotFoundOrEmployeeNotFound> GetEmployeeForPatchAsync
        (Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges, 
        CancellationToken ct = default)
    {
        var (_, companyError) = await CheckIfCompanyExists(companyId, compTrackChanges, ct);
        if (companyError is not null)
            return companyError;

        var (employeeDb, employeeError) = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            empTrackChanges, ct);
        if (employeeError is not null)
            return employeeError;

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeDb);

        return (employeeToPatch!, employeeDb!);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, 
        Employee employeeEntity, CancellationToken ct = default)
    {
        _mapper.Map(employeeToPatch, employeeEntity);

        await _repository.SaveAsync(ct);
    }

    private async Task<OneOf<Company, CompanyNotFoundResponse>> CheckIfCompanyExists(Guid companyId, bool trackChanges, 
        CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);
        if (company is null)
            return new CompanyNotFoundResponse(companyId);

        return company;
    }

    private async Task<OneOf<Employee, EmployeeNotFoundResponse>> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, 
        Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges, ct);
        if (employeeDb is null)
            return new EmployeeNotFoundResponse(id);

        return employeeDb;
    }
}
