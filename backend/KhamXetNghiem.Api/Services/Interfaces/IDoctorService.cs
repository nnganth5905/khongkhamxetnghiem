using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface IDoctorService
{
    Task<List<DoctorResponse>> GetDoctorsAsync(
        string? specialtyId,
        string? query,
        decimal? minimumStars,
        bool activeOnly,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResponse> GetDoctorByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResponse> CreateDoctorAsync(
        DoctorRequest request,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResponse> UpdateDoctorAsync(
        int id,
        DoctorRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteDoctorAsync(
        int id,
        CancellationToken cancellationToken = default
    );

    Task<List<PendingDoctorResultResponse>> GetPendingResultsAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResultDetailResponse> GetResultDetailAsync(
        string resultId,
        string login,
        CancellationToken cancellationToken = default
    );

    Task<ApproveResultResponse> ApproveResultAsync(
        string resultId,
        string conclusion,
        string login,
        CancellationToken cancellationToken = default
    );
}