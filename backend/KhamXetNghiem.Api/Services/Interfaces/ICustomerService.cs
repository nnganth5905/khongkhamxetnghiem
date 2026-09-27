using System.Security.Claims;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse> GetCurrentCustomerAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<CustomerResponse> UpdateCurrentCustomerAsync(
        ClaimsPrincipal user,
        CustomerRequest request,
        CancellationToken cancellationToken = default
    );

    Task<List<CustomerResponse>> GetAllCustomersAsync(
        string? query,
        CancellationToken cancellationToken = default
    );

    Task<CustomerResponse> GetCustomerByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<CustomerResponse> CreateCustomerAsync(
        CustomerRequest request,
        CancellationToken cancellationToken = default
    );

    Task<CustomerResponse> UpdateCustomerAsync(
        string id,
        CustomerRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteCustomerAsync(
        string id,
        CancellationToken cancellationToken = default
    );
}