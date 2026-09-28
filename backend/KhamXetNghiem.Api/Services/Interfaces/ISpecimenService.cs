using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface ISpecimenService
{
    Task<List<TechnicianOptionResponse>>
        GetTechniciansAsync(
            CancellationToken cancellationToken = default
        );

    Task<List<PendingSpecimenItemResponse>>
        GetPendingItemsAsync(
            string appointmentId,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        );

    Task<SpecimenResponse> CollectAsync(
        CollectSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenActionResponse> HandoverAsync(
        string id,
        HandoverSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<List<SpecimenResponse>> GetSpecimensAsync(
        string? status,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenResponse> GetSpecimenAsync(
        string id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenActionResponse> ReceiveAsync(
        string id,
        ReceiveSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenActionResponse> RejectAsync(
        string id,
        RejectSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );
}