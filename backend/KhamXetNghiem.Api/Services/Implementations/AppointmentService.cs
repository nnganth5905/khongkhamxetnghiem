using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Repositories.Models;
using KhamXetNghiem.Api.Services.Interfaces;
using KhamXetNghiem.Api.Utilities;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class AppointmentService
    : IAppointmentService
{
    private readonly IAppointmentRepository
        _appointmentRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository
    )
    {
        _appointmentRepository =
            appointmentRepository;
    }

    // =====================================================
    // OPTIONS
    // =====================================================

    public Task<AppointmentOptionsResponse> GetOptionsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return _appointmentRepository
            .GetOptionsAsync(
                cancellationToken
            );
    }

    // =====================================================
    // OLD TAKEN-TIMES API
    // =====================================================

    public async Task<List<string>> GetTakenTimesAsync(
        string type,
        string doctorId,
        string date,
        CancellationToken cancellationToken = default
    )
    {
        var id =
            ParseDoctorId(
                doctorId
            );

        var parsedDate =
            ParseDate(
                date
            );

        return await _appointmentRepository
            .GetTakenTimesAsync(
                NormalizeType(type),
                id,
                parsedDate,
                cancellationToken
            );
    }

    // =====================================================
    // REAL AVAILABLE SLOTS
    // =====================================================

    public async Task<AvailableSlotsResponse>
        GetAvailableSlotsAsync(
            string type,
            string doctorId,
            string date,
            string? excludeAppointmentId,
            CancellationToken cancellationToken = default
        )
    {
        _ =
            NormalizeType(
                type
            );

        var id =
            ParseDoctorId(
                doctorId
            );

        var parsedDate =
            ParseDate(
                date
            );

        var doctor =
            await GetActiveDoctorAsync(
                id,
                cancellationToken
            );

        var windows =
            await _appointmentRepository
                .GetDoctorWorkWindowsAsync(
                    doctor.Id,
                    parsedDate,
                    cancellationToken
                );

        var takenTimes =
            await _appointmentRepository
                .GetDoctorOccupiedTimesAsync(
                    doctor.Id,
                    parsedDate,
                    NormalizeNullable(
                        excludeAppointmentId
                    ),
                    cancellationToken
                );

        var taken =
            new HashSet<string>(
                takenTimes,
                StringComparer.OrdinalIgnoreCase
            );

        var slots =
            new SortedSet<string>();

        foreach (var window in windows)
        {
            var current =
                window.Start;

            /*
             * Slot 60 phút giống UI cũ,
             * nhưng chỉ sinh slot nằm trong lịch làm việc.
             *
             * Ví dụ 08:00-12:00:
             * 08,09,10,11.
             * Không cho đặt 12:00 vì đã hết ca.
             */
            while (current < window.End)
            {
                var slot =
                    FormatTime(
                        current
                    );

                if (
                    !taken.Contains(
                        slot
                    )
                    &&
                    !IsPastSlot(
                        parsedDate,
                        current
                    )
                )
                {
                    slots.Add(
                        slot
                    );
                }

                current =
                    current.Add(
                        TimeSpan.FromHours(1)
                    );
            }
        }

        return new AvailableSlotsResponse
        {
            DoctorId =
                doctor.Id,

            Date =
                parsedDate.ToString(
                    "yyyy-MM-dd"
                ),

            Slots =
                slots.ToList(),

            TakenTimes =
                takenTimes
        };
    }

    // =====================================================
    // CREATE EXAMINATION
    // =====================================================

    public async Task<CreateAppointmentResponse>
        CreateExaminationAsync(
            ExaminationAppointmentRequest request,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var doctorId =
            ParseDoctorId(
                request.Idbacsi
            );

        var doctor =
            await GetActiveDoctorAsync(
                doctorId,
                cancellationToken
            );

        var specialtyId =
            Require(
                request.Idchuyenkhoa,
                "Chuyên khoa không được để trống."
            );

        if (
            !string.Equals(
                doctor.SpecialtyId,
                specialtyId,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BadRequestException(
                "Bác sĩ không thuộc chuyên khoa đã chọn."
            );
        }

        var date =
            ParseDate(
                request.Ngay
            );

        var time =
            ParseTime(
                request.Gio
            );

        ValidateFutureSlot(
            date,
            time
        );

        var facilityId =
            await ResolveFacilityAsync(
                request.Idcoso,
                doctor,
                cancellationToken
            );

        var account =
            await ResolveAccountAsync(
                user,
                cancellationToken
            );

        var customer =
            await ResolveCustomerAsync(
                account,
                request.Hoten,
                request.Sodienthoai,
                request.Email,
                request.Gioitinh,
                request.Ngaysinh,
                cancellationToken
            );

        var code =
            GenerateCode(
                "DLK"
            );

        var qrCode =
            GenerateQrCode();

        var note =
            BuildExaminationNote(
                request.Lydokham,
                request.Ghichu
            );

        var created =
            await _appointmentRepository
                .CreateExaminationAsync(
                    new NewExaminationBooking(
                        code,
                        qrCode,
                        account?.UserId,
                        customer.Id,
                        specialtyId,
                        doctor.Id,
                        facilityId,
                        date,
                        time,
                        note
                    ),
                    cancellationToken
                );

        return new CreateAppointmentResponse
        {
            Id =
                created.Id,

            AppointmentCode =
                created.Code,

            Type =
                "EXAMINATION",

            QrCode =
                created.QrCode,

            Message =
                "Đã lưu lịch khám thành công."
        };
    }

    // =====================================================
    // CREATE TEST
    // =====================================================

    public async Task<CreateAppointmentResponse>
        CreateTestAsync(
            TestAppointmentRequest request,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var doctorId =
            ParseDoctorId(
                request.Idbacsi
            );

        var doctor =
            await GetActiveDoctorAsync(
                doctorId,
                cancellationToken
            );

        var tests =
            await ResolveTestsAsync(
                request.Idxetnghiem,
                cancellationToken
            );

        foreach (var test in tests)
        {
            if (
                !string.Equals(
                    test.SpecialtyId,
                    doctor.SpecialtyId,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    $"Xét nghiệm {test.Name} không thuộc chuyên khoa của bác sĩ đã chọn."
                );
            }
        }

        var date =
            ParseDate(
                request.Ngay
            );

        var time =
            ParseTime(
                request.Gio
            );

        ValidateFutureSlot(
            date,
            time
        );

        var facilityId =
            await ResolveFacilityAsync(
                request.Idcoso,
                doctor,
                cancellationToken
            );

        var account =
            await ResolveAccountAsync(
                user,
                cancellationToken
            );

        var customer =
            await ResolveCustomerAsync(
                account,
                request.Hoten,
                request.Sodienthoai,
                request.Email,
                request.Gioitinh,
                request.Ngaysinh,
                cancellationToken
            );

        var code =
            GenerateCode(
                "DLXN"
            );

        var qrCode =
            GenerateQrCode();

        var created =
            await _appointmentRepository
                .CreateTestAsync(
                    new NewTestBooking(
                        code,
                        qrCode,
                        account?.UserId,
                        customer.Id,
                        doctor.Id,
                        facilityId,
                        date,
                        time,
                        NormalizeNullable(
                            request.Ghichu
                        ),
                        tests
                    ),
                    cancellationToken
                );

        return new CreateAppointmentResponse
        {
            Id =
                created.Id,

            AppointmentCode =
                created.Code,

            Type =
                "TEST",

            QrCode =
                created.QrCode,

            Message =
                "Đã lưu lịch xét nghiệm thành công."
        };
    }

    // =====================================================
    // QUICK APPOINTMENT
    // =====================================================

    public async Task<CreateAppointmentResponse>
        CreateQuickAsync(
            QuickAppointmentRequest request,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var account =
            await ResolveAccountAsync(
                user,
                cancellationToken
            )
            ?? throw new ForbiddenException(
                "Đặt lịch nhanh yêu cầu tài khoản khách hàng."
            );

        if (
            string.IsNullOrWhiteSpace(
                account.CustomerId
            )
        )
        {
            throw new BadRequestException(
                "Tài khoản chưa liên kết hồ sơ khách hàng."
            );
        }

        var customer =
            await _appointmentRepository
                .FindCustomerByIdAsync(
                    account.CustomerId,
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                "Không tìm thấy hồ sơ khách hàng."
            );

        var doctor =
            await GetActiveDoctorAsync(
                ParseDoctorId(
                    request.Idbacsi
                ),
                cancellationToken
            );

        var date =
            ParseDate(
                request.Ngay
            );

        var time =
            ParseTime(
                request.Gio
            );

        ValidateFutureSlot(
            date,
            time
        );

        var facilityId =
            await ResolveFacilityAsync(
                null,
                doctor,
                cancellationToken
            );

        var code =
            GenerateCode(
                "DLK"
            );

        var qr =
            GenerateQrCode();

        var result =
            await _appointmentRepository
                .CreateExaminationAsync(
                    new NewExaminationBooking(
                        code,
                        qr,
                        account.UserId,
                        customer.Id,
                        doctor.SpecialtyId,
                        doctor.Id,
                        facilityId,
                        date,
                        time,
                        NormalizeNullable(
                            request.Ghichu
                        )
                    ),
                    cancellationToken
                );

        return new CreateAppointmentResponse
        {
            Id = result.Id,
            AppointmentCode = result.Code,
            Type = "EXAMINATION",
            QrCode = result.QrCode,
            Message = "Đặt lịch nhanh thành công."
        };
    }

    // =====================================================
    // MY
    // =====================================================

    public async Task<List<AppointmentResponse>>
        GetMyAppointmentsAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var account =
            await ResolveAccountAsync(
                user,
                cancellationToken
            );

        if (account is null)
        {
            return new List<AppointmentResponse>();
        }

        return await _appointmentRepository
            .GetMyAppointmentsAsync(
                account.UserId,
                account.CustomerId,
                cancellationToken
            );
    }

    // =====================================================
    // ALL
    // =====================================================

    public async Task<List<AppointmentResponse>>
        GetAllAppointmentsAsync(
            string? date,
            CancellationToken cancellationToken = default
        )
    {
        var target =
            string.IsNullOrWhiteSpace(
                date
            )
                ? VietnamTime.Now.Date
                : ParseDate(date);

        return await _appointmentRepository
            .GetAllAppointmentsAsync(
                target,
                cancellationToken
            );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<AppointmentDetailResponse> GetDetailAsync(
        string id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var code =
            Require(
                id,
                "Mã lịch hẹn không hợp lệ."
            );

        var edit =
            await _appointmentRepository
                .GetEditDataAsync(
                    code,
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy lịch hẹn: {code}"
            );

        await EnsureCanManageAppointmentAsync(
            edit,
            user,
            cancellationToken
        );

        return await _appointmentRepository
            .GetDetailAsync(
                code,
                cancellationToken
            )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy lịch hẹn: {code}"
            );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task<AppointmentDetailResponse> UpdateAsync(
        string id,
        UpdateAppointmentRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var current =
            await _appointmentRepository
                .GetEditDataAsync(
                    id,
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy lịch hẹn: {id}"
            );

        var account =
            await EnsureCanManageAppointmentAsync(
                current,
                user,
                cancellationToken
            );

        if (
            current.Status is
                "checked_in"
                or "completed"
                or "cancelled"
                or "no_show"
        )
        {
            throw new BadRequestException(
                "Lịch hẹn hiện tại không thể thay đổi."
            );
        }

        var date =
            ParseDate(
                request.NewDate
            );

        var time =
            ParseTime(
                request.NewTime
            );

        ValidateFutureSlot(
            date,
            time
        );

        var doctorId =
            string.IsNullOrWhiteSpace(
                request.NewDoctorId
            )
                ? current.DoctorId
                    ?? throw new BadRequestException(
                        "Lịch hẹn chưa có bác sĩ."
                    )
                : ParseDoctorId(
                    request.NewDoctorId
                );

        var doctor =
            await GetActiveDoctorAsync(
                doctorId,
                cancellationToken
            );

        if (
            current.Type == "EXAMINATION"
            &&
            !string.Equals(
                doctor.SpecialtyId,
                current.SpecialtyId,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BadRequestException(
                "Bác sĩ mới không thuộc chuyên khoa của lịch khám."
            );
        }

        if (current.Type == "TEST")
        {
            var testIds =
                await _appointmentRepository
                    .GetTestIdsAsync(
                        current.Code,
                        cancellationToken
                    );

            foreach (var testId in testIds)
            {
                var test =
                    await _appointmentRepository
                        .FindTestAsync(
                            testId,
                            cancellationToken
                        );

                if (
                    test is not null
                    &&
                    !string.Equals(
                        test.SpecialtyId,
                        doctor.SpecialtyId,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new BadRequestException(
                        "Bác sĩ mới không phù hợp với xét nghiệm đã đăng ký."
                    );
                }
            }
        }

        await _appointmentRepository
            .UpdateAppointmentAsync(
                current,
                doctor.Id,
                date,
                time,
                NormalizeNullable(
                    request.Note
                ),
                account?.UserId,
                cancellationToken
            );

        return await _appointmentRepository
            .GetDetailAsync(
                current.Code,
                cancellationToken
            )
            ?? throw new ResourceNotFoundException(
                "Không đọc được lịch sau khi cập nhật."
            );
    }

    // =====================================================
    // CANCEL
    // =====================================================

    public async Task CancelAsync(
        string id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var current =
            await _appointmentRepository
                .GetEditDataAsync(
                    id,
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy lịch hẹn: {id}"
            );

        var account =
            await EnsureCanManageAppointmentAsync(
                current,
                user,
                cancellationToken
            );

        if (
            current.Status is
                "checked_in"
                or "completed"
        )
        {
            throw new BadRequestException(
                "Không thể hủy lịch đã check-in hoặc hoàn thành."
            );
        }

        if (
            current.Status is
                "cancelled"
                or "no_show"
        )
        {
            throw new BadRequestException(
                "Lịch hẹn đã kết thúc hoặc đã hủy."
            );
        }

        await _appointmentRepository
            .CancelAppointmentAsync(
                current,
                account?.UserId,
                cancellationToken
            );
    }

    // =====================================================
    // NO SHOW
    // =====================================================

    public Task<int> MarkExpiredNoShowsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return _appointmentRepository
            .MarkExpiredAppointmentsNoShowAsync(
                VietnamTime.Now.Date,
                cancellationToken
            );
    }

    // =====================================================
    // CUSTOMER
    // =====================================================

    private async Task<AppointmentCustomerData>
        ResolveCustomerAsync(
            AppointmentAccountContext? account,
            string? fullName,
            string? phone,
            string? email,
            string? gender,
            string? dob,
            CancellationToken cancellationToken
        )
    {
        var name =
            Require(
                fullName,
                "Họ tên không được để trống."
            );

        var normalizedPhone =
            Require(
                phone,
                "Số điện thoại không được để trống."
            );

        /*
         * Nếu thông tin gửi lên trùng hồ sơ đang đăng nhập
         * => sử dụng hồ sơ đó.
         *
         * Nếu khác => đây có thể là "đặt cho người khác".
         */
        if (
            account is not null
            &&
            !string.IsNullOrWhiteSpace(
                account.CustomerId
            )
        )
        {
            var linked =
                await _appointmentRepository
                    .FindCustomerByIdAsync(
                        account.CustomerId,
                        cancellationToken
                    );

            if (
                linked is not null
                &&
                string.Equals(
                    linked.FullName.Trim(),
                    name.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                string.Equals(
                    linked.Phone?.Trim(),
                    normalizedPhone.Trim(),
                    StringComparison.Ordinal
                )
            )
            {
                return linked;
            }
        }

        var existing =
            await _appointmentRepository
                .FindCustomerAsync(
                    name,
                    normalizedPhone,
                    cancellationToken
                );

        if (existing is not null)
        {
            return existing;
        }

        var birthDate =
            ParseNullableDate(
                dob
            );

        var normalizedGender =
            NormalizeGender(
                gender
            );

        /*
         * khachhang.IDKhachHang varchar(10).
         * KH + 8 hex = đúng 10 ký tự.
         */
        var customerId =
            "KH"
            +
            Guid.NewGuid()
                .ToString("N")[..8]
                .ToUpperInvariant();

        return await _appointmentRepository
            .CreateCustomerAsync(
                customerId,
                name,
                normalizedPhone,
                NormalizeNullable(email),
                birthDate,
                normalizedGender,
                cancellationToken
            );
    }

    // =====================================================
    // AUTH
    // =====================================================

    private async Task<AppointmentAccountContext?>
        ResolveAccountAsync(
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
            user.Identity.Name;

        if (
            string.IsNullOrWhiteSpace(
                login
            )
        )
        {
            return null;
        }

        return await _appointmentRepository
            .FindAccountAsync(
                login,
                cancellationToken
            );
    }

    private async Task<AppointmentAccountContext?>
        EnsureCanManageAppointmentAsync(
            AppointmentEditData appointment,
            ClaimsPrincipal user,
            CancellationToken cancellationToken
        )
    {
        if (
            user.IsInRole("ADMIN")
            ||
            user.IsInRole("RECEPTIONIST")
            ||
            user.IsInRole("DOCTOR")
        )
        {
            return await ResolveAccountAsync(
                user,
                cancellationToken
            );
        }

        var account =
            await ResolveAccountAsync(
                user,
                cancellationToken
            );

        if (account is null)
        {
            throw new ForbiddenException(
                "Bạn không có quyền xem lịch hẹn này."
            );
        }

        var ownsByUser =
            appointment.UserId.HasValue
            &&
            appointment.UserId.Value
                == account.UserId;

        var ownsByCustomer =
            !string.IsNullOrWhiteSpace(
                account.CustomerId
            )
            &&
            string.Equals(
                account.CustomerId,
                appointment.CustomerId,
                StringComparison.OrdinalIgnoreCase
            );

        if (!ownsByUser && !ownsByCustomer)
        {
            throw new ForbiddenException(
                "Bạn không có quyền thao tác lịch hẹn này."
            );
        }

        return account;
    }

    // =====================================================
    // DOCTOR / FACILITY / TEST
    // =====================================================

    private async Task<AppointmentDoctorData>
        GetActiveDoctorAsync(
            int doctorId,
            CancellationToken cancellationToken
        )
    {
        var doctor =
            await _appointmentRepository
                .FindDoctorAsync(
                    doctorId,
                    cancellationToken
                )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy bác sĩ: {doctorId}"
            );

        if (
            !string.Equals(
                doctor.Status,
                "active",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BadRequestException(
                "Bác sĩ hiện không hoạt động."
            );
        }

        return doctor;
    }

    private async Task<string> ResolveFacilityAsync(
        string? requestFacilityId,
        AppointmentDoctorData doctor,
        CancellationToken cancellationToken
    )
    {
        var facilityId =
            NormalizeNullable(
                requestFacilityId
            )
            ??
            NormalizeNullable(
                doctor.FacilityId
            );

        if (facilityId is null)
        {
            throw new BadRequestException(
                "Chưa xác định được cơ sở khám."
            );
        }

        if (
            !await _appointmentRepository
                .IsFacilityActiveAsync(
                    facilityId,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Cơ sở {facilityId} không tồn tại hoặc không hoạt động."
            );
        }

        if (
            !string.IsNullOrWhiteSpace(
                doctor.FacilityId
            )
            &&
            !string.Equals(
                doctor.FacilityId,
                facilityId,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BadRequestException(
                "Bác sĩ không làm việc tại cơ sở đã chọn."
            );
        }

        return facilityId;
    }

    private async Task<List<AppointmentTestData>>
        ResolveTestsAsync(
            string? rawIds,
            CancellationToken cancellationToken
        )
    {
        var ids =
            Require(
                rawIds,
                "Vui lòng chọn xét nghiệm."
            )
            .Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries
                |
                StringSplitOptions.TrimEntries
            )
            .Distinct(
                StringComparer.OrdinalIgnoreCase
            )
            .ToList();

        var result =
            new List<AppointmentTestData>();

        foreach (var id in ids)
        {
            var test =
                await _appointmentRepository
                    .FindTestAsync(
                        id,
                        cancellationToken
                    )
                ?? throw new BadRequestException(
                    $"Xét nghiệm {id} không tồn tại hoặc đã ngừng hoạt động."
                );

            result.Add(test);
        }

        return result;
    }

    // =====================================================
    // VALIDATION
    // =====================================================

    private static DateTime ParseDate(
        string? value
    )
    {
        if (
            !DateTime.TryParseExact(
                value?.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date
            )
        )
        {
            throw new BadRequestException(
                "Ngày phải có định dạng yyyy-MM-dd."
            );
        }

        return date.Date;
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

        return ParseDate(
            value
        );
    }

    private static TimeSpan ParseTime(
        string? value
    )
    {
        var formats =
            new[]
            {
                @"hh\:mm",
                @"hh\:mm\:ss"
            };

        if (
            !TimeSpan.TryParseExact(
                value?.Trim(),
                formats,
                CultureInfo.InvariantCulture,
                out var time
            )
        )
        {
            throw new BadRequestException(
                "Giờ hẹn không hợp lệ."
            );
        }

        return time;
    }

    private static int ParseDoctorId(
        string? value
    )
    {
        if (
            !int.TryParse(
                value,
                out var doctorId
            )
            ||
            doctorId <= 0
        )
        {
            throw new BadRequestException(
                "Mã bác sĩ không hợp lệ."
            );
        }

        return doctorId;
    }

    private static void ValidateFutureSlot(
        DateTime date,
        TimeSpan time
    )
    {
        var now =
            VietnamTime.Now;

        if (date.Date < now.Date)
        {
            throw new BadRequestException(
                "Không thể đặt lịch trong quá khứ."
            );
        }

        if (
            date.Date == now.Date
            &&
            time <= now.TimeOfDay
        )
        {
            throw new BadRequestException(
                "Khung giờ đã qua."
            );
        }
    }

    private static bool IsPastSlot(
        DateTime date,
        TimeSpan time
    )
    {
        var now =
            VietnamTime.Now;

        return
            date.Date < now.Date
            ||
            (
                date.Date == now.Date
                &&
                time <= now.TimeOfDay
            );
    }

    private static string NormalizeType(
        string? type
    )
    {
        var value =
            type?.Trim()
                .ToUpperInvariant();

        return value switch
        {
            "EXAMINATION" =>
                "EXAMINATION",

            "TEST" =>
                "TEST",

            _ =>
                throw new BadRequestException(
                    "Loại lịch hẹn không hợp lệ."
                )
        };
    }

    private static string NormalizeGender(
        string? gender
    )
    {
        return gender?
            .Trim()
            .ToLowerInvariant()
        switch
        {
            "nam" => "nam",
            "nu" => "nu",
            "khac" => "khac",
            _ => "khac"
        };
    }

    private static string BuildExaminationNote(
        string? reason,
        string? note
    )
    {
        var r =
            NormalizeNullable(
                reason
            );

        var n =
            NormalizeNullable(
                note
            );

        var value =
            (r, n) switch
            {
                (not null, not null) =>
                    $"Lý do khám: {r}\nGhi chú: {n}",

                (not null, null) =>
                    $"Lý do khám: {r}",

                (null, not null) =>
                    n,

                _ =>
                    string.Empty
            };

        return value.Length <= 500
            ? value
            : value[..500];
    }

    private static string GenerateCode(
        string prefix
    )
    {
        return
            $"{prefix}-"
            +
            Guid.NewGuid()
                .ToString("N")[..12]
                .ToUpperInvariant();
    }

    private static string GenerateQrCode()
    {
        return
            "QR-"
            +
            Guid.NewGuid()
                .ToString("N")
                .ToUpperInvariant();
    }

    private static string FormatTime(
        TimeSpan time
    )
    {
        return
            $"{(int)time.TotalHours:00}:{time.Minutes:00}";
    }

    private static string Require(
        string? value,
        string error
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                value
            )
        )
        {
            throw new BadRequestException(
                error
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