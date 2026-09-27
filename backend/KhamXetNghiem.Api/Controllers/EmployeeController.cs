using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api/admin/employees")]
[Authorize(Roles = "ADMIN")]
public sealed class EmployeeController
    : ControllerBase
{
    private readonly IEmployeeService
        _employeeService;

    public EmployeeController(
        IEmployeeService employeeService
    )
    {
        _employeeService =
            employeeService;
    }

    // =====================================================
    // LIST
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> GetAllEmployees(
        [FromQuery] string? q,
        [FromQuery] string? role,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _employeeService
                .GetAllEmployeesAsync(
                    q,
                    role,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // DETAIL
    // =====================================================

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(
        string id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _employeeService
                .GetEmployeeByIdAsync(
                    id,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // CREATE
    // =====================================================

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] EmployeeRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _employeeService
                .CreateEmployeeAsync(
                    request,
                    cancellationToken
                );

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new
            {
                id =
                    result.IdNhanVien
            },
            result
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(
        string id,
        [FromBody] EmployeeRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _employeeService
                .UpdateEmployeeAsync(
                    id,
                    request,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // SOFT DELETE
    // =====================================================

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(
        string id,
        CancellationToken cancellationToken
    )
    {
        await _employeeService
            .DeleteEmployeeAsync(
                id,
                cancellationToken
            );

        return NoContent();
    }
}