using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class WorkScheduleService
    : IWorkScheduleService
{
    private readonly IWorkScheduleRepository
        _repository;

    public WorkScheduleService(
        IWorkScheduleRepository repository
    )
    {
        _repository =
            repository;
    }

    // =====================================================
    // PUBLIC DOCTOR SCHEDULE
    // =====================================================

    public async Task<List<WorkScheduleResponse>>
        GetDoctorScheduleAsync(
            int doctorId,
            string? date,
            CancellationToken cancellationToken = default
        )
    {
        if (
            !await _repository
                .DoctorExistsAsync(
                    doctorId,
                    cancellationToken
                )
        )
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy bác sĩ: {doctorId}"
            );
        }

        DateTime? parsedDate = null;

        if (
            !string.IsNullOrWhiteSpace(
                date
            )
        )
        {
            parsedDate =
                ParseDate(date);
        }

        return await _repository
            .GetDoctorSchedulesAsync(
                doctorId,
                parsedDate,
                cancellationToken
            );
    }

    // =====================================================
    // ADMIN LIST
    // =====================================================

    public Task<List<WorkScheduleResponse>>
        GetSchedulesAsync(
            string? query,
            CancellationToken cancellationToken = default
        )
    {
        return _repository
            .GetAllAsync(
                query,
                cancellationToken
            );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<WorkScheduleResponse> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            await _repository
                .GetResponseByIdAsync(
                    id,
                    cancellationToken
                );

        if (result is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy lịch làm việc: {id}"
            );
        }

        return result;
    }

    // =====================================================
    // CREATE
    // =====================================================

    public async Task<WorkScheduleResponse> CreateAsync(
        WorkScheduleRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var normalized =
            await ValidateAndNormalizeAsync(
                request,
                null,
                cancellationToken
            );

        int? creatorUserId = null;

        var login =
            user.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            )
            ??
            user.Identity?.Name;

        if (
            !string.IsNullOrWhiteSpace(
                login
            )
        )
        {
            creatorUserId =
                await _repository
                    .FindUserIdByLoginAsync(
                        login,
                        cancellationToken
                    );
        }

        var schedule =
            new WorkSchedule
            {
                DoctorId =
                    request.IdBacSi,

                RoomId =
                    normalized.RoomId,

                WorkDate =
                    normalized.Date,

                Shift =
                    normalized.Shift,

                StartTime =
                    normalized.Start,

                EndTime =
                    normalized.End,

                /*
                 * Route này chỉ dành cho ADMIN.
                 */
                Source =
                    "admin",

                CreatedByUserId =
                    creatorUserId,

                Status =
                    normalized.Status,

                Note =
                    NormalizeNullable(
                        request.GhiChu
                    )
            };

        schedule.Id =
            await _repository
                .CreateAsync(
                    schedule,
                    cancellationToken
                );

        return (
            await _repository
                .GetResponseByIdAsync(
                    schedule.Id,
                    cancellationToken
                )
        )
        ?? throw new InvalidOperationException(
            "Không đọc được lịch vừa tạo."
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task<WorkScheduleResponse> UpdateAsync(
        long id,
        WorkScheduleRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var existing =
            await _repository
                .FindByIdAsync(
                    id,
                    cancellationToken
                );

        if (existing is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy lịch làm việc: {id}"
            );
        }

        var normalized =
            await ValidateAndNormalizeAsync(
                request,
                id,
                cancellationToken
            );

        existing.DoctorId =
            request.IdBacSi;

        existing.RoomId =
            normalized.RoomId;

        existing.WorkDate =
            normalized.Date;

        existing.Shift =
            normalized.Shift;

        existing.StartTime =
            normalized.Start;

        existing.EndTime =
            normalized.End;

        existing.Status =
            normalized.Status;

        existing.Note =
            NormalizeNullable(
                request.GhiChu
            );

        await _repository
            .UpdateAsync(
                existing,
                cancellationToken
            );

        return (
            await _repository
                .GetResponseByIdAsync(
                    id,
                    cancellationToken
                )
        )
        ?? throw new ResourceNotFoundException(
            $"Không tìm thấy lịch làm việc: {id}"
        );
    }

    // =====================================================
    // DELETE = CANCEL
    // =====================================================

    public async Task DeleteAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var existing =
            await _repository
                .FindByIdAsync(
                    id,
                    cancellationToken
                );

        if (existing is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy lịch làm việc: {id}"
            );
        }

        /*
         * Không DELETE lịch sử.
         */
        existing.Status =
            "huy";

        await _repository
            .UpdateAsync(
                existing,
                cancellationToken
            );
    }

    // =====================================================
    // VALIDATE + NORMALIZE
    // =====================================================

    private async Task<NormalizedSchedule>
        ValidateAndNormalizeAsync(
            WorkScheduleRequest request,
            long? excludeScheduleId,
            CancellationToken cancellationToken
        )
    {
        if (request.IdBacSi <= 0)
        {
            throw new BadRequestException(
                "Mã bác sĩ không hợp lệ."
            );
        }

        if (
            !await _repository
                .DoctorExistsAsync(
                    request.IdBacSi,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Bác sĩ {request.IdBacSi} không tồn tại hoặc đã ngừng hoạt động."
            );
        }

        var date =
            ParseDate(
                request.NgayLamViec
            );

        var start =
            ParseTime(
                request.GioBatDau,
                "Giờ bắt đầu"
            );

        var end =
            ParseTime(
                request.GioKetThuc,
                "Giờ kết thúc"
            );

        /*
         * DB cũng có CHECK:
         * GioBatDau < GioKetThuc
         */
        if (start >= end)
        {
            throw new BadRequestException(
                "Giờ bắt đầu phải nhỏ hơn giờ kết thúc."
            );
        }

        var roomId =
            NormalizeNullable(
                request.IdPhong
            );

        if (
            roomId is not null
            &&
            !await _repository
                .RoomExistsAndActiveAsync(
                    roomId,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Phòng {roomId} không tồn tại hoặc không hoạt động."
            );
        }

        var status =
            NormalizeStatus(
                request.TrangThai
            );

        /*
         * Lịch hủy không cần kiểm tra collision.
         */
        if (status == "duoc_duyet")
        {
            var doctorOverlap =
                await _repository
                    .HasDoctorOverlapAsync(
                        request.IdBacSi,
                        date,
                        start,
                        end,
                        excludeScheduleId,
                        cancellationToken
                    );

            if (doctorOverlap)
            {
                throw new BadRequestException(
                    "Bác sĩ đã có lịch làm việc trùng khoảng thời gian này."
                );
            }

            if (roomId is not null)
            {
                var roomOverlap =
                    await _repository
                        .HasRoomOverlapAsync(
                            roomId,
                            date,
                            start,
                            end,
                            excludeScheduleId,
                            cancellationToken
                        );

                if (roomOverlap)
                {
                    throw new BadRequestException(
                        $"Phòng {roomId} đã được sử dụng trong khoảng thời gian này."
                    );
                }
            }
        }

        return new NormalizedSchedule(
            date,
            start,
            end,
            NormalizeShift(
                request.CaLamViec
            ),
            roomId,
            status
        );
    }

    // =====================================================
    // DATE
    // =====================================================

    private static DateTime ParseDate(
        string value
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
                "Ngày làm việc phải có định dạng yyyy-MM-dd."
            );
        }

        return date.Date;
    }

    // =====================================================
    // TIME
    // =====================================================

    private static TimeSpan ParseTime(
        string value,
        string fieldName
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
                out var result
            )
        )
        {
            throw new BadRequestException(
                $"{fieldName} không hợp lệ."
            );
        }

        return result;
    }

    // =====================================================
    // SHIFT
    // =====================================================

    private static string NormalizeShift(
        string? shift
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                shift
            )
        )
        {
            return "TuyChinh";
        }

        return shift
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "MORNING" or
            "SANG" =>
                "Sang",

            "AFTERNOON" or
            "CHIEU" =>
                "Chieu",

            "EVENING" or
            "TOI" =>
                "Toi",

            /*
             * DB không có FULL_DAY.
             * Vì giờ đầu/cuối đã lưu riêng,
             * coi đây là lịch tùy chỉnh.
             */
            "FULL_DAY" or
            "CUSTOM" or
            "TUYCHINH" =>
                "TuyChinh",

            _ =>
                throw new BadRequestException(
                    $"Ca làm việc '{shift}' không hợp lệ."
                )
        };
    }

    // =====================================================
    // STATUS
    // =====================================================

    private static string NormalizeStatus(
        string? status
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                status
            )
        )
        {
            return "duoc_duyet";
        }

        return status
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "ACTIVE" or
            "APPROVED" or
            "DUOC_DUYET" =>
                "duoc_duyet",

            "CANCELLED" or
            "HUY" =>
                "huy",

            _ =>
                throw new BadRequestException(
                    $"Trạng thái lịch '{status}' không hợp lệ."
                )
        };
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

    private sealed record NormalizedSchedule(
        DateTime Date,
        TimeSpan Start,
        TimeSpan End,
        string Shift,
        string? RoomId,
        string Status
    );
}