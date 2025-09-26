using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Infrastructure.Presentation.ActionFilters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController : ControllerBase
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

        patchDoc.ApplyTo(result.employeeToPatch, ModelState);

        TryValidateModel(result.employeeToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity, ct);

        return NoContent();
    }
}