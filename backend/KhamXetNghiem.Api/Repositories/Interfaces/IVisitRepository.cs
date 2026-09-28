using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Repositories.Models;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IVisitRepository
{
    Task<VisitActorContext?> GetActorAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<VisitAppointmentSource?> FindAppointmentAsync(
        string appointmentCode,
        string? type,
        CancellationToken cancellationToken = default
    );

    Task<VisitAppointmentSource?> FindAppointmentByQrAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    Task<CheckInResponse> CheckInAsync(
        VisitAppointmentSource appointment,
        int? actorUserId,
        string? note,
        CancellationToken cancellationToken = default
    );

    Task<List<QueueItemResponse>> GetWaitingListAsync(
        string? type,
        CancellationToken cancellationToken = default
    );

    Task<Visit?> GetVisitAsync(
        long visitId,
        string type,
        CancellationToken cancellationToken = default
    );

    Task<Visit> UpdateStatusAsync(
        long visitId,
        string type,
        string status,
        string? note,
        int? actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<List<DoctorQueueItemResponse>> GetDoctorQueueAsync(
        int doctorId,
        CancellationToken cancellationToken = default
    );

    Task CallDoctorPatientAsync(
        long visitId,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );

    Task HoldDoctorPatientAsync(
        long visitId,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<long> StartExamAsync(
        long visitId,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );

    Task CallByAppointmentAsync(
        string appointmentCode,
        int? actorUserId,
        CancellationToken cancellationToken = default
    );

    Task HoldByAppointmentAsync(
        string appointmentCode,
        int? actorUserId,
        CancellationToken cancellationToken = default
    );

    Task SkipByAppointmentAsync(
        string appointmentCode,
        int? actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<WalkInVisitResponse> CreateWalkInAsync(
        WalkInVisitData request,
        int? actorUserId,
        CancellationToken cancellationToken = default
    );
}