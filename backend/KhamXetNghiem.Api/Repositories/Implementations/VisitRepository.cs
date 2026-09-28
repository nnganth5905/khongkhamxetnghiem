using System.Data;
using System.Data.Common;
using System.Globalization;

using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Repositories.Models;
using KhamXetNghiem.Api.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class VisitRepository : IVisitRepository
{
    private readonly AppDbContext _dbContext;

    public VisitRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // ACTOR
    // =====================================================

    public async Task<VisitActorContext?> GetActorAsync(
        string login,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    UserID,
                    IDBacSi,
                    IDNhanVien,
                    IDKhachHang
                FROM users
                WHERE
                    IsActive = 1
                    AND
                    (
                        Email = @login
                        OR Username = @login
                    )
                LIMIT 1
                """,
                cancellationToken,
                ("@login", login)
            );

        if (rows.Count == 0)
        {
            return null;
        }

        var row = rows[0];

        return new VisitActorContext(
            GetInt(row, "UserID")!.Value,
            GetInt(row, "IDBacSi"),
            GetString(row, "IDNhanVien"),
            GetString(row, "IDKhachHang")
        );
    }

    // =====================================================
    // FIND APPOINTMENT
    // =====================================================

    public async Task<VisitAppointmentSource?> FindAppointmentAsync(
        string appointmentCode,
        string? type,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedType =
            NormalizeTypeFromCodeOrInput(
                appointmentCode,
                type
            );

        return normalizedType == "EXAMINATION"
            ? await FindExaminationAppointmentAsync(
                appointmentCode,
                cancellationToken
            )
            : await FindTestAppointmentAsync(
                appointmentCode,
                cancellationToken
            );
    }

    public async Task<VisitAppointmentSource?> FindAppointmentByQrAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT *
                FROM
                (
                    SELECT
                        IDDatLichKham AS DbId,
                        MaDatLich AS Code,
                        'EXAMINATION' AS VisitType,
                        UserID AS BookedUserId,
                        IDKhachHang AS CustomerId,
                        IDBacSi AS DoctorId,
                        IDChuyenKhoa AS SpecialtyId,
                        CoSoID AS FacilityId,
                        NgayKham AS AppointmentDate,
                        GioKham AS AppointmentTime,
                        TrangThai AS AppointmentStatus,
                        GhiChu AS Note,
                        MaQR AS QrCode
                    FROM datlichkham
                    WHERE
                        MaQR = @code
                        OR MaDatLich = @code

                    UNION ALL

                    SELECT
                        IDDatLichXN AS DbId,
                        MaDatLich AS Code,
                        'TEST' AS VisitType,
                        UserID AS BookedUserId,
                        IDKhachHang AS CustomerId,
                        IDBacSi AS DoctorId,
                        NULL AS SpecialtyId,
                        CoSoID AS FacilityId,
                        NgayXetNghiem AS AppointmentDate,
                        GioXetNghiem AS AppointmentTime,
                        TrangThai AS AppointmentStatus,
                        GhiChu AS Note,
                        MaQR AS QrCode
                    FROM datlichxetnghiem
                    WHERE
                        MaQR = @code
                        OR MaDatLich = @code
                ) x
                LIMIT 2
                """,
                cancellationToken,
                ("@code", code)
            );

        if (rows.Count == 0)
        {
            return null;
        }

        if (rows.Count > 1)
        {
            throw new BadRequestException(
                "Mã QR không xác định duy nhất một lịch hẹn."
            );
        }

        return MapAppointment(
            rows[0]
        );
    }

    // =====================================================
    // CHECK-IN
    // =====================================================

    public async Task<CheckInResponse> CheckInAsync(
        VisitAppointmentSource appointment,
        int? actorUserId,
        string? note,
        CancellationToken cancellationToken = default
    )
    {
        var now =
            VietnamTime.Now;

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken
                );

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        try
        {
            var current =
                await FindAppointmentForUpdateAsync(
                    appointment,
                    connection,
                    dbTransaction,
                    cancellationToken
                )
                ?? throw new ResourceNotFoundException(
                    "Không tìm thấy lịch hẹn."
                );

            if (
                current.AppointmentDate.Date
                != now.Date
            )
            {
                throw new BadRequestException(
                    $"Lịch hẹn có ngày {current.AppointmentDate:dd/MM/yyyy}. Chỉ được check-in đúng ngày hẹn."
                );
            }

            if (
                current.Status is
                    "cancelled"
                    or "no_show"
                    or "completed"
            )
            {
                throw new BadRequestException(
                    $"Không thể check-in lịch có trạng thái {current.Status}."
                );
            }

            if (
                current.Status ==
                "checked_in"
            )
            {
                throw new BadRequestException(
                    "Lịch hẹn đã được check-in."
                );
            }

            var existing =
                await FindVisitByAppointmentDbIdAsync(
                    current,
                    connection,
                    dbTransaction,
                    true,
                    cancellationToken
                );

            if (existing is not null)
            {
                throw new BadRequestException(
                    "Lịch hẹn đã có lượt tiếp nhận. Không thể check-in lần hai."
                );
            }

            VisitRoomData? room;

            if (
                current.Type ==
                "EXAMINATION"
            )
            {
                room =
                    await FindExaminationRoomAsync(
                        current.FacilityId,
                        current.SpecialtyId,
                        connection,
                        dbTransaction,
                        cancellationToken
                    );
            }
            else
            {
                var specialty =
                    await GetTestAppointmentSpecialtyAsync(
                        current.DbId,
                        connection,
                        dbTransaction,
                        cancellationToken
                    );

                room =
                    await FindTestRoomAsync(
                        current.FacilityId,
                        specialty,
                        connection,
                        dbTransaction,
                        cancellationToken
                    );
            }

            var queueNumber =
                await GetNextQueueNumberAsync(
                    current.Type,
                    now.Date,
                    connection,
                    dbTransaction,
                    cancellationToken
                );

            long visitId;

            if (
                current.Type ==
                "EXAMINATION"
            )
            {
                if (
                    !current.DoctorId.HasValue
                )
                {
                    throw new BadRequestException(
                        "Lịch khám chưa có bác sĩ."
                    );
                }

                await ExecuteAsync(
                    """
                    INSERT INTO luotkham
                    (
                        IDDatLichKham,
                        IDKhachHang,
                        IDBacSi,
                        IDPhong,
                        SoThuTu,
                        ThoiGianTiepNhan,
                        TrangThai
                    )
                    VALUES
                    (
                        @appointmentId,
                        @customerId,
                        @doctorId,
                        @roomId,
                        @queueNumber,
                        @receivedAt,
                        'da_tiep_nhan'
                    )
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@appointmentId", current.DbId),
                    ("@customerId", current.CustomerId),
                    ("@doctorId", current.DoctorId),
                    ("@roomId", room?.Id),
                    ("@queueNumber", queueNumber),
                    ("@receivedAt", now)
                );

                visitId =
                    Convert.ToInt64(
                        await ScalarAsync(
                            "SELECT LAST_INSERT_ID();",
                            cancellationToken,
                            connection,
                            dbTransaction
                        )
                    );

                await ExecuteAsync(
                    """
                    UPDATE datlichkham
                    SET TrangThai = 'checked_in'
                    WHERE IDDatLichKham = @id
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", current.DbId)
                );

                await InsertTrackingAsync(
                    connection,
                    dbTransaction,
                    current.CustomerId,
                    "luotkham",
                    visitId.ToString(),
                    "Check-in khám bệnh",
                    current.Status,
                    "da_tiep_nhan",
                    actorUserId,
                    note,
                    cancellationToken
                );
            }
            else
            {
                await ExecuteAsync(
                    """
                    INSERT INTO luotxetnghiem
                    (
                        IDDatLichXN,
                        IDKhachHang,
                        IDPhong,
                        SoThuTu,
                        ThoiGianTiepNhan,
                        TrangThai
                    )
                    VALUES
                    (
                        @appointmentId,
                        @customerId,
                        @roomId,
                        @queueNumber,
                        @receivedAt,
                        'da_tiep_nhan'
                    )
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@appointmentId", current.DbId),
                    ("@customerId", current.CustomerId),
                    ("@roomId", room?.Id),
                    ("@queueNumber", queueNumber),
                    ("@receivedAt", now)
                );

                visitId =
                    Convert.ToInt64(
                        await ScalarAsync(
                            "SELECT LAST_INSERT_ID();",
                            cancellationToken,
                            connection,
                            dbTransaction
                        )
                    );

                await EnsureTestOrderAsync(
                    current,
                    visitId,
                    connection,
                    dbTransaction,
                    cancellationToken
                );

                await ExecuteAsync(
                    """
                    UPDATE datlichxetnghiem
                    SET TrangThai = 'checked_in'
                    WHERE IDDatLichXN = @id
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", current.DbId)
                );

                await InsertTrackingAsync(
                    connection,
                    dbTransaction,
                    current.CustomerId,
                    "luotxetnghiem",
                    visitId.ToString(),
                    "Check-in xét nghiệm",
                    current.Status,
                    "da_tiep_nhan",
                    actorUserId,
                    note,
                    cancellationToken
                );
            }

            await InsertCheckInNotificationAsync(
                current,
                visitId,
                room?.Name,
                connection,
                dbTransaction,
                cancellationToken
            );

            await transaction.CommitAsync(
                cancellationToken
            );

            return new CheckInResponse
            {
                Success = true,

                Message =
                    current.Type == "EXAMINATION"
                        ? $"Check-in khám thành công. Số thứ tự: {queueNumber}."
                        : $"Check-in xét nghiệm thành công. Số thứ tự: {queueNumber}.",

                VisitId =
                    visitId.ToString(),

                AppointmentId =
                    current.Code,

                Type =
                    current.Type,

                QueueNumber =
                    queueNumber,

                RoomId =
                    room?.Id,

                RoomName =
                    room?.Name,

                ReceivedAt =
                    now,

                Status =
                    "da_tiep_nhan"
            };
        }
        catch (DbException ex)
            when (
                ex.Message.Contains(
                    "Duplicate",
                    StringComparison.OrdinalIgnoreCase
                )
            )
        {
            throw new BadRequestException(
                "Lịch hẹn đã được check-in trước đó."
            );
        }
    }

    // =====================================================
    // RECEPTION WAITING
    // =====================================================

    public async Task<List<QueueItemResponse>> GetWaitingListAsync(
        string? type,
        CancellationToken cancellationToken = default
    )
    {
        var normalized =
            NormalizeQueueType(type);

        var now =
            VietnamTime.Now;

        var result =
            new List<QueueItemResponse>();

        if (
            normalized is
                "ALL"
                or "EXAMINATION"
        )
        {
            var rows =
                await QueryAsync(
                    """
                    SELECT
                        lk.IDLuotKham AS VisitId,
                        d.MaDatLich,
                        lk.SoThuTu,
                        k.IDKhachHang,
                        k.TenKhachHang,
                        lk.IDPhong,
                        p.TenPhong,
                        lk.ThoiGianTiepNhan,
                        lk.TrangThai
                    FROM luotkham lk
                    JOIN datlichkham d
                        ON lk.IDDatLichKham = d.IDDatLichKham
                    JOIN khachhang k
                        ON lk.IDKhachHang = k.IDKhachHang
                    LEFT JOIN phong p
                        ON lk.IDPhong = p.IDPhong
                    WHERE
                        DATE(lk.ThoiGianTiepNhan) = @date
                        AND lk.TrangThai IN
                        (
                            'da_tiep_nhan',
                            'cho_kham',
                            'da_den_luot',
                            'cho_goi_lai'
                        )
                    """,
                    cancellationToken,
                    ("@date", now.Date)
                );

            result.AddRange(
                rows.Select(
                    row =>
                        new QueueItemResponse
                        {
                            Id =
                                $"EXAMINATION:{GetLong(row, "VisitId")}",

                            AppointmentId =
                                GetString(row, "MaDatLich")!,

                            Type =
                                "EXAMINATION",

                            QueueNumber =
                                GetInt(row, "SoThuTu"),

                            CustomerId =
                                GetString(row, "IDKhachHang")!,

                            CustomerName =
                                GetString(row, "TenKhachHang")!,

                            ServiceName =
                                "Khám bệnh",

                            RoomId =
                                GetString(row, "IDPhong"),

                            RoomName =
                                GetString(row, "TenPhong"),

                            ReceivedAt =
                                GetDateTime(
                                    row,
                                    "ThoiGianTiepNhan"
                                ),

                            Status =
                                GetString(row, "TrangThai")!
                        }
                )
            );
        }

        if (
            normalized is
                "ALL"
                or "TEST"
        )
        {
            var rows =
                await QueryAsync(
                    """
                    SELECT
                        lx.IDLuotXetNghiem AS VisitId,
                        d.MaDatLich,
                        lx.SoThuTu,
                        k.IDKhachHang,
                        k.TenKhachHang,
                        lx.IDPhong,
                        p.TenPhong,
                        lx.ThoiGianTiepNhan,
                        lx.TrangThai
                    FROM luotxetnghiem lx
                    JOIN datlichxetnghiem d
                        ON lx.IDDatLichXN = d.IDDatLichXN
                    JOIN khachhang k
                        ON lx.IDKhachHang = k.IDKhachHang
                    LEFT JOIN phong p
                        ON lx.IDPhong = p.IDPhong
                    WHERE
                        DATE(lx.ThoiGianTiepNhan) = @date
                        AND lx.TrangThai IN
                        (
                            'da_tiep_nhan',
                            'cho_xet_nghiem',
                            'da_den_luot'
                        )
                    """,
                    cancellationToken,
                    ("@date", now.Date)
                );

            result.AddRange(
                rows.Select(
                    row =>
                        new QueueItemResponse
                        {
                            Id =
                                $"TEST:{GetLong(row, "VisitId")}",

                            AppointmentId =
                                GetString(row, "MaDatLich")!,

                            Type =
                                "TEST",

                            QueueNumber =
                                GetInt(row, "SoThuTu"),

                            CustomerId =
                                GetString(row, "IDKhachHang")!,

                            CustomerName =
                                GetString(row, "TenKhachHang")!,

                            ServiceName =
                                "Xét nghiệm",

                            RoomId =
                                GetString(row, "IDPhong"),

                            RoomName =
                                GetString(row, "TenPhong"),

                            ReceivedAt =
                                GetDateTime(
                                    row,
                                    "ThoiGianTiepNhan"
                                ),

                            Status =
                                GetString(row, "TrangThai")!
                        }
                )
            );
        }

        return result
            .OrderBy(
                x => x.ReceivedAt
            )
            .ThenBy(
                x => x.QueueNumber
            )
            .ToList();
    }

    // =====================================================
    // GET VISIT
    // =====================================================

    public async Task<Visit?> GetVisitAsync(
        long visitId,
        string type,
        CancellationToken cancellationToken = default
    )
    {
        var normalized =
            NormalizeVisitType(type);

        if (
            normalized ==
            "EXAMINATION"
        )
        {
            var rows =
                await QueryAsync(
                    """
                    SELECT
                        lk.IDLuotKham AS VisitId,
                        d.MaDatLich,
                        lk.IDKhachHang,
                        lk.IDBacSi,
                        lk.IDPhong,
                        lk.SoThuTu,
                        lk.ThoiGianTiepNhan,
                        lk.ThoiGianBatDau,
                        lk.ThoiGianKetThuc,
                        lk.TrangThai,
                        d.GhiChu
                    FROM luotkham lk
                    JOIN datlichkham d
                        ON lk.IDDatLichKham = d.IDDatLichKham
                    WHERE lk.IDLuotKham = @id
                    LIMIT 1
                    """,
                    cancellationToken,
                    ("@id", visitId)
                );

            return rows.Count == 0
                ? null
                : MapExaminationVisit(
                    rows[0]
                );
        }

        var testRows =
            await QueryAsync(
                """
                SELECT
                    lx.IDLuotXetNghiem AS VisitId,
                    d.MaDatLich,
                    lx.IDKhachHang,
                    d.IDBacSi,
                    lx.IDPhong,
                    lx.SoThuTu,
                    lx.ThoiGianTiepNhan,
                    lx.ThoiGianBatDau,
                    lx.ThoiGianKetThuc,
                    lx.TrangThai,
                    d.GhiChu
                FROM luotxetnghiem lx
                JOIN datlichxetnghiem d
                    ON lx.IDDatLichXN = d.IDDatLichXN
                WHERE lx.IDLuotXetNghiem = @id
                LIMIT 1
                """,
                cancellationToken,
                ("@id", visitId)
            );

        return testRows.Count == 0
            ? null
            : MapTestVisit(
                testRows[0]
            );
    }

    // =====================================================
    // UPDATE STATUS
    // =====================================================

    public async Task<Visit> UpdateStatusAsync(
        long visitId,
        string type,
        string status,
        string? note,
        int? actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedType =
            NormalizeVisitType(type);

        var now =
            VietnamTime.Now;

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        string oldStatus;
        string customerId;

        if (
            normalizedType ==
            "EXAMINATION"
        )
        {
            var rows =
                await QueryAsync(
                    """
                    SELECT
                        IDKhachHang,
                        TrangThai
                    FROM luotkham
                    WHERE IDLuotKham = @id
                    FOR UPDATE
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", visitId)
                );

            if (
                rows.Count == 0
            )
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy lượt khám {visitId}."
                );
            }

            customerId =
                GetString(
                    rows[0],
                    "IDKhachHang"
                )!;

            oldStatus =
                GetString(
                    rows[0],
                    "TrangThai"
                )!;

            await ExecuteAsync(
                """
                UPDATE luotkham
                SET
                    TrangThai = @status,

                    ThoiGianBatDau =
                        CASE
                            WHEN @status = 'dang_kham'
                            THEN COALESCE(ThoiGianBatDau, @now)
                            ELSE ThoiGianBatDau
                        END,

                    ThoiGianKetThuc =
                        CASE
                            WHEN @status = 'hoan_tat'
                            THEN COALESCE(ThoiGianKetThuc, @now)
                            ELSE ThoiGianKetThuc
                        END

                WHERE IDLuotKham = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@status", status),
                ("@now", now),
                ("@id", visitId)
            );

            if (
                status ==
                "hoan_tat"
            )
            {
                await ExecuteAsync(
                    """
                    UPDATE datlichkham d
                    JOIN luotkham lk
                        ON d.IDDatLichKham = lk.IDDatLichKham
                    SET d.TrangThai = 'completed'
                    WHERE lk.IDLuotKham = @id
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", visitId)
                );
            }

            await InsertTrackingAsync(
                connection,
                dbTransaction,
                customerId,
                "luotkham",
                visitId.ToString(),
                "Cập nhật trạng thái lượt khám",
                oldStatus,
                status,
                actorUserId,
                note,
                cancellationToken
            );
        }
        else
        {
            var rows =
                await QueryAsync(
                    """
                    SELECT
                        IDKhachHang,
                        TrangThai
                    FROM luotxetnghiem
                    WHERE IDLuotXetNghiem = @id
                    FOR UPDATE
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", visitId)
                );

            if (
                rows.Count == 0
            )
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy lượt xét nghiệm {visitId}."
                );
            }

            customerId =
                GetString(
                    rows[0],
                    "IDKhachHang"
                )!;

            oldStatus =
                GetString(
                    rows[0],
                    "TrangThai"
                )!;

            await ExecuteAsync(
                """
                UPDATE luotxetnghiem
                SET
                    TrangThai = @status,

                    ThoiGianBatDau =
                        CASE
                            WHEN @status IN ('dang_lay_mau', 'dang_xet_nghiem')
                            THEN COALESCE(ThoiGianBatDau, @now)
                            ELSE ThoiGianBatDau
                        END,

                    ThoiGianKetThuc =
                        CASE
                            WHEN @status = 'hoan_tat'
                            THEN COALESCE(ThoiGianKetThuc, @now)
                            ELSE ThoiGianKetThuc
                        END

                WHERE IDLuotXetNghiem = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@status", status),
                ("@now", now),
                ("@id", visitId)
            );

            if (
                status ==
                "hoan_tat"
            )
            {
                await ExecuteAsync(
                    """
                    UPDATE datlichxetnghiem d
                    JOIN luotxetnghiem lx
                        ON d.IDDatLichXN = lx.IDDatLichXN
                    SET d.TrangThai = 'completed'
                    WHERE lx.IDLuotXetNghiem = @id
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", visitId)
                );
            }

            await InsertTrackingAsync(
                connection,
                dbTransaction,
                customerId,
                "luotxetnghiem",
                visitId.ToString(),
                "Cập nhật trạng thái lượt xét nghiệm",
                oldStatus,
                status,
                actorUserId,
                note,
                cancellationToken
            );
        }

        await transaction.CommitAsync(
            cancellationToken
        );

        return await GetVisitAsync(
                   visitId,
                   normalizedType,
                   cancellationToken
               )
               ?? throw new ResourceNotFoundException(
                   "Không đọc được lượt sau khi cập nhật."
               );
    }

    // =====================================================
    // DOCTOR QUEUE
    // =====================================================

    public async Task<List<DoctorQueueItemResponse>>
        GetDoctorQueueAsync(
            int doctorId,
            CancellationToken cancellationToken = default
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    lk.IDLuotKham,
                    lk.SoThuTu,
                    k.IDKhachHang,
                    k.TenKhachHang,
                    d.GioKham,
                    lk.TrangThai,
                    lk.ThoiGianTiepNhan,
                    p.TenPhong
                FROM luotkham lk
                JOIN datlichkham d
                    ON lk.IDDatLichKham = d.IDDatLichKham
                JOIN khachhang k
                    ON lk.IDKhachHang = k.IDKhachHang
                LEFT JOIN phong p
                    ON lk.IDPhong = p.IDPhong
                WHERE
                    lk.IDBacSi = @doctorId
                    AND DATE(lk.ThoiGianTiepNhan) = @date
                    AND lk.TrangThai IN
                    (
                        'da_tiep_nhan',
                        'cho_kham',
                        'da_den_luot',
                        'cho_goi_lai'
                    )
                ORDER BY
                    CASE
                        WHEN lk.TrangThai = 'da_den_luot'
                            THEN 1
                        WHEN lk.TrangThai = 'cho_goi_lai'
                            THEN 3
                        ELSE 2
                    END,
                    lk.SoThuTu
                """,
                cancellationToken,
                ("@doctorId", doctorId),
                ("@date", VietnamTime.Now.Date)
            );

        return rows.Select(
            row =>
                new DoctorQueueItemResponse
                {
                    Id =
                        GetLong(
                            row,
                            "IDLuotKham"
                        )!.Value,

                    Stt =
                        GetInt(
                            row,
                            "SoThuTu"
                        ),

                    MaKhachHang =
                        GetString(
                            row,
                            "IDKhachHang"
                        )!,

                    TenKhachHang =
                        GetString(
                            row,
                            "TenKhachHang"
                        )!,

                    GioKham =
                        FormatTime(
                            GetTime(
                                row,
                                "GioKham"
                            )
                        ),

                    TrangThai =
                        GetString(
                            row,
                            "TrangThai"
                        )!,

                    Phong =
                        GetString(
                            row,
                            "TenPhong"
                        ),

                    ThoiGianTiepNhan =
                        GetDateTime(
                            row,
                            "ThoiGianTiepNhan"
                        )
                }
        ).ToList();
    }

    // =====================================================
    // DOCTOR CALL
    // =====================================================

    public async Task CallDoctorPatientAsync(
        long visitId,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        var rows =
            await QueryAsync(
                """
                SELECT
                    lk.IDKhachHang,
                    lk.IDBacSi,
                    lk.TrangThai,
                    p.TenPhong
                FROM luotkham lk
                LEFT JOIN phong p
                    ON lk.IDPhong = p.IDPhong
                WHERE lk.IDLuotKham = @id
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", visitId)
            );

        if (
            rows.Count == 0
        )
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy lượt khám."
            );
        }

        var ownerDoctor =
            GetInt(
                rows[0],
                "IDBacSi"
            );

        if (
            ownerDoctor !=
            doctorId
        )
        {
            throw new ForbiddenException(
                "Lượt khám không thuộc bác sĩ đang đăng nhập."
            );
        }

        var oldStatus =
            GetString(
                rows[0],
                "TrangThai"
            )!;

        if (
            oldStatus is
                "dang_kham"
                or "hoan_tat"
                or "bo_luot"
        )
        {
            throw new BadRequestException(
                "Không thể gọi bệnh nhân ở trạng thái hiện tại."
            );
        }

        await ExecuteAsync(
            """
            UPDATE luotkham
            SET TrangThai = 'da_den_luot'
            WHERE IDLuotKham = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@id", visitId)
        );

        var customerId =
            GetString(
                rows[0],
                "IDKhachHang"
            )!;

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            customerId,
            "luotkham",
            visitId.ToString(),
            "Bác sĩ gọi bệnh nhân vào phòng",
            oldStatus,
            "da_den_luot",
            actorUserId,
            null,
            cancellationToken
        );

        await InsertCustomerNotificationAsync(
            connection,
            dbTransaction,
            customerId,
            "kham_benh",
            "Đã đến lượt khám",
            string.IsNullOrWhiteSpace(
                GetString(
                    rows[0],
                    "TenPhong"
                )
            )
                ? "Vui lòng di chuyển đến phòng khám."
                : $"Vui lòng di chuyển vào {GetString(rows[0], "TenPhong")}.",
            "luotkham",
            visitId.ToString(),
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );
    }

    // =====================================================
    // DOCTOR HOLD
    // =====================================================

    public async Task HoldDoctorPatientAsync(
        long visitId,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        var rows =
            await QueryAsync(
                """
                SELECT
                    IDKhachHang,
                    IDBacSi,
                    TrangThai
                FROM luotkham
                WHERE IDLuotKham = @id
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", visitId)
            );

        if (
            rows.Count == 0
        )
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy lượt khám."
            );
        }

        if (
            GetInt(
                rows[0],
                "IDBacSi"
            ) != doctorId
        )
        {
            throw new ForbiddenException(
                "Lượt khám không thuộc bác sĩ đang đăng nhập."
            );
        }

        var oldStatus =
            GetString(
                rows[0],
                "TrangThai"
            )!;

        await ExecuteAsync(
            """
            UPDATE luotkham
            SET TrangThai = 'cho_goi_lai'
            WHERE IDLuotKham = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@id", visitId)
        );

        var customerId =
            GetString(
                rows[0],
                "IDKhachHang"
            )!;

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            customerId,
            "luotkham",
            visitId.ToString(),
            "Bệnh nhân vắng mặt, chuyển chờ gọi lại",
            oldStatus,
            "cho_goi_lai",
            actorUserId,
            null,
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );
    }

    // =====================================================
    // START EXAM
    // =====================================================

    public async Task<long> StartExamAsync(
        long visitId,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        var now =
            VietnamTime.Now;

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken
                );

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        var rows =
            await QueryAsync(
                """
                SELECT
                    IDKhachHang,
                    IDBacSi,
                    TrangThai,
                    ThoiGianBatDau
                FROM luotkham
                WHERE IDLuotKham = @id
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", visitId)
            );

        if (
            rows.Count == 0
        )
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy lượt khám."
            );
        }

        if (
            GetInt(
                rows[0],
                "IDBacSi"
            ) != doctorId
        )
        {
            throw new ForbiddenException(
                "Lượt khám không thuộc bác sĩ đang đăng nhập."
            );
        }

        var oldStatus =
            GetString(
                rows[0],
                "TrangThai"
            )!;

        if (
            oldStatus is
                "hoan_tat"
                or "bo_luot"
        )
        {
            throw new BadRequestException(
                "Lượt khám đã kết thúc."
            );
        }

        await ExecuteAsync(
            """
            UPDATE luotkham
            SET
                TrangThai = 'dang_kham',
                ThoiGianBatDau =
                    COALESCE(
                        ThoiGianBatDau,
                        @now
                    )
            WHERE IDLuotKham = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@now", now),
            ("@id", visitId)
        );

        var existingExam =
            await ScalarAsync(
                """
                SELECT IDKham
                FROM kham
                WHERE IDLuotKham = @visitId
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@visitId", visitId)
            );

        long examinationId;

        if (
            existingExam is not null
        )
        {
            examinationId =
                Convert.ToInt64(
                    existingExam
                );
        }
        else
        {
            await ExecuteAsync(
                """
                INSERT INTO kham
                (
                    IDLuotKham,
                    IDBacSi,
                    ThoiGianKham
                )
                VALUES
                (
                    @visitId,
                    @doctorId,
                    @now
                )
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@visitId", visitId),
                ("@doctorId", doctorId),
                ("@now", now)
            );

            examinationId =
                Convert.ToInt64(
                    await ScalarAsync(
                        "SELECT LAST_INSERT_ID();",
                        cancellationToken,
                        connection,
                        dbTransaction
                    )
                );
        }

        var customerId =
            GetString(
                rows[0],
                "IDKhachHang"
            )!;

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            customerId,
            "luotkham",
            visitId.ToString(),
            "Bác sĩ bắt đầu khám",
            oldStatus,
            "dang_kham",
            actorUserId,
            null,
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );

        return examinationId;
    }

    // =====================================================
    // RECEPTION QUEUE CALL
    // =====================================================

    public async Task CallByAppointmentAsync(
        string appointmentCode,
        int? actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        await ChangeReceptionQueueStateAsync(
            appointmentCode,
            "CALL",
            actorUserId,
            cancellationToken
        );
    }

    public async Task HoldByAppointmentAsync(
        string appointmentCode,
        int? actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        await ChangeReceptionQueueStateAsync(
            appointmentCode,
            "HOLD",
            actorUserId,
            cancellationToken
        );
    }

    public async Task SkipByAppointmentAsync(
        string appointmentCode,
        int? actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        await ChangeReceptionQueueStateAsync(
            appointmentCode,
            "SKIP",
            actorUserId,
            cancellationToken
        );
    }

    // =====================================================
    // WALK-IN
    // =====================================================

    public async Task<WalkInVisitResponse> CreateWalkInAsync(
        WalkInVisitData request,
        int? actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        var now =
            VietnamTime.Now;

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken
                );

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        var customerId =
            await FindOrCreateWalkInCustomerAsync(
                request,
                connection,
                dbTransaction,
                cancellationToken
            );

        if (
            request.Type ==
            "EXAMINATION"
        )
        {
            if (
                string.IsNullOrWhiteSpace(
                    request.SpecialtyId
                )
            )
            {
                throw new BadRequestException(
                    "Tiếp nhận khám bệnh phải chọn chuyên khoa."
                );
            }

            var doctorRows =
                await QueryAsync(
                    """
                    SELECT
                        b.IDBacSi,
                        b.CoSoID
                    FROM bacsi b
                    LEFT JOIN lichlamviec l
                        ON
                            l.IDBacSi = b.IDBacSi
                            AND l.Ngay = @date
                            AND l.TrangThai = 'duoc_duyet'
                            AND @time >= l.GioBatDau
                            AND @time < l.GioKetThuc
                    WHERE
                        b.KhoaID = @specialtyId
                        AND b.TrangThai = 'active'
                    ORDER BY
                        CASE
                            WHEN l.LichID IS NOT NULL
                                THEN 0
                            ELSE 1
                        END,
                        b.IDBacSi
                    LIMIT 1
                    FOR UPDATE
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@date", now.Date),
                    ("@time", now.TimeOfDay),
                    ("@specialtyId", request.SpecialtyId)
                );

            if (
                doctorRows.Count == 0
            )
            {
                throw new BadRequestException(
                    "Không có bác sĩ đang hoạt động thuộc chuyên khoa đã chọn."
                );
            }

            var doctorId =
                GetInt(
                    doctorRows[0],
                    "IDBacSi"
                )!.Value;

            var facilityId =
                GetString(
                    doctorRows[0],
                    "CoSoID"
                );

            if (
                string.IsNullOrWhiteSpace(
                    facilityId
                )
            )
            {
                facilityId =
                    await GetFirstActiveFacilityAsync(
                        connection,
                        dbTransaction,
                        cancellationToken
                    );
            }

            var room =
                await FindExaminationRoomAsync(
                    facilityId!,
                    request.SpecialtyId,
                    connection,
                    dbTransaction,
                    cancellationToken
                );

            var queue =
                await GetNextQueueNumberAsync(
                    "EXAMINATION",
                    now.Date,
                    connection,
                    dbTransaction,
                    cancellationToken
                );

            var code =
                GenerateCode(
                    "DLK"
                );

            var qr =
                GenerateQr();

            var note =
                BuildNote(
                    request.Reason,
                    request.Note
                );

            await ExecuteAsync(
                """
                INSERT INTO datlichkham
                (
                    MaDatLich,
                    UserID,
                    IDKhachHang,
                    IDChuyenKhoa,
                    IDBacSi,
                    CoSoID,
                    NgayKham,
                    GioKham,
                    GhiChu,
                    TrangThai,
                    StatusMail,
                    MaQR
                )
                VALUES
                (
                    @code,
                    NULL,
                    @customerId,
                    @specialtyId,
                    @doctorId,
                    @facilityId,
                    @date,
                    @time,
                    @note,
                    'checked_in',
                    'pending',
                    @qr
                )
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@code", code),
                ("@customerId", customerId),
                ("@specialtyId", request.SpecialtyId),
                ("@doctorId", doctorId),
                ("@facilityId", facilityId),
                ("@date", now.Date),
                ("@time", now.TimeOfDay),
                ("@note", note),
                ("@qr", qr)
            );

            var appointmentId =
                Convert.ToInt64(
                    await ScalarAsync(
                        "SELECT LAST_INSERT_ID();",
                        cancellationToken,
                        connection,
                        dbTransaction
                    )
                );

            await ExecuteAsync(
                """
                INSERT INTO luotkham
                (
                    IDDatLichKham,
                    IDKhachHang,
                    IDBacSi,
                    IDPhong,
                    SoThuTu,
                    ThoiGianTiepNhan,
                    TrangThai
                )
                VALUES
                (
                    @appointmentId,
                    @customerId,
                    @doctorId,
                    @roomId,
                    @queue,
                    @now,
                    'da_tiep_nhan'
                )
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@appointmentId", appointmentId),
                ("@customerId", customerId),
                ("@doctorId", doctorId),
                ("@roomId", room?.Id),
                ("@queue", queue),
                ("@now", now)
            );

            var visitId =
                Convert.ToInt64(
                    await ScalarAsync(
                        "SELECT LAST_INSERT_ID();",
                        cancellationToken,
                        connection,
                        dbTransaction
                    )
                );

            await InsertTrackingAsync(
                connection,
                dbTransaction,
                customerId,
                "luotkham",
                visitId.ToString(),
                "Tiếp nhận khách vãng lai khám bệnh",
                null,
                "da_tiep_nhan",
                actorUserId,
                note,
                cancellationToken
            );

            await transaction.CommitAsync(
                cancellationToken
            );

            return new WalkInVisitResponse
            {
                Message =
                    $"Tiếp nhận khám thành công. Số thứ tự: {queue}.",

                AppointmentId =
                    code,

                VisitId =
                    visitId.ToString(),

                CustomerId =
                    customerId,

                Type =
                    "EXAMINATION",

                QueueNumber =
                    queue,

                RoomId =
                    room?.Id,

                RoomName =
                    room?.Name,

                ReceivedAt =
                    now
            };
        }

        // =================================================
        // WALK-IN TEST
        // =================================================

        if (
            string.IsNullOrWhiteSpace(
                request.TestId
            )
        )
        {
            throw new BadRequestException(
                "Tiếp nhận xét nghiệm phải chọn loại xét nghiệm."
            );
        }

        var testRows =
            await QueryAsync(
                """
                SELECT
                    IDXetNghiem,
                    ChuyenKhoaID,
                    TenXetNghiem,
                    Gia
                FROM loaixetnghiem
                WHERE
                    IDXetNghiem = @id
                    AND Status = 'yes'
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", request.TestId)
            );

        if (
            testRows.Count == 0
        )
        {
            throw new BadRequestException(
                "Xét nghiệm không tồn tại hoặc đã ngừng hoạt động."
            );
        }

        var testSpecialty =
            GetString(
                testRows[0],
                "ChuyenKhoaID"
            )!;

        if (
            !string.IsNullOrWhiteSpace(
                request.SpecialtyId
            )
            &&
            !string.Equals(
                request.SpecialtyId,
                testSpecialty,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BadRequestException(
                "Xét nghiệm không thuộc chuyên khoa đã chọn."
            );
        }

        var price =
            GetDecimal(
                testRows[0],
                "Gia"
            ) ?? 0;

        var roomRows =
            await QueryAsync(
                """
                SELECT
                    p.IDPhong,
                    p.TenPhong,
                    p.CoSoID
                FROM phong p
                JOIN coso c
                    ON p.CoSoID = c.CoSoID
                WHERE
                    p.TrangThai = 'active'
                    AND c.TrangThai = 'active'
                    AND p.LoaiPhong IN
                    (
                        'lay_mau',
                        'xet_nghiem'
                    )
                    AND
                    (
                        p.IDChuyenKhoa = @specialtyId
                        OR p.IDChuyenKhoa IS NULL
                    )
                ORDER BY
                    CASE
                        WHEN p.LoaiPhong = 'lay_mau'
                            THEN 0
                        ELSE 1
                    END,
                    CASE
                        WHEN p.IDChuyenKhoa = @specialtyId
                            THEN 0
                        ELSE 1
                    END,
                    p.IDPhong
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@specialtyId", testSpecialty)
            );

        string facilityId;

        VisitRoomData? testRoom = null;

        if (
            roomRows.Count > 0
        )
        {
            facilityId =
                GetString(
                    roomRows[0],
                    "CoSoID"
                )!;

            testRoom =
                new VisitRoomData(
                    GetString(
                        roomRows[0],
                        "IDPhong"
                    )!,
                    GetString(
                        roomRows[0],
                        "TenPhong"
                    )!,
                    facilityId,
                    testSpecialty,
                    "xet_nghiem"
                );
        }
        else
        {
            facilityId =
                await GetFirstActiveFacilityAsync(
                    connection,
                    dbTransaction,
                    cancellationToken
                );
        }

        var testQueue =
            await GetNextQueueNumberAsync(
                "TEST",
                now.Date,
                connection,
                dbTransaction,
                cancellationToken
            );

        var testCode =
            GenerateCode(
                "DLXN"
            );

        var testQr =
            GenerateQr();

        var testNote =
            BuildNote(
                request.Reason,
                request.Note
            );

        await ExecuteAsync(
            """
            INSERT INTO datlichxetnghiem
            (
                MaDatLich,
                UserID,
                IDKhachHang,
                IDBacSi,
                CoSoID,
                NgayXetNghiem,
                GioXetNghiem,
                GhiChu,
                TrangThai,
                StatusMail,
                MaQR
            )
            VALUES
            (
                @code,
                NULL,
                @customerId,
                NULL,
                @facilityId,
                @date,
                @time,
                @note,
                'checked_in',
                'pending',
                @qr
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@code", testCode),
            ("@customerId", customerId),
            ("@facilityId", facilityId),
            ("@date", now.Date),
            ("@time", now.TimeOfDay),
            ("@note", testNote),
            ("@qr", testQr)
        );

        var testAppointmentId =
            Convert.ToInt64(
                await ScalarAsync(
                    "SELECT LAST_INSERT_ID();",
                    cancellationToken,
                    connection,
                    dbTransaction
                )
            );

        await ExecuteAsync(
            """
            INSERT INTO ctdatlichxetnghiem
            (
                IDDatLichXN,
                IDXetNghiem,
                DonGia,
                GhiChu
            )
            VALUES
            (
                @appointmentId,
                @testId,
                @price,
                @note
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@appointmentId", testAppointmentId),
            ("@testId", request.TestId),
            ("@price", price),
            ("@note", Truncate(testNote, 255))
        );

        await ExecuteAsync(
            """
            INSERT INTO luotxetnghiem
            (
                IDDatLichXN,
                IDKhachHang,
                IDPhong,
                SoThuTu,
                ThoiGianTiepNhan,
                TrangThai
            )
            VALUES
            (
                @appointmentId,
                @customerId,
                @roomId,
                @queue,
                @now,
                'da_tiep_nhan'
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@appointmentId", testAppointmentId),
            ("@customerId", customerId),
            ("@roomId", testRoom?.Id),
            ("@queue", testQueue),
            ("@now", now)
        );

        var testVisitId =
            Convert.ToInt64(
                await ScalarAsync(
                    "SELECT LAST_INSERT_ID();",
                    cancellationToken,
                    connection,
                    dbTransaction
                )
            );

        var testOrderId =
            GenerateCode(
                "PXN"
            );

        await ExecuteAsync(
            """
            INSERT INTO phieuxetnghiem
            (
                IDPhieuXetNghiem,
                IDKhachHang,
                IDKham,
                IDLuotXetNghiem,
                IDBacSiChiDinh,
                IDBacSiPhuTrach,
                NgayTao,
                TongTien,
                TrangThaiThanhToan,
                TrangThai,
                GhiChu
            )
            VALUES
            (
                @orderId,
                @customerId,
                NULL,
                @visitId,
                NULL,
                NULL,
                @now,
                @total,
                'chua_thanh_toan',
                'moi_tao',
                @note
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@orderId", testOrderId),
            ("@customerId", customerId),
            ("@visitId", testVisitId),
            ("@now", now),
            ("@total", price),
            ("@note", testNote)
        );

        await ExecuteAsync(
            """
            INSERT INTO ctphieuxetnghiem
            (
                IDPhieuXetNghiem,
                IDXetNghiem,
                SoLuong,
                DonGia,
                GhiChu,
                TrangThai,
                Status
            )
            VALUES
            (
                @orderId,
                @testId,
                1,
                @price,
                @note,
                'cho_lay_mau',
                'yes'
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@orderId", testOrderId),
            ("@testId", request.TestId),
            ("@price", price),
            ("@note", Truncate(testNote, 255))
        );

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            customerId,
            "luotxetnghiem",
            testVisitId.ToString(),
            "Tiếp nhận khách vãng lai xét nghiệm",
            null,
            "da_tiep_nhan",
            actorUserId,
            testNote,
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );

        return new WalkInVisitResponse
        {
            Message =
                $"Tiếp nhận xét nghiệm thành công. Số thứ tự: {testQueue}.",

            AppointmentId =
                testCode,

            VisitId =
                testVisitId.ToString(),

            CustomerId =
                customerId,

            Type =
                "TEST",

            QueueNumber =
                testQueue,

            RoomId =
                testRoom?.Id,

            RoomName =
                testRoom?.Name,

            TestOrderId =
                testOrderId,

            ReceivedAt =
                now
        };
    }

    // =====================================================
    // RECEPTION STATE HELPER
    // =====================================================

    private async Task ChangeReceptionQueueStateAsync(
        string appointmentCode,
        string action,
        int? actorUserId,
        CancellationToken cancellationToken
    )
    {
        var appointment =
            await FindAppointmentAsync(
                appointmentCode,
                null,
                cancellationToken
            )
            ?? throw new ResourceNotFoundException(
                $"Không tìm thấy lịch hẹn {appointmentCode}."
            );

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        var visit =
            await FindVisitByAppointmentDbIdAsync(
                appointment,
                connection,
                dbTransaction,
                true,
                cancellationToken
            )
            ?? throw new BadRequestException(
                "Lịch hẹn chưa check-in."
            );

        var oldStatus =
            visit.Status;

        string newStatus;

        if (
            action ==
            "CALL"
        )
        {
            newStatus =
                "da_den_luot";
        }
        else if (
            action ==
            "HOLD"
        )
        {
            newStatus =
                appointment.Type ==
                "EXAMINATION"
                    ? "cho_goi_lai"
                    : "cho_xet_nghiem";
        }
        else
        {
            newStatus =
                appointment.Type ==
                "EXAMINATION"
                    ? "bo_luot"
                    : "huy";
        }

        var table =
            appointment.Type ==
            "EXAMINATION"
                ? "luotkham"
                : "luotxetnghiem";

        var idColumn =
            appointment.Type ==
            "EXAMINATION"
                ? "IDLuotKham"
                : "IDLuotXetNghiem";

        await ExecuteAsync(
            $"""
            UPDATE {table}
            SET TrangThai = @status
            WHERE {idColumn} = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@status", newStatus),
            ("@id", long.Parse(visit.Id))
        );

        if (
            action ==
            "SKIP"
        )
        {
            var appointmentTable =
                appointment.Type ==
                "EXAMINATION"
                    ? "datlichkham"
                    : "datlichxetnghiem";

            var appointmentIdColumn =
                appointment.Type ==
                "EXAMINATION"
                    ? "IDDatLichKham"
                    : "IDDatLichXN";

            await ExecuteAsync(
                $"""
                UPDATE {appointmentTable}
                SET TrangThai = 'cancelled'
                WHERE {appointmentIdColumn} = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", appointment.DbId)
            );
        }

        var actionName =
            action switch
            {
                "CALL" =>
                    "Gọi bệnh nhân",

                "HOLD" =>
                    "Chuyển bệnh nhân chờ gọi lại",

                _ =>
                    "Bệnh nhân bỏ lượt"
            };

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            appointment.CustomerId,
            appointment.Type ==
                "EXAMINATION"
                ? "luotkham"
                : "luotxetnghiem",
            visit.Id,
            actionName,
            oldStatus,
            newStatus,
            actorUserId,
            null,
            cancellationToken
        );

        if (
            action ==
            "CALL"
        )
        {
            await InsertCustomerNotificationAsync(
                connection,
                dbTransaction,
                appointment.CustomerId,
                appointment.Type ==
                    "EXAMINATION"
                    ? "kham_benh"
                    : "xet_nghiem",
                appointment.Type ==
                    "EXAMINATION"
                    ? "Đã đến lượt khám"
                    : "Đã đến lượt xét nghiệm",
                appointment.Type ==
                    "EXAMINATION"
                    ? "Vui lòng di chuyển vào phòng khám."
                    : "Vui lòng di chuyển đến khu vực lấy mẫu/xét nghiệm.",
                appointment.Type ==
                    "EXAMINATION"
                    ? "luotkham"
                    : "luotxetnghiem",
                visit.Id,
                cancellationToken
            );
        }

        await transaction.CommitAsync(
            cancellationToken
        );
    }

    // =====================================================
    // CREATE TEST ORDER FROM BOOKED APPOINTMENT
    // =====================================================

    private async Task EnsureTestOrderAsync(
        VisitAppointmentSource appointment,
        long visitId,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var existing =
            await ScalarAsync(
                """
                SELECT IDPhieuXetNghiem
                FROM phieuxetnghiem
                WHERE IDLuotXetNghiem = @visitId
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                transaction,
                ("@visitId", visitId)
            );

        if (
            existing is not null
        )
        {
            return;
        }

        var items =
            await QueryAsync(
                """
                SELECT
                    ct.IDXetNghiem,
                    ct.DonGia,
                    ct.GhiChu
                FROM ctdatlichxetnghiem ct
                WHERE ct.IDDatLichXN = @appointmentId
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                transaction,
                ("@appointmentId", appointment.DbId)
            );

        if (
            items.Count == 0
        )
        {
            throw new BadRequestException(
                "Lịch xét nghiệm chưa có loại xét nghiệm đăng ký."
            );
        }

        var total =
            items.Sum(
                x =>
                    GetDecimal(
                        x,
                        "DonGia"
                    ) ?? 0
            );

        var orderId =
            GenerateCode(
                "PXN"
            );

        await ExecuteAsync(
            """
            INSERT INTO phieuxetnghiem
            (
                IDPhieuXetNghiem,
                IDKhachHang,
                IDKham,
                IDLuotXetNghiem,
                IDBacSiChiDinh,
                IDBacSiPhuTrach,
                NgayTao,
                TongTien,
                TrangThaiThanhToan,
                TrangThai,
                GhiChu
            )
            VALUES
            (
                @orderId,
                @customerId,
                NULL,
                @visitId,
                NULL,
                @doctorId,
                @now,
                @total,
                'chua_thanh_toan',
                'moi_tao',
                @note
            )
            """,
            cancellationToken,
            connection,
            transaction,
            ("@orderId", orderId),
            ("@customerId", appointment.CustomerId),
            ("@visitId", visitId),
            ("@doctorId", appointment.DoctorId),
            ("@now", VietnamTime.Now),
            ("@total", total),
            ("@note", appointment.Note)
        );

        foreach (
            var item in items
        )
        {
            await ExecuteAsync(
                """
                INSERT INTO ctphieuxetnghiem
                (
                    IDPhieuXetNghiem,
                    IDXetNghiem,
                    SoLuong,
                    DonGia,
                    GhiChu,
                    TrangThai,
                    Status
                )
                VALUES
                (
                    @orderId,
                    @testId,
                    1,
                    @price,
                    @note,
                    'cho_lay_mau',
                    'yes'
                )
                """,
                cancellationToken,
                connection,
                transaction,
                ("@orderId", orderId),
                (
                    "@testId",
                    GetString(
                        item,
                        "IDXetNghiem"
                    )
                ),
                (
                    "@price",
                    GetDecimal(
                        item,
                        "DonGia"
                    ) ?? 0
                ),
                (
                    "@note",
                    GetString(
                        item,
                        "GhiChu"
                    )
                )
            );
        }
    }

    // =====================================================
    // FIND ROOMS
    // =====================================================

    private async Task<VisitRoomData?> FindExaminationRoomAsync(
        string facilityId,
        string? specialtyId,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDPhong,
                    TenPhong,
                    CoSoID,
                    IDChuyenKhoa,
                    LoaiPhong
                FROM phong
                WHERE
                    CoSoID = @facilityId
                    AND LoaiPhong = 'kham'
                    AND TrangThai = 'active'
                    AND
                    (
                        IDChuyenKhoa = @specialtyId
                        OR IDChuyenKhoa IS NULL
                    )
                ORDER BY
                    CASE
                        WHEN IDChuyenKhoa = @specialtyId
                            THEN 0
                        ELSE 1
                    END,
                    IDPhong
                LIMIT 1
                """,
                cancellationToken,
                connection,
                transaction,
                ("@facilityId", facilityId),
                ("@specialtyId", specialtyId)
            );

        return rows.Count == 0
            ? null
            : MapRoom(
                rows[0]
            );
    }

    private async Task<VisitRoomData?> FindTestRoomAsync(
        string facilityId,
        string? specialtyId,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDPhong,
                    TenPhong,
                    CoSoID,
                    IDChuyenKhoa,
                    LoaiPhong
                FROM phong
                WHERE
                    CoSoID = @facilityId
                    AND LoaiPhong IN
                    (
                        'lay_mau',
                        'xet_nghiem'
                    )
                    AND TrangThai = 'active'
                    AND
                    (
                        @specialtyId IS NULL
                        OR IDChuyenKhoa = @specialtyId
                        OR IDChuyenKhoa IS NULL
                    )
                ORDER BY
                    CASE
                        WHEN LoaiPhong = 'lay_mau'
                            THEN 0
                        ELSE 1
                    END,
                    CASE
                        WHEN IDChuyenKhoa = @specialtyId
                            THEN 0
                        ELSE 1
                    END,
                    IDPhong
                LIMIT 1
                """,
                cancellationToken,
                connection,
                transaction,
                ("@facilityId", facilityId),
                ("@specialtyId", specialtyId)
            );

        return rows.Count == 0
            ? null
            : MapRoom(
                rows[0]
            );
    }

    // =====================================================
    // NEXT QUEUE
    // =====================================================

    private async Task<int> GetNextQueueNumberAsync(
        string type,
        DateTime date,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        string sql;

        if (
            type ==
            "EXAMINATION"
        )
        {
            sql =
                """
                SELECT lk.SoThuTu
                FROM luotkham lk
                JOIN datlichkham d
                    ON lk.IDDatLichKham = d.IDDatLichKham
                WHERE d.NgayKham = @date
                ORDER BY lk.SoThuTu DESC
                LIMIT 1
                FOR UPDATE
                """;
        }
        else
        {
            sql =
                """
                SELECT lx.SoThuTu
                FROM luotxetnghiem lx
                JOIN datlichxetnghiem d
                    ON lx.IDDatLichXN = d.IDDatLichXN
                WHERE d.NgayXetNghiem = @date
                ORDER BY lx.SoThuTu DESC
                LIMIT 1
                FOR UPDATE
                """;
        }

        var value =
            await ScalarAsync(
                sql,
                cancellationToken,
                connection,
                transaction,
                ("@date", date.Date)
            );

        return value is null
            ? 1
            : Convert.ToInt32(
                value
            ) + 1;
    }

    // =====================================================
    // FIND VISIT BY APPOINTMENT
    // =====================================================

    private async Task<Visit?> FindVisitByAppointmentDbIdAsync(
        VisitAppointmentSource appointment,
        DbConnection connection,
        DbTransaction transaction,
        bool forUpdate,
        CancellationToken cancellationToken
    )
    {
        if (
            appointment.Type ==
            "EXAMINATION"
        )
        {
            var rows =
                await QueryAsync(
                    $"""
                    SELECT
                        lk.IDLuotKham AS VisitId,
                        d.MaDatLich,
                        lk.IDKhachHang,
                        lk.IDBacSi,
                        lk.IDPhong,
                        lk.SoThuTu,
                        lk.ThoiGianTiepNhan,
                        lk.ThoiGianBatDau,
                        lk.ThoiGianKetThuc,
                        lk.TrangThai,
                        d.GhiChu
                    FROM luotkham lk
                    JOIN datlichkham d
                        ON lk.IDDatLichKham = d.IDDatLichKham
                    WHERE lk.IDDatLichKham = @id
                    LIMIT 1
                    {(forUpdate ? "FOR UPDATE" : string.Empty)}
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@id", appointment.DbId)
                );

            return rows.Count == 0
                ? null
                : MapExaminationVisit(
                    rows[0]
                );
        }

        var testRows =
            await QueryAsync(
                $"""
                SELECT
                    lx.IDLuotXetNghiem AS VisitId,
                    d.MaDatLich,
                    lx.IDKhachHang,
                    d.IDBacSi,
                    lx.IDPhong,
                    lx.SoThuTu,
                    lx.ThoiGianTiepNhan,
                    lx.ThoiGianBatDau,
                    lx.ThoiGianKetThuc,
                    lx.TrangThai,
                    d.GhiChu
                FROM luotxetnghiem lx
                JOIN datlichxetnghiem d
                    ON lx.IDDatLichXN = d.IDDatLichXN
                WHERE lx.IDDatLichXN = @id
                LIMIT 1
                {(forUpdate ? "FOR UPDATE" : string.Empty)}
                """,
                cancellationToken,
                connection,
                transaction,
                ("@id", appointment.DbId)
            );

        return testRows.Count == 0
            ? null
            : MapTestVisit(
                testRows[0]
            );
    }

    // =====================================================
    // APPOINTMENT QUERIES
    // =====================================================

    private async Task<VisitAppointmentSource?>
        FindExaminationAppointmentAsync(
            string code,
            CancellationToken cancellationToken
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDDatLichKham AS DbId,
                    MaDatLich AS Code,
                    'EXAMINATION' AS VisitType,
                    UserID AS BookedUserId,
                    IDKhachHang AS CustomerId,
                    IDBacSi AS DoctorId,
                    IDChuyenKhoa AS SpecialtyId,
                    CoSoID AS FacilityId,
                    NgayKham AS AppointmentDate,
                    GioKham AS AppointmentTime,
                    TrangThai AS AppointmentStatus,
                    GhiChu AS Note,
                    MaQR AS QrCode
                FROM datlichkham
                WHERE MaDatLich = @code
                LIMIT 1
                """,
                cancellationToken,
                ("@code", code)
            );

        return rows.Count == 0
            ? null
            : MapAppointment(
                rows[0]
            );
    }

    private async Task<VisitAppointmentSource?>
        FindTestAppointmentAsync(
            string code,
            CancellationToken cancellationToken
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDDatLichXN AS DbId,
                    MaDatLich AS Code,
                    'TEST' AS VisitType,
                    UserID AS BookedUserId,
                    IDKhachHang AS CustomerId,
                    IDBacSi AS DoctorId,
                    NULL AS SpecialtyId,
                    CoSoID AS FacilityId,
                    NgayXetNghiem AS AppointmentDate,
                    GioXetNghiem AS AppointmentTime,
                    TrangThai AS AppointmentStatus,
                    GhiChu AS Note,
                    MaQR AS QrCode
                FROM datlichxetnghiem
                WHERE MaDatLich = @code
                LIMIT 1
                """,
                cancellationToken,
                ("@code", code)
            );

        return rows.Count == 0
            ? null
            : MapAppointment(
                rows[0]
            );
    }

    private async Task<VisitAppointmentSource?>
        FindAppointmentForUpdateAsync(
            VisitAppointmentSource source,
            DbConnection connection,
            DbTransaction transaction,
            CancellationToken cancellationToken
        )
    {
        string sql;

        if (
            source.Type ==
            "EXAMINATION"
        )
        {
            sql =
                """
                SELECT
                    IDDatLichKham AS DbId,
                    MaDatLich AS Code,
                    'EXAMINATION' AS VisitType,
                    UserID AS BookedUserId,
                    IDKhachHang AS CustomerId,
                    IDBacSi AS DoctorId,
                    IDChuyenKhoa AS SpecialtyId,
                    CoSoID AS FacilityId,
                    NgayKham AS AppointmentDate,
                    GioKham AS AppointmentTime,
                    TrangThai AS AppointmentStatus,
                    GhiChu AS Note,
                    MaQR AS QrCode
                FROM datlichkham
                WHERE IDDatLichKham = @id
                FOR UPDATE
                """;
        }
        else
        {
            sql =
                """
                SELECT
                    IDDatLichXN AS DbId,
                    MaDatLich AS Code,
                    'TEST' AS VisitType,
                    UserID AS BookedUserId,
                    IDKhachHang AS CustomerId,
                    IDBacSi AS DoctorId,
                    NULL AS SpecialtyId,
                    CoSoID AS FacilityId,
                    NgayXetNghiem AS AppointmentDate,
                    GioXetNghiem AS AppointmentTime,
                    TrangThai AS AppointmentStatus,
                    GhiChu AS Note,
                    MaQR AS QrCode
                FROM datlichxetnghiem
                WHERE IDDatLichXN = @id
                FOR UPDATE
                """;
        }

        var rows =
            await QueryAsync(
                sql,
                cancellationToken,
                connection,
                transaction,
                ("@id", source.DbId)
            );

        return rows.Count == 0
            ? null
            : MapAppointment(
                rows[0]
            );
    }

    // =====================================================
    // TEST SPECIALTY
    // =====================================================

    private async Task<string?> GetTestAppointmentSpecialtyAsync(
        long appointmentId,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var result =
            await ScalarAsync(
                """
                SELECT l.ChuyenKhoaID
                FROM ctdatlichxetnghiem ct
                JOIN loaixetnghiem l
                    ON ct.IDXetNghiem = l.IDXetNghiem
                WHERE ct.IDDatLichXN = @id
                LIMIT 1
                """,
                cancellationToken,
                connection,
                transaction,
                ("@id", appointmentId)
            );

        return result is null
            ? null
            : Convert.ToString(
                result,
                CultureInfo.InvariantCulture
            );
    }

    // =====================================================
    // CUSTOMER
    // =====================================================

    private async Task<string> FindOrCreateWalkInCustomerAsync(
        WalkInVisitData request,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT IDKhachHang
                FROM khachhang
                WHERE
                    SoDienThoai = @phone
                    AND Status = 'yes'
                ORDER BY CreatedAt DESC
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                transaction,
                ("@phone", request.Phone)
            );

        if (
            rows.Count > 0
        )
        {
            return GetString(
                rows[0],
                "IDKhachHang"
            )!;
        }

        var id =
            "KH"
            +
            Guid.NewGuid()
                .ToString("N")[..8]
                .ToUpperInvariant();

        await ExecuteAsync(
            """
            INSERT INTO khachhang
            (
                IDKhachHang,
                TenKhachHang,
                NgaySinh,
                SoDienThoai,
                GioiTinh,
                DiaChi,
                Email,
                Status
            )
            VALUES
            (
                @id,
                @name,
                @birthDate,
                @phone,
                @gender,
                @address,
                @email,
                'yes'
            )
            """,
            cancellationToken,
            connection,
            transaction,
            ("@id", id),
            ("@name", request.FullName),
            ("@birthDate", request.BirthDate?.Date),
            ("@phone", request.Phone),
            ("@gender", request.Gender),
            ("@address", request.Address),
            ("@email", request.Email)
        );

        return id;
    }

    private async Task<string> GetFirstActiveFacilityAsync(
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var result =
            await ScalarAsync(
                """
                SELECT CoSoID
                FROM coso
                WHERE TrangThai = 'active'
                ORDER BY CoSoID
                LIMIT 1
                """,
                cancellationToken,
                connection,
                transaction
            );

        if (
            result is null
        )
        {
            throw new BadRequestException(
                "Hệ thống chưa có cơ sở đang hoạt động."
            );
        }

        return Convert.ToString(
            result,
            CultureInfo.InvariantCulture
        )!;
    }

    // =====================================================
    // TRACKING
    // =====================================================

    private async Task InsertTrackingAsync(
        DbConnection connection,
        DbTransaction transaction,
        string customerId,
        string objectType,
        string objectId,
        string action,
        string? oldStatus,
        string? newStatus,
        int? actorUserId,
        string? description,
        CancellationToken cancellationToken
    )
    {
        await ExecuteAsync(
            """
            INSERT INTO truyvet
            (
                IDKhachHang,
                LoaiDoiTuong,
                IDDoiTuong,
                HanhDong,
                TrangThaiCu,
                TrangThaiMoi,
                UserIDThucHien,
                NguonThucHien,
                ThoiGian,
                MoTa
            )
            VALUES
            (
                @customerId,
                @objectType,
                @objectId,
                @action,
                @oldStatus,
                @newStatus,
                @actorUserId,
                @source,
                @time,
                @description
            )
            """,
            cancellationToken,
            connection,
            transaction,
            ("@customerId", customerId),
            ("@objectType", objectType),
            ("@objectId", objectId),
            ("@action", action),
            ("@oldStatus", oldStatus),
            ("@newStatus", newStatus),
            ("@actorUserId", actorUserId),
            (
                "@source",
                actorUserId.HasValue
                    ? "user"
                    : "system"
            ),
            ("@time", VietnamTime.Now),
            ("@description", description)
        );
    }

    // =====================================================
    // NOTIFICATION
    // =====================================================

    private async Task InsertCheckInNotificationAsync(
        VisitAppointmentSource appointment,
        long visitId,
        string? roomName,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var text =
            appointment.Type ==
            "EXAMINATION"
                ? $"Bạn đã check-in khám thành công. Số lượt đã được ghi nhận{(string.IsNullOrWhiteSpace(roomName) ? "." : $" tại {roomName}.")}"
                : $"Bạn đã check-in xét nghiệm thành công{(string.IsNullOrWhiteSpace(roomName) ? "." : $" tại {roomName}.")}";

        await InsertCustomerNotificationAsync(
            connection,
            transaction,
            appointment.CustomerId,
            appointment.Type ==
                "EXAMINATION"
                ? "kham_benh"
                : "xet_nghiem",
            "Check-in thành công",
            text,
            appointment.Type ==
                "EXAMINATION"
                ? "luotkham"
                : "luotxetnghiem",
            visitId.ToString(),
            cancellationToken
        );
    }

    private async Task InsertCustomerNotificationAsync(
        DbConnection connection,
        DbTransaction transaction,
        string customerId,
        string notificationType,
        string title,
        string content,
        string objectType,
        string objectId,
        CancellationToken cancellationToken
    )
    {
        var userId =
            await ScalarAsync(
                """
                SELECT UserID
                FROM users
                WHERE
                    IDKhachHang = @customerId
                    AND IsActive = 1
                LIMIT 1
                """,
                cancellationToken,
                connection,
                transaction,
                ("@customerId", customerId)
            );

        if (
            userId is null
        )
        {
            return;
        }

        await ExecuteAsync(
            """
            INSERT INTO thongbao
            (
                UserIDNhan,
                LoaiThongBao,
                TieuDe,
                NoiDung,
                LoaiDoiTuong,
                IDDoiTuong,
                DaDoc,
                ThoiGianTao
            )
            VALUES
            (
                @userId,
                @type,
                @title,
                @content,
                @objectType,
                @objectId,
                0,
                @time
            )
            """,
            cancellationToken,
            connection,
            transaction,
            ("@userId", Convert.ToInt32(userId)),
            ("@type", notificationType),
            ("@title", title),
            ("@content", content),
            ("@objectType", objectType),
            ("@objectId", objectId),
            ("@time", VietnamTime.Now)
        );
    }

    // =====================================================
    // MAPPERS
    // =====================================================

    private static VisitAppointmentSource MapAppointment(
        IReadOnlyDictionary<string, object?> row
    )
    {
        return new VisitAppointmentSource(
            GetLong(row, "DbId")!.Value,
            GetString(row, "Code")!,
            GetString(row, "VisitType")!,
            GetInt(row, "BookedUserId"),
            GetString(row, "CustomerId")!,
            GetInt(row, "DoctorId"),
            GetString(row, "SpecialtyId"),
            GetString(row, "FacilityId")!,
            GetDate(row, "AppointmentDate"),
            GetTime(row, "AppointmentTime"),
            GetString(row, "AppointmentStatus")!,
            GetString(row, "Note"),
            GetString(row, "QrCode")
        );
    }

    private static Visit MapExaminationVisit(
        IReadOnlyDictionary<string, object?> row
    )
    {
        return new Visit
        {
            Id =
                GetLong(
                    row,
                    "VisitId"
                )!.Value.ToString(),

            AppointmentId =
                GetString(
                    row,
                    "MaDatLich"
                )!,

            CustomerId =
                GetString(
                    row,
                    "IDKhachHang"
                )!,

            DoctorId =
                GetInt(
                    row,
                    "IDBacSi"
                )?.ToString(),

            RoomId =
                GetString(
                    row,
                    "IDPhong"
                ),

            Type =
                "EXAMINATION",

            QueueNumber =
                GetInt(
                    row,
                    "SoThuTu"
                ),

            ReceivedAt =
                GetDateTime(
                    row,
                    "ThoiGianTiepNhan"
                ),

            StartedAt =
                GetDateTime(
                    row,
                    "ThoiGianBatDau"
                ),

            EndedAt =
                GetDateTime(
                    row,
                    "ThoiGianKetThuc"
                ),

            Status =
                GetString(
                    row,
                    "TrangThai"
                )!,

            Note =
                GetString(
                    row,
                    "GhiChu"
                )
        };
    }

    private static Visit MapTestVisit(
        IReadOnlyDictionary<string, object?> row
    )
    {
        return new Visit
        {
            Id =
                GetLong(
                    row,
                    "VisitId"
                )!.Value.ToString(),

            AppointmentId =
                GetString(
                    row,
                    "MaDatLich"
                )!,

            CustomerId =
                GetString(
                    row,
                    "IDKhachHang"
                )!,

            DoctorId =
                GetInt(
                    row,
                    "IDBacSi"
                )?.ToString(),

            RoomId =
                GetString(
                    row,
                    "IDPhong"
                ),

            Type =
                "TEST",

            QueueNumber =
                GetInt(
                    row,
                    "SoThuTu"
                ),

            ReceivedAt =
                GetDateTime(
                    row,
                    "ThoiGianTiepNhan"
                ),

            StartedAt =
                GetDateTime(
                    row,
                    "ThoiGianBatDau"
                ),

            EndedAt =
                GetDateTime(
                    row,
                    "ThoiGianKetThuc"
                ),

            Status =
                GetString(
                    row,
                    "TrangThai"
                )!,

            Note =
                GetString(
                    row,
                    "GhiChu"
                )
        };
    }

    private static VisitRoomData MapRoom(
        IReadOnlyDictionary<string, object?> row
    )
    {
        return new VisitRoomData(
            GetString(row, "IDPhong")!,
            GetString(row, "TenPhong")!,
            GetString(row, "CoSoID")!,
            GetString(row, "IDChuyenKhoa"),
            GetString(row, "LoaiPhong")!
        );
    }

    // =====================================================
    // NORMALIZE
    // =====================================================

    private static string NormalizeTypeFromCodeOrInput(
        string appointmentCode,
        string? type
    )
    {
        if (
            !string.IsNullOrWhiteSpace(
                type
            )
        )
        {
            return NormalizeVisitType(
                type
            );
        }

        var code =
            appointmentCode.Trim()
                .ToUpperInvariant();

        if (
            code.StartsWith(
                "DLXN",
                StringComparison.Ordinal
            )
        )
        {
            return "TEST";
        }

        if (
            code.StartsWith(
                "DLK",
                StringComparison.Ordinal
            )
        )
        {
            return "EXAMINATION";
        }

        throw new BadRequestException(
            "Không xác định được loại lịch hẹn. Vui lòng gửi type."
        );
    }

    private static string NormalizeVisitType(
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
                    "Loại lượt không hợp lệ."
                )
        };
    }

    private static string NormalizeQueueType(
        string? type
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                type
            )
        )
        {
            return "ALL";
        }

        var value =
            type.Trim()
                .ToUpperInvariant();

        return value switch
        {
            "ALL" =>
                "ALL",

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
                    "Loại hàng chờ không hợp lệ."
                )
        };
    }

    // =====================================================
    // TEXT HELPERS
    // =====================================================

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

    private static string GenerateQr()
    {
        return
            "QR-"
            +
            Guid.NewGuid()
                .ToString("N")
                .ToUpperInvariant();
    }

    private static string? BuildNote(
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

        var result =
            (r, n) switch
            {
                (not null, not null) =>
                    $"Lý do: {r}\nGhi chú: {n}",

                (not null, null) =>
                    $"Lý do: {r}",

                (null, not null) =>
                    n,

                _ =>
                    null
            };

        return Truncate(
            result,
            500
        );
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

    private static string? Truncate(
        string? value,
        int length
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

        var text =
            value.Trim();

        return text.Length <= length
            ? text
            : text[..length];
    }

    // =====================================================
    // RAW DATABASE HELPERS
    // =====================================================

    private async Task<List<Dictionary<string, object?>>> QueryAsync(
        string sql,
        CancellationToken cancellationToken,
        params (string Name, object? Value)[] parameters
    )
    {
        return await QueryAsync(
            sql,
            cancellationToken,
            null,
            null,
            parameters
        );
    }

    private async Task<List<Dictionary<string, object?>>> QueryAsync(
        string sql,
        CancellationToken cancellationToken,
        DbConnection? existingConnection,
        DbTransaction? transaction,
        params (string Name, object? Value)[] parameters
    )
    {
        var connection =
            existingConnection
            ?? _dbContext.Database
                .GetDbConnection();

        var closeAfter =
            existingConnection is null
            &&
            connection.State !=
            ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                sql;

            command.Transaction =
                transaction;

            AddParameters(
                command,
                parameters
            );

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken
                );

            var result =
                new List<
                    Dictionary<
                        string,
                        object?
                    >
                >();

            while (
                await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                var row =
                    new Dictionary<
                        string,
                        object?
                    >(
                        StringComparer
                            .OrdinalIgnoreCase
                    );

                for (
                    var index = 0;
                    index <
                    reader.FieldCount;
                    index++
                )
                {
                    row[
                        reader.GetName(
                            index
                        )
                    ] =
                        reader.IsDBNull(
                            index
                        )
                            ? null
                            : reader.GetValue(
                                index
                            );
                }

                result.Add(
                    row
                );
            }

            return result;
        }
        finally
        {
            if (closeAfter)
            {
                await connection
                    .CloseAsync();
            }
        }
    }

    private async Task<int> ExecuteAsync(
        string sql,
        CancellationToken cancellationToken,
        params (string Name, object? Value)[] parameters
    )
    {
        return await ExecuteAsync(
            sql,
            cancellationToken,
            null,
            null,
            parameters
        );
    }

    private async Task<int> ExecuteAsync(
        string sql,
        CancellationToken cancellationToken,
        DbConnection? existingConnection,
        DbTransaction? transaction,
        params (string Name, object? Value)[] parameters
    )
    {
        var connection =
            existingConnection
            ?? _dbContext.Database
                .GetDbConnection();

        var closeAfter =
            existingConnection is null
            &&
            connection.State !=
            ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                sql;

            command.Transaction =
                transaction;

            AddParameters(
                command,
                parameters
            );

            return await command
                .ExecuteNonQueryAsync(
                    cancellationToken
                );
        }
        finally
        {
            if (closeAfter)
            {
                await connection
                    .CloseAsync();
            }
        }
    }

    private async Task<object?> ScalarAsync(
        string sql,
        CancellationToken cancellationToken,
        params (string Name, object? Value)[] parameters
    )
    {
        return await ScalarAsync(
            sql,
            cancellationToken,
            null,
            null,
            parameters
        );
    }

    private async Task<object?> ScalarAsync(
        string sql,
        CancellationToken cancellationToken,
        DbConnection? existingConnection,
        DbTransaction? transaction,
        params (string Name, object? Value)[] parameters
    )
    {
        var connection =
            existingConnection
            ?? _dbContext.Database
                .GetDbConnection();

        var closeAfter =
            existingConnection is null
            &&
            connection.State !=
            ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                sql;

            command.Transaction =
                transaction;

            AddParameters(
                command,
                parameters
            );

            return await command
                .ExecuteScalarAsync(
                    cancellationToken
                );
        }
        finally
        {
            if (closeAfter)
            {
                await connection
                    .CloseAsync();
            }
        }
    }

    private static void AddParameters(
        DbCommand command,
        IEnumerable<
            (string Name, object? Value)
        > parameters
    )
    {
        foreach (
            var item in parameters
        )
        {
            var parameter =
                command.CreateParameter();

            parameter.ParameterName =
                item.Name;

            parameter.Value =
                item.Value
                ?? DBNull.Value;

            command.Parameters.Add(
                parameter
            );
        }
    }

    // =====================================================
    // VALUES
    // =====================================================

    private static string? GetString(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(
                   key,
                   out var value
               )
               &&
               value is not null
            ? Convert.ToString(
                value,
                CultureInfo.InvariantCulture
            )
            : null;
    }

    private static int? GetInt(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(
                   key,
                   out var value
               )
               &&
               value is not null
            ? Convert.ToInt32(
                value
            )
            : null;
    }

    private static long? GetLong(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(
                   key,
                   out var value
               )
               &&
               value is not null
            ? Convert.ToInt64(
                value
            )
            : null;
    }

    private static decimal? GetDecimal(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(
                   key,
                   out var value
               )
               &&
               value is not null
            ? Convert.ToDecimal(
                value
            )
            : null;
    }

    private static DateTime GetDate(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return Convert.ToDateTime(
            row[key]
        ).Date;
    }

    private static DateTime? GetDateTime(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(
                   key,
                   out var value
               )
               &&
               value is not null
            ? DateTime.SpecifyKind(
                Convert.ToDateTime(
                    value
                ),
                DateTimeKind.Unspecified
            )
            : null;
    }

    private static TimeSpan GetTime(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        var value =
            row[key]!;

        if (
            value is TimeSpan time
        )
        {
            return time;
        }

        return TimeSpan.Parse(
            value.ToString()!,
            CultureInfo.InvariantCulture
        );
    }

    private static string FormatTime(
        TimeSpan time
    )
    {
        return
            $"{(int)time.TotalHours:00}:{time.Minutes:00}";
    }
}