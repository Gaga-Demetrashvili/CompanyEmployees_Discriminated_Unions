using CompanyEmployees.Core.Domain.Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface IEmployeeService
{
    Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
        Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges, 
        CancellationToken ct = default);
    Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity, 
        CancellationToken ct = default);
}
