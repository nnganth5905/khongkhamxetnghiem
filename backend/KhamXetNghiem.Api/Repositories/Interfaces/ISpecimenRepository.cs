using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Repositories.Models;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface ISpecimenRepository
{
    Task<SpecimenActorContext?> GetActorAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<List<TechnicianOptionResponse>> GetTechniciansAsync(
        CancellationToken cancellationToken = default
    );

    Task<List<PendingSpecimenItemResponse>> GetPendingItemsAsync(
        string appointmentId,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenResponse> CollectAsync(
        CollectSpecimenRequest request,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenActionResponse> HandoverAsync(
        string specimenReference,
        HandoverSpecimenRequest request,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<List<SpecimenResponse>> GetSpecimensAsync(
        string technicianId,
        string? status,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenResponse?> GetSpecimenAsync(
        string specimenReference,
        string technicianId,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenActionResponse> ReceiveAsync(
        string specimenReference,
        ReceiveSpecimenRequest request,
        string technicianId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenActionResponse> RejectAsync(
        string specimenReference,
        string reason,
        string technicianId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );
}