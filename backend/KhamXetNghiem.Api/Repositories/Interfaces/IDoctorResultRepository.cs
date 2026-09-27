using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IDoctorResultRepository
{
    Task<DoctorIdentity?> FindDoctorIdentityAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<List<PendingDoctorResultResponse>> GetPendingAsync(
        int doctorId,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResultDetailResponse?> GetDetailAsync(
        string resultId,
        int doctorId,
        CancellationToken cancellationToken = default
    );

    Task<ApproveResultResponse> ApproveAsync(
        string resultId,
        string conclusion,
        DoctorIdentity doctor,
        CancellationToken cancellationToken = default
    );
}

public sealed record DoctorIdentity(
    int UserId,
    int DoctorId,
    string Email,
    string? Username
);