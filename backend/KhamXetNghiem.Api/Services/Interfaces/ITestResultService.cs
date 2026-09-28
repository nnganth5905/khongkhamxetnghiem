using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface ITestResultService
{
    Task<ResultEntryResponse> GetEntryAsync(
        long worklistId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<TestResultActionResponse> SaveDraftAsync(
        long worklistId,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<TestResultActionResponse> SubmitAsync(
        long worklistId,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<List<PendingDoctorResultResponse>> GetPendingAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResultDetailResponse> GetDoctorResultAsync(
        string resultId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<TestResultActionResponse> ApproveAsync(
        string resultId,
        ApproveTestResultRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );
}