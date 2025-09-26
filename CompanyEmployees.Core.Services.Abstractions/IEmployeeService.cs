using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.OneOfTypes;
using CompanyEmployees.Core.Domain.Responses;
using OneOf;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface IEmployeeService
{
    Task<EmployeePatchTupleOrCompanyNotFoundOrEmployeeNotFound> GetEmployeeForPatchAsync(
        Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges, 
        CancellationToken ct = default);
    Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity, 
        CancellationToken ct = default);
}
