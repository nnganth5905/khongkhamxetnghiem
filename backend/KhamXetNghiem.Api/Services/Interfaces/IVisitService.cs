using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface IVisitService
{
    Task<CheckInResponse> CheckInAsync(
        string appointmentId,
        string? customerId,
        string? visitType,
        string? note,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<CheckInResponse> CheckInByQrAsync(
        string qrCode,
        string? note,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<List<QueueItemResponse>> GetWaitingListAsync(
        string? type,
        CancellationToken cancellationToken = default
    );

    Task<VisitResponse> GetVisitByIdAsync(
        long id,
        string? type,
        CancellationToken cancellationToken = default
    );

    Task<VisitResponse> UpdateStatusAsync(
        long id,
        string type,
        VisitStatusRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<List<DoctorQueueItemResponse>> GetDoctorWaitingQueueAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task CallPatientAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task HoldPatientAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<long> StartExamAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task CallByAppointmentAsync(
        string appointmentId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task HoldByAppointmentAsync(
        string appointmentId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task SkipByAppointmentAsync(
        string appointmentId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<WalkInVisitResponse> CreateWalkInAsync(
        WalkInVisitRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );
}