using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Repositories.Models;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class TestResultService
    : ITestResultService
{
    private readonly ITestResultRepository
        _repository;

    public TestResultService(
        ITestResultRepository repository
    )
    {
        _repository =
            repository;
    }

    // =====================================================
    // TECHNICIAN
    // =====================================================

    public async Task<ResultEntryResponse> GetEntryAsync(
        long worklistId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        return await _repository
            .GetResultEntryAsync(
                worklistId,
                actor.EmployeeId!,
                cancellationToken
            );
    }

    public async Task<TestResultActionResponse> SaveDraftAsync(
        long worklistId,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        return await _repository
            .SaveResultAsync(
                worklistId,
                NormalizeRequest(request),
                false,
                actor.EmployeeId!,
                actor.UserId,
                cancellationToken
            );
    }

    public async Task<TestResultActionResponse> SubmitAsync(
        long worklistId,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        if (
            request.Indicators.Count == 0
        )
        {
            throw new BadRequestException(
                "Phiếu xét nghiệm chưa có kết quả chỉ số."
            );
        }

        return await _repository
            .SaveResultAsync(
                worklistId,
                NormalizeRequest(request),
                true,
                actor.EmployeeId!,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // DOCTOR
    // =====================================================

    public async Task<List<PendingDoctorResultResponse>> GetPendingAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        return await _repository
            .GetPendingDoctorResultsAsync(
                actor.DoctorId!.Value,
                cancellationToken
            );
    }

    public async Task<DoctorResultDetailResponse> GetDoctorResultAsync(
        string resultId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        return await _repository
            .GetDoctorResultAsync(
                resultId.Trim(),
                actor.DoctorId!.Value,
                cancellationToken
            );
    }

    public async Task<TestResultActionResponse> ApproveAsync(
        string resultId,
        ApproveTestResultRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(resultId))
        {
            throw new BadRequestException(
                "Thiếu mã kết quả."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Conclusion))
        {
            throw new BadRequestException(
                "Bác sĩ phải nhập kết luận trước khi duyệt."
            );
        }

        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        return await _repository
            .ApproveAsync(
                resultId.Trim(),
                request.Conclusion.Trim(),
                actor.DoctorId!.Value,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // AUTH
    // =====================================================

    private async Task<TestResultActor> RequireTechnicianAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            )
            ?? throw new ForbiddenException(
                "Không xác định được kỹ thuật viên."
            );

        if (
            string.IsNullOrWhiteSpace(
                actor.EmployeeId
            )
            ||
            !string.Equals(
                actor.EmployeePosition,
                "ktv",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            !string.Equals(
                actor.EmployeeStatus,
                "yes",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new ForbiddenException(
                "Tài khoản không liên kết kỹ thuật viên đang hoạt động."
            );
        }

        return actor;
    }

    private async Task<TestResultActor> RequireDoctorAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            )
            ?? throw new ForbiddenException(
                "Không xác định được bác sĩ."
            );

        if (!actor.DoctorId.HasValue)
        {
            throw new ForbiddenException(
                "Tài khoản chưa liên kết hồ sơ bác sĩ."
            );
        }

        return actor;
    }

    private async Task<TestResultActor?> ResolveActorAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var login =
            user.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            )
            ??
            user.FindFirstValue(
                ClaimTypes.Email
            )
            ??
            user.Identity?.Name;

        if (string.IsNullOrWhiteSpace(login))
        {
            return null;
        }

        return await _repository
            .GetActorAsync(
                login,
                cancellationToken
            );
    }

    // =====================================================
    // NORMALIZE
    // =====================================================

    private static ResultEntryRequest NormalizeRequest(
        ResultEntryRequest request
    )
    {
        request.GeneralResult =
            Clean(
                request.GeneralResult
            );

        request.Notes =
            Clean(
                request.Notes
            );

        foreach (var indicator in request.Indicators)
        {
            indicator.IndicatorId =
                indicator.IndicatorId?.Trim()
                ?? string.Empty;

            indicator.Value =
                Clean(
                    indicator.Value
                );

            indicator.Note =
                Clean(
                    indicator.Note
                );
        }

        return request;
    }

    private static string? Clean(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}