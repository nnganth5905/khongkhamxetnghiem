using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Repositories.Models;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class SpecimenService
    : ISpecimenService
{
    private readonly ISpecimenRepository
        _specimenRepository;

    public SpecimenService(
        ISpecimenRepository specimenRepository
    )
    {
        _specimenRepository =
            specimenRepository;
    }

    // =====================================================
    // TECHNICIANS
    // =====================================================

    public Task<List<TechnicianOptionResponse>>
        GetTechniciansAsync(
            CancellationToken cancellationToken = default
        )
    {
        return _specimenRepository
            .GetTechniciansAsync(
                cancellationToken
            );
    }

    // =====================================================
    // PENDING ITEMS
    // =====================================================

    public async Task<List<PendingSpecimenItemResponse>>
        GetPendingItemsAsync(
            string appointmentId,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        await RequireDoctorAsync(
            user,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(appointmentId))
        {
            throw new BadRequestException(
                "Thiếu mã lịch xét nghiệm."
            );
        }

        return await _specimenRepository
            .GetPendingItemsAsync(
                appointmentId.Trim(),
                cancellationToken
            );
    }

    // =====================================================
    // COLLECT
    // =====================================================

    public async Task<SpecimenResponse> CollectAsync(
        CollectSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        if (string.IsNullOrWhiteSpace(request.AppointmentId))
        {
            throw new BadRequestException(
                "Mã lịch xét nghiệm không được để trống."
            );
        }

        if (request.TestOrderItemId <= 0)
        {
            throw new BadRequestException(
                "Vui lòng chọn xét nghiệm cần lấy mẫu."
            );
        }

        if (string.IsNullOrWhiteSpace(request.SpecimenType))
        {
            throw new BadRequestException(
                "Loại mẫu không được để trống."
            );
        }

        request.AppointmentId =
            request.AppointmentId.Trim();

        request.SpecimenType =
            request.SpecimenType.Trim();

        request.Barcode =
            NormalizeNullable(
                request.Barcode
            );

        request.Notes =
            NormalizeNullable(
                request.Notes
            );

        return await _specimenRepository
            .CollectAsync(
                request,
                actor.DoctorId!.Value,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // HANDOVER
    // =====================================================

    public async Task<SpecimenActionResponse> HandoverAsync(
        string id,
        HandoverSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new BadRequestException(
                "Thiếu mã mẫu."
            );
        }

        if (string.IsNullOrWhiteSpace(request.ReceiverId))
        {
            throw new BadRequestException(
                "Vui lòng chọn kỹ thuật viên tiếp nhận."
            );
        }

        request.ReceiverId =
            request.ReceiverId.Trim();

        request.Note =
            NormalizeNullable(
                request.Note
            );

        return await _specimenRepository
            .HandoverAsync(
                id.Trim(),
                request,
                actor.DoctorId!.Value,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // LIST
    // =====================================================

    public async Task<List<SpecimenResponse>> GetSpecimensAsync(
        string? status,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        return await _specimenRepository
            .GetSpecimensAsync(
                actor.EmployeeId!,
                status,
                cancellationToken
            );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<SpecimenResponse> GetSpecimenAsync(
        string id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        return await _specimenRepository
                   .GetSpecimenAsync(
                       id,
                       actor.EmployeeId!,
                       cancellationToken
                   )
               ?? throw new ResourceNotFoundException(
                   "Không tìm thấy mẫu bệnh phẩm hoặc mẫu không được bàn giao cho bạn."
               );
    }

    // =====================================================
    // RECEIVE
    // =====================================================

    public async Task<SpecimenActionResponse> ReceiveAsync(
        string id,
        ReceiveSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        request.Condition =
            string.IsNullOrWhiteSpace(request.Condition)
                ? "GOOD"
                : request.Condition
                    .Trim()
                    .ToUpperInvariant();

        request.Notes =
            NormalizeNullable(
                request.Notes
            );

        return await _specimenRepository
            .ReceiveAsync(
                id,
                request,
                actor.EmployeeId!,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // REJECT
    // =====================================================

    public async Task<SpecimenActionResponse> RejectAsync(
        string id,
        RejectSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new BadRequestException(
                "Lý do từ chối mẫu không được để trống."
            );
        }

        return await _specimenRepository
            .RejectAsync(
                id,
                request.Reason.Trim(),
                actor.EmployeeId!,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // ACTOR
    // =====================================================

    private async Task<SpecimenActorContext>
        RequireDoctorAsync(
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
                "Không xác định được tài khoản bác sĩ."
            );

        if (!actor.DoctorId.HasValue)
        {
            throw new ForbiddenException(
                "Tài khoản chưa liên kết hồ sơ bác sĩ."
            );
        }

        return actor;
    }

    private async Task<SpecimenActorContext>
        RequireTechnicianAsync(
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
                "Không xác định được tài khoản kỹ thuật viên."
            );

        if (string.IsNullOrWhiteSpace(actor.EmployeeId))
        {
            throw new ForbiddenException(
                "Tài khoản chưa liên kết nhân viên."
            );
        }

        if (
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
                "Nhân viên không phải kỹ thuật viên đang hoạt động."
            );
        }

        return actor;
    }

    private async Task<SpecimenActorContext?>
        ResolveActorAsync(
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

        return await _specimenRepository
            .GetActorAsync(
                login,
                cancellationToken
            );
    }

    private static string? NormalizeNullable(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}