using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;

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

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
        (Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges, 
        CancellationToken ct = default)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges, ct);

        var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            empTrackChanges, ct);

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeDb);

        return (employeeToPatch, employeeDb);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, 
        Employee employeeEntity, CancellationToken ct = default)
    {
        _mapper.Map(employeeToPatch, employeeEntity);

        await _repository.SaveAsync(ct);
    }

    private async Task<Company> CheckIfCompanyExists(Guid companyId, bool trackChanges, 
        CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        return company;
    }

    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, 
        Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges, ct);
        if (employeeDb is null)
            throw new EmployeeNotFoundException(id);
        return employeeDb;
    }
}
