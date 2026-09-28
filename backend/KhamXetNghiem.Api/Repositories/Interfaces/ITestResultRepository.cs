using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Repositories.Models;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface ITestResultRepository
{
    Task<TestResultActor?> GetActorAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<ResultEntryResponse> GetResultEntryAsync(
        long worklistId,
        string technicianId,
        CancellationToken cancellationToken = default
    );

    Task<TestResultActionResponse> SaveResultAsync(
        long worklistId,
        ResultEntryRequest request,
        bool submit,
        string technicianId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<List<PendingDoctorResultResponse>> GetPendingDoctorResultsAsync(
        int doctorId,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResultDetailResponse> GetDoctorResultAsync(
        string resultId,
        int doctorId,
        CancellationToken cancellationToken = default
    );

    Task<TestResultActionResponse> ApproveAsync(
        string resultId,
        string conclusion,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    );
}