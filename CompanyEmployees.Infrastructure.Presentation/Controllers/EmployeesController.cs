using CompanyEmployees.Core.Services.Abstractions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController : ApiControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesController(IServiceManager service) => _service = service;

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc, CancellationToken ct)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null.");

        var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id,
            compTrackChanges: false, empTrackChanges: true, ct);

        return await result.Match(
            async empTuple =>
            {
                patchDoc.ApplyTo(empTuple.employeeToPatch, ModelState);

                TryValidateModel(empTuple.employeeToPatch);

                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);

                await _service.EmployeeService.SaveChangesForPatchAsync(empTuple.employeeToPatch,
               empTuple.employeeEntity!, ct);

                return NoContent();
            },
            async compNotFound => await Task.FromResult(ProcessError(compNotFound)),
            async empNotFound => await Task.FromResult(ProcessError(empNotFound))
         );
    }
}