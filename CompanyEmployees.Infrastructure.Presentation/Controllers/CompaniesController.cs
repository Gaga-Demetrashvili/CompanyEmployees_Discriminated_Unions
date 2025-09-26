using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Infrastructure.Presentation.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController : ApiControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetCompanies(CancellationToken ct)
    {
        var companies = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false, ct);

        return Ok(companies);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    public async Task<IActionResult> GetCompany(Guid id, CancellationToken ct)
    {
        var result = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false, ct);

        return result.Match(
            company => Ok(company),
            error => ProcessError(error)
        );
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company, 
        CancellationToken ct)
    {
        var createdCompany = await _service.CompanyService.CreateCompanyAsync(company, ct);

        return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company, CancellationToken ct)
    {
        var result = await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true, ct);

        return result.Match(
            success => NoContent(),
            error => ProcessError(error)
        );
    }
}