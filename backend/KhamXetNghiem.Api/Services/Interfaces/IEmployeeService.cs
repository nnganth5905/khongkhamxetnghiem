using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeResponse>> GetAllEmployeesAsync(
        string? query,
        string? role,
        CancellationToken cancellationToken = default
    );

    Task<EmployeeResponse> GetEmployeeByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<EmployeeResponse> CreateEmployeeAsync(
        EmployeeRequest request,
        CancellationToken cancellationToken = default
    );

    Task<EmployeeResponse> UpdateEmployeeAsync(
        string id,
        EmployeeRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteEmployeeAsync(
        string id,
        CancellationToken cancellationToken = default
    );
}