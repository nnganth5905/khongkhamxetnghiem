using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Repositories.Models;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class VisitService : IVisitService
{
    private readonly IVisitRepository
        _visitRepository;

    public VisitService(
        IVisitRepository visitRepository
    )
    {
        _visitRepository =
            visitRepository;
    }

    // =====================================================
    // CHECK-IN
    // =====================================================

    public async Task<CheckInResponse> CheckInAsync(
        string appointmentId,
        string? customerId,
        string? visitType,
        string? note,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                appointmentId
            )
        )
        {
            throw new BadRequestException(
                "Mã đặt lịch không được để trống."
            );
        }

        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            );

        var appointment =
            await _visitRepository
                .FindAppointmentAsync(
                    appointmentId.Trim(),
                    NormalizeNullable(
                        visitType
                    ),
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy lịch hẹn {appointmentId}."
            );

        if (
            !string.IsNullOrWhiteSpace(
                customerId
            )
            &&
            !string.Equals(
                appointment.CustomerId,
                customerId.Trim(),
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BadRequestException(
                "Mã khách hàng không thuộc lịch hẹn này."
            );
        }

        return await _visitRepository
            .CheckInAsync(
                appointment,
                actor?.UserId,
                NormalizeNullable(
                    note
                ),
                cancellationToken
            );
    }

    // =====================================================
    // QR
    // =====================================================

    public async Task<CheckInResponse> CheckInByQrAsync(
        string qrCode,
        string? note,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                qrCode
            )
        )
        {
            throw new BadRequestException(
                "Mã QR không được để trống."
            );
        }

        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            );

        var appointment =
            await _visitRepository
                .FindAppointmentByQrAsync(
                    qrCode.Trim(),
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                "Mã QR / mã đặt lịch không hợp lệ."
            );

        return await _visitRepository
            .CheckInAsync(
                appointment,
                actor?.UserId,
                NormalizeNullable(
                    note
                ),
                cancellationToken
            );
    }

    // =====================================================
    // WAITING
    // =====================================================

    public Task<List<QueueItemResponse>> GetWaitingListAsync(
        string? type,
        CancellationToken cancellationToken = default
    )
    {
        return _visitRepository
            .GetWaitingListAsync(
                type,
                cancellationToken
            );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<VisitResponse> GetVisitByIdAsync(
        long id,
        string? type,
        CancellationToken cancellationToken = default
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                type
            )
        )
        {
            throw new BadRequestException(
                "Cần truyền type=EXAMINATION hoặc type=TEST vì ID lượt của hai bảng có thể trùng nhau."
            );
        }

        var normalizedType =
            NormalizeType(
                type
            );

        var visit =
            await _visitRepository
                .GetVisitAsync(
                    id,
                    normalizedType,
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy lượt {id}."
            );

        return MapResponse(
            visit
        );
    }

    // =====================================================
    // STATUS
    // =====================================================

    public async Task<VisitResponse> UpdateStatusAsync(
        long id,
        string type,
        VisitStatusRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedType =
            NormalizeType(
                type
            );

        var status =
            NormalizeStatus(
                normalizedType,
                request.Status
            );

        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            );

        var visit =
            await _visitRepository
                .UpdateStatusAsync(
                    id,
                    normalizedType,
                    status,
                    NormalizeNullable(
                        request.Note
                    ),
                    actor?.UserId,
                    cancellationToken
                );

        return MapResponse(
            visit
        );
    }

    // =====================================================
    // DOCTOR QUEUE
    // =====================================================

    public async Task<List<DoctorQueueItemResponse>>
        GetDoctorWaitingQueueAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        return await _visitRepository
            .GetDoctorQueueAsync(
                actor.DoctorId!.Value,
                cancellationToken
            );
    }

    // =====================================================
    // DOCTOR CALL
    // =====================================================

    public async Task CallPatientAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        await _visitRepository
            .CallDoctorPatientAsync(
                id,
                actor.DoctorId!.Value,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // DOCTOR HOLD / SKIP
    // =====================================================

    public async Task HoldPatientAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        await _visitRepository
            .HoldDoctorPatientAsync(
                id,
                actor.DoctorId!.Value,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // START EXAM
    // =====================================================

    public async Task<long> StartExamAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await RequireDoctorAsync(
                user,
                cancellationToken
            );

        return await _visitRepository
            .StartExamAsync(
                id,
                actor.DoctorId!.Value,
                actor.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // RECEPTION QUEUE ACTIONS
    // =====================================================

    public async Task CallByAppointmentAsync(
        string appointmentId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            );

        await _visitRepository
            .CallByAppointmentAsync(
                appointmentId,
                actor?.UserId,
                cancellationToken
            );
    }

    public async Task HoldByAppointmentAsync(
        string appointmentId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            );

        await _visitRepository
            .HoldByAppointmentAsync(
                appointmentId,
                actor?.UserId,
                cancellationToken
            );
    }

    public async Task SkipByAppointmentAsync(
        string appointmentId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            );

        await _visitRepository
            .SkipByAppointmentAsync(
                appointmentId,
                actor?.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // WALK-IN
    // =====================================================

    public async Task<WalkInVisitResponse> CreateWalkInAsync(
        WalkInVisitRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var fullName =
            Require(
                request.FullName,
                "Họ tên không được để trống."
            );

        var phone =
            Require(
                request.Phone,
                "Số điện thoại không được để trống."
            );

        var type =
            NormalizeType(
                request.ServiceType
            );

        if (
            type ==
            "EXAMINATION"
            &&
            string.IsNullOrWhiteSpace(
                request.SpecialtyId
            )
        )
        {
            throw new BadRequestException(
                "Vui lòng chọn chuyên khoa."
            );
        }

        if (
            type ==
            "TEST"
            &&
            string.IsNullOrWhiteSpace(
                request.TestId
            )
        )
        {
            throw new BadRequestException(
                "Vui lòng chọn loại xét nghiệm."
            );
        }

        var birthDate =
            ParseNullableDate(
                request.DateOfBirth
            );

        var actor =
            await ResolveActorAsync(
                user,
                cancellationToken
            );

        var data =
            new WalkInVisitData(
                fullName,
                phone,
                NormalizeNullable(
                    request.Email
                ),
                birthDate,
                NormalizeGender(
                    request.Gender
                ),
                NormalizeNullable(
                    request.Address
                ),
                type,
                NormalizeNullable(
                    request.SpecialtyId
                ),
                NormalizeNullable(
                    request.TestId
                ),
                NormalizeNullable(
                    request.Reason
                ),
                NormalizeNullable(
                    request.Notes
                )
            );

        return await _visitRepository
            .CreateWalkInAsync(
                data,
                actor?.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // ACTOR
    // =====================================================

    private async Task<VisitActorContext?>
        ResolveActorAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken
        )
    {
        if (
            user.Identity?.IsAuthenticated
            != true
        )
        {
            return null;
        }

        var login =
            user.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            )
            ??
            user.FindFirstValue(
                ClaimTypes.Email
            )
            ??
            user.Identity.Name;

        if (
            string.IsNullOrWhiteSpace(
                login
            )
        )
        {
            return null;
        }

        return await _visitRepository
            .GetActorAsync(
                login,
                cancellationToken
            );
    }

    private async Task<VisitActorContext> RequireDoctorAsync(
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

        if (
            !actor.DoctorId.HasValue
        )
        {
            throw new ForbiddenException(
                "Tài khoản chưa liên kết bác sĩ."
            );
        }

        return actor;
    }

    // =====================================================
    // STATUS MAPPING
    // =====================================================

    private static string NormalizeStatus(
        string type,
        string? status
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                status
            )
        )
        {
            throw new BadRequestException(
                "Trạng thái không được để trống."
            );
        }

        var value =
            status.Trim()
                .ToUpperInvariant();

        if (
            type ==
            "EXAMINATION"
        )
        {
            return value switch
            {
                "WAITING" =>
                    "cho_kham",

                "CALLED" =>
                    "da_den_luot",

                "ON_HOLD" =>
                    "cho_goi_lai",

                "IN_PROGRESS" =>
                    "dang_kham",

                "WAITING_TEST" =>
                    "da_chi_dinh_xn",

                "WAITING_RESULT" =>
                    "moi_doc_kq",

                "RESULT_READY" =>
                    "moi_doc_kq",

                "COMPLETED" =>
                    "hoan_tat",

                "CANCELLED" =>
                    "bo_luot",

                "DA_TIEP_NHAN" =>
                    "da_tiep_nhan",

                "CHO_KHAM" =>
                    "cho_kham",

                "DA_DEN_LUOT" =>
                    "da_den_luot",

                "CHO_GOI_LAI" =>
                    "cho_goi_lai",

                "DANG_KHAM" =>
                    "dang_kham",

                "DA_CHI_DINH_XN" =>
                    "da_chi_dinh_xn",

                "MOI_DOC_KQ" =>
                    "moi_doc_kq",

                "DANG_TU_VAN_KQ" =>
                    "dang_tu_van_kq",

                "BO_LUOT" =>
                    "bo_luot",

                "HOAN_TAT" =>
                    "hoan_tat",

                _ =>
                    throw new BadRequestException(
                        "Trạng thái lượt khám không hợp lệ."
                    )
            };
        }

        return value switch
        {
            "WAITING" =>
                "cho_xet_nghiem",

            "CALLED" =>
                "da_den_luot",

            "ON_HOLD" =>
                "cho_xet_nghiem",

            "IN_PROGRESS" =>
                "dang_xet_nghiem",

            "WAITING_TEST" =>
                "dang_lay_mau",

            "WAITING_RESULT" =>
                "cho_duyet_kq",

            "RESULT_READY" =>
                "da_co_kq",

            "COMPLETED" =>
                "hoan_tat",

            "CANCELLED" =>
                "huy",

            "DA_TIEP_NHAN" =>
                "da_tiep_nhan",

            "CHO_XET_NGHIEM" =>
                "cho_xet_nghiem",

            "DA_DEN_LUOT" =>
                "da_den_luot",

            "DANG_LAY_MAU" =>
                "dang_lay_mau",

            "DA_LAY_MAU" =>
                "da_lay_mau",

            "KTV_TIEP_NHAN" =>
                "ktv_tiep_nhan",

            "DANG_XET_NGHIEM" =>
                "dang_xet_nghiem",

            "CHO_DUYET_KQ" =>
                "cho_duyet_kq",

            "DA_CO_KQ" =>
                "da_co_kq",

            "MOI_DOC_KQ" =>
                "moi_doc_kq",

            "DANG_TU_VAN_KQ" =>
                "dang_tu_van_kq",

            "HOAN_TAT" =>
                "hoan_tat",

            "HUY" =>
                "huy",

            _ =>
                throw new BadRequestException(
                    "Trạng thái lượt xét nghiệm không hợp lệ."
                )
        };
    }

    // =====================================================
    // OTHER HELPERS
    // =====================================================

    private static VisitResponse MapResponse(
        Visit visit
    )
    {
        return new VisitResponse
        {
            Id =
                visit.Id,

            AppointmentId =
                visit.AppointmentId,

            CustomerId =
                visit.CustomerId,

            DoctorId =
                visit.DoctorId,

            RoomId =
                visit.RoomId,

            Type =
                visit.Type,

            QueueNumber =
                visit.QueueNumber,

            Status =
                visit.Status,

            ReceivedAt =
                visit.ReceivedAt,

            StartedAt =
                visit.StartedAt,

            EndedAt =
                visit.EndedAt,

            Note =
                visit.Note
        };
    }

    private static string NormalizeType(
        string? type
    )
    {
        return type?
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "EXAMINATION" =>
                "EXAMINATION",

            "EXAM" =>
                "EXAMINATION",

            "TEST" =>
                "TEST",

            "TESTING" =>
                "TEST",

            _ =>
                throw new BadRequestException(
                    "Loại dịch vụ không hợp lệ."
                )
        };
    }

    private static DateTime? ParseNullableDate(
        string? value
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                value
            )
        )
        {
            return null;
        }

        if (
            !DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date
            )
        )
        {
            throw new BadRequestException(
                "Ngày sinh phải có định dạng yyyy-MM-dd."
            );
        }

        return date.Date;
    }

    private static string NormalizeGender(
        string? value
    )
    {
        var gender =
            value?
                .Trim()
                .ToLowerInvariant();

        return gender switch
        {
            "nam" =>
                "nam",

            "nữ" =>
                "nu",

            "nu" =>
                "nu",

            "khác" =>
                "khac",

            "khac" =>
                "khac",

            _ =>
                "khac"
        };
    }

    private static string Require(
        string? value,
        string message
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                value
            )
        )
        {
            throw new BadRequestException(
                message
            );
        }

        return value.Trim();
    }

    private static string? NormalizeNullable(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(
            value
        )
            ? null
            : value.Trim();
    }
}