using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<ReceptionistDashboardResponse>
        GetReceptionistAsync(
            CancellationToken cancellationToken = default
        );

    Task<DoctorDashboardResponse>
        GetDoctorAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        );

    Task<TechnicianDashboardResponse>
        GetTechnicianAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        );

    Task<AdminDashboardResponse>
        GetAdminAsync(
            CancellationToken cancellationToken = default
        );

    Task<CustomerDashboardResponse>
        GetCustomerAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        );
}