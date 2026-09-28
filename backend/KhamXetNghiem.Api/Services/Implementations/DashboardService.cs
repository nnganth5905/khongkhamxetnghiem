using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Services.Interfaces;
using KhamXetNghiem.Api.Utilities;

using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class DashboardService
    : IDashboardService
{
    private readonly AppDbContext _dbContext;

    public DashboardService(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =========================================================
    // RECEPTIONIST
    // =========================================================

    public async Task<ReceptionistDashboardResponse>
        GetReceptionistAsync(
            CancellationToken cancellationToken = default
        )
    {
        var now =
            VietnamTime.Now;

        var today =
            now.Date;

        var through =
            today.AddDays(7);

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            await EnsureOpenAsync(
                connection,
                cancellationToken
            );

        try
        {
            // -------------------------------------------------
            // Tổng lịch hẹn hôm nay
            // -------------------------------------------------

            var todayAppointments =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)
                        FROM datlichkham
                        WHERE
                            NgayKham = @today
                            AND TrangThai <> 'cancelled'
                    )
                    +
                    (
                        SELECT COUNT(*)
                        FROM datlichxetnghiem
                        WHERE
                            NgayXetNghiem = @today
                            AND TrangThai <> 'cancelled'
                    );
                    """,
                    cancellationToken,
                    ("@today", today)
                );

            // -------------------------------------------------
            // Đã check-in hôm nay
            //
            // luotkham / luotxetnghiem chỉ được tạo sau check-in.
            // -------------------------------------------------

            var checkedIn =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)
                        FROM luotkham lk
                        JOIN datlichkham d
                            ON d.IDDatLichKham =
                               lk.IDDatLichKham
                        WHERE d.NgayKham = @today
                    )
                    +
                    (
                        SELECT COUNT(*)
                        FROM luotxetnghiem lx
                        JOIN datlichxetnghiem d
                            ON d.IDDatLichXN =
                               lx.IDDatLichXN
                        WHERE d.NgayXetNghiem = @today
                    );
                    """,
                    cancellationToken,
                    ("@today", today)
                );

            // -------------------------------------------------
            // Đang chờ
            // -------------------------------------------------

            var waiting =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)
                        FROM luotkham lk
                        JOIN datlichkham d
                            ON d.IDDatLichKham =
                               lk.IDDatLichKham
                        WHERE
                            d.NgayKham = @today
                            AND lk.TrangThai IN
                            (
                                'da_tiep_nhan',
                                'cho_kham',
                                'da_den_luot',
                                'cho_goi_lai'
                            )
                    )
                    +
                    (
                        SELECT COUNT(*)
                        FROM luotxetnghiem lx
                        JOIN datlichxetnghiem d
                            ON d.IDDatLichXN =
                               lx.IDDatLichXN
                        WHERE
                            d.NgayXetNghiem = @today
                            AND lx.TrangThai IN
                            (
                                'da_tiep_nhan',
                                'cho_xet_nghiem',
                                'da_den_luot'
                            )
                    );
                    """,
                    cancellationToken,
                    ("@today", today)
                );

            // -------------------------------------------------
            // WALK-IN
            //
            // DB hiện chưa có cột Source/AppointmentSource.
            // Giữ quy ước cũ: UserID NULL = đặt tại quầy/vãng lai.
            // -------------------------------------------------

            var walkIns =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)
                        FROM datlichkham
                        WHERE
                            NgayKham = @today
                            AND UserID IS NULL
                            AND TrangThai <> 'cancelled'
                    )
                    +
                    (
                        SELECT COUNT(*)
                        FROM datlichxetnghiem
                        WHERE
                            NgayXetNghiem = @today
                            AND UserID IS NULL
                            AND TrangThai <> 'cancelled'
                    );
                    """,
                    cancellationToken,
                    ("@today", today)
                );

            var upcoming =
                await GetUpcomingAppointmentsAsync(
                    connection,
                    today,
                    through,
                    10,
                    cancellationToken
                );

            return new ReceptionistDashboardResponse
            {
                TodayAppointments =
                    todayAppointments,

                CheckedIn =
                    checkedIn,

                Waiting =
                    waiting,

                WalkIns =
                    walkIns,

                UpcomingAppointments =
                    upcoming
            };
        }
        finally
        {
            await CloseIfNeededAsync(
                connection,
                closeAfter
            );
        }
    }

    // =========================================================
    // DOCTOR
    // =========================================================

    public async Task<DoctorDashboardResponse>
        GetDoctorAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var actor =
            await GetActorAsync(
                user,
                cancellationToken
            );

        if (actor?.DoctorId is null)
        {
            throw new ForbiddenException(
                "Tài khoản bác sĩ chưa liên kết hồ sơ bác sĩ."
            );
        }

        var doctorId =
            actor.DoctorId.Value;

        var now =
            VietnamTime.Now;

        var today =
            now.Date;

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            await EnsureOpenAsync(
                connection,
                cancellationToken
            );

        try
        {
            // -------------------------------------------------
            // Tổng lượt khám hôm nay
            // -------------------------------------------------

            var todayVisits =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM luotkham lk

                    JOIN datlichkham d
                        ON d.IDDatLichKham =
                           lk.IDDatLichKham

                    WHERE
                        lk.IDBacSi = @doctorId
                        AND d.NgayKham = @today;
                    """,
                    cancellationToken,
                    ("@doctorId", doctorId),
                    ("@today", today)
                );

            // -------------------------------------------------
            // Đang chờ
            // -------------------------------------------------

            var waitingCount =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM luotkham lk

                    JOIN datlichkham d
                        ON d.IDDatLichKham =
                           lk.IDDatLichKham

                    WHERE
                        lk.IDBacSi = @doctorId
                        AND d.NgayKham = @today

                        AND lk.TrangThai IN
                        (
                            'da_tiep_nhan',
                            'cho_kham',
                            'da_den_luot',
                            'cho_goi_lai'
                        );
                    """,
                    cancellationToken,
                    ("@doctorId", doctorId),
                    ("@today", today)
                );

            // -------------------------------------------------
            // Hoàn tất
            //
            // Schema luotkham thật dùng "hoan_tat".
            // -------------------------------------------------

            var completedToday =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM luotkham lk

                    JOIN datlichkham d
                        ON d.IDDatLichKham =
                           lk.IDDatLichKham

                    WHERE
                        lk.IDBacSi = @doctorId
                        AND d.NgayKham = @today
                        AND lk.TrangThai = 'hoan_tat';
                    """,
                    cancellationToken,
                    ("@doctorId", doctorId),
                    ("@today", today)
                );

            // -------------------------------------------------
            // Kết quả chờ duyệt thuộc bác sĩ
            // -------------------------------------------------

            var pendingResultRows =
                await QueryAsync(
                    connection,
                    """
                    SELECT
                        kq.IDKetQua,
                        kh.TenKhachHang,
                        xn.TenXetNghiem,
                        nv.TenNhanVien
                            AS TechnicianName,
                        kq.ThoiGianHoanThanh,

                        EXISTS
                        (
                            SELECT 1
                            FROM ketquachiso kqc
                            WHERE
                                kqc.IDKetQua =
                                kq.IDKetQua

                                AND kqc.Status = 'yes'

                                AND kqc.DanhGia IN
                                (
                                    'thap',
                                    'cao',
                                    'bat_thuong'
                                )
                        ) AS HasAbnormal

                    FROM ketquaxetnghiem kq

                    JOIN ctphieuxetnghiem ct
                        ON ct.IDCTPhieu =
                           kq.IDCTPhieu

                    JOIN phieuxetnghiem px
                        ON px.IDPhieuXetNghiem =
                           ct.IDPhieuXetNghiem

                    JOIN khachhang kh
                        ON kh.IDKhachHang =
                           px.IDKhachHang

                    JOIN loaixetnghiem xn
                        ON xn.IDXetNghiem =
                           ct.IDXetNghiem

                    LEFT JOIN nhanvien nv
                        ON nv.IDNhanVien =
                           kq.IDKTV

                    LEFT JOIN luotxetnghiem lx
                        ON lx.IDLuotXetNghiem =
                           px.IDLuotXetNghiem

                    LEFT JOIN datlichxetnghiem dx
                        ON dx.IDDatLichXN =
                           lx.IDDatLichXN

                    WHERE
                        kq.TrangThai = 'cho_duyet'

                        AND
                        (
                            px.IDBacSiPhuTrach =
                                @doctorId

                            OR
                            (
                                px.IDBacSiPhuTrach
                                    IS NULL

                                AND dx.IDBacSi =
                                    @doctorId
                            )
                        )

                    ORDER BY
                        HasAbnormal DESC,
                        kq.ThoiGianHoanThanh

                    LIMIT 8;
                    """,
                    cancellationToken,
                    ("@doctorId", doctorId)
                );

            var pendingResults =
                pendingResultRows
                    .Select(
                        row =>
                            new DoctorPendingResultDashboardResponse
                            {
                                Id =
                                    GetString(
                                        row,
                                        "IDKetQua"
                                    ),

                                PatientName =
                                    GetString(
                                        row,
                                        "TenKhachHang"
                                    ),

                                TestName =
                                    GetString(
                                        row,
                                        "TenXetNghiem"
                                    ),

                                TechnicianName =
                                    GetString(
                                        row,
                                        "TechnicianName"
                                    ),

                                SubmittedAt =
                                    GetDateTime(
                                        row,
                                        "ThoiGianHoanThanh"
                                    ),

                                HasAbnormalIndicator =
                                    GetInt(
                                        row,
                                        "HasAbnormal"
                                    ) == 1
                            }
                    )
                    .ToList();

            var pendingApprovals =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM ketquaxetnghiem kq

                    JOIN ctphieuxetnghiem ct
                        ON ct.IDCTPhieu =
                           kq.IDCTPhieu

                    JOIN phieuxetnghiem px
                        ON px.IDPhieuXetNghiem =
                           ct.IDPhieuXetNghiem

                    LEFT JOIN luotxetnghiem lx
                        ON lx.IDLuotXetNghiem =
                           px.IDLuotXetNghiem

                    LEFT JOIN datlichxetnghiem dx
                        ON dx.IDDatLichXN =
                           lx.IDDatLichXN

                    WHERE
                        kq.TrangThai = 'cho_duyet'

                        AND
                        (
                            px.IDBacSiPhuTrach =
                                @doctorId

                            OR
                            (
                                px.IDBacSiPhuTrach
                                    IS NULL

                                AND dx.IDBacSi =
                                    @doctorId
                            )
                        );
                    """,
                    cancellationToken,
                    ("@doctorId", doctorId)
                );

            // -------------------------------------------------
            // Phòng hiện tại
            //
            // lichlamviec.TrangThai:
            // duoc_duyet / huy
            // -------------------------------------------------

            var roomRows =
                await QueryAsync(
                    connection,
                    """
                    SELECT
                        l.IDPhong,
                        p.TenPhong

                    FROM lichlamviec l

                    LEFT JOIN phong p
                        ON p.IDPhong =
                           l.IDPhong

                    WHERE
                        l.IDBacSi = @doctorId
                        AND l.Ngay = @today
                        AND l.TrangThai = 'duoc_duyet'

                    ORDER BY
                        CASE
                            WHEN
                                @nowTime >= l.GioBatDau
                                AND
                                @nowTime < l.GioKetThuc

                            THEN 0

                            ELSE 1
                        END,

                        l.GioBatDau

                    LIMIT 1;
                    """,
                    cancellationToken,
                    ("@doctorId", doctorId),
                    ("@today", today),
                    ("@nowTime", now.TimeOfDay)
                );

            var currentRoom =
                roomRows.Count == 0
                    ? "—"
                    : (
                        GetString(
                            roomRows[0],
                            "TenPhong"
                        ) switch
                        {
                            { Length: > 0 } roomName =>
                                roomName,

                            _ =>
                                GetString(
                                    roomRows[0],
                                    "IDPhong"
                                )
                        }
                    );

            if (string.IsNullOrWhiteSpace(currentRoom))
            {
                currentRoom = "—";
            }

            // -------------------------------------------------
            // Bệnh nhân tiếp theo
            // -------------------------------------------------

            var patientRows =
                await QueryAsync(
                    connection,
                    """
                    SELECT
                        lk.IDLuotKham,
                        lk.SoThuTu,
                        lk.IDKhachHang,
                        kh.TenKhachHang,
                        TIME_FORMAT(
                            d.GioKham,
                            '%H:%i'
                        ) AS GioKham,

                        COALESCE(
                            NULLIF(
                                TRIM(d.LyDoKham),
                                ''
                            ),
                            NULLIF(
                                TRIM(d.GhiChu),
                                ''
                            ),
                            'Khám bệnh'
                        ) AS Reason,

                        lk.TrangThai

                    FROM luotkham lk

                    JOIN datlichkham d
                        ON d.IDDatLichKham =
                           lk.IDDatLichKham

                    JOIN khachhang kh
                        ON kh.IDKhachHang =
                           lk.IDKhachHang

                    WHERE
                        lk.IDBacSi =
                            @doctorId

                        AND d.NgayKham =
                            @today

                        AND lk.TrangThai IN
                        (
                            'da_tiep_nhan',
                            'cho_kham',
                            'da_den_luot',
                            'cho_goi_lai'
                        )

                    ORDER BY
                        CASE
                            WHEN lk.TrangThai =
                                 'da_den_luot'
                            THEN 0

                            WHEN lk.TrangThai =
                                 'cho_kham'
                            THEN 1

                            WHEN lk.TrangThai =
                                 'da_tiep_nhan'
                            THEN 2

                            ELSE 3
                        END,

                        lk.SoThuTu,
                        d.GioKham

                    LIMIT 8;
                    """,
                    cancellationToken,
                    ("@doctorId", doctorId),
                    ("@today", today)
                );

            var nextPatients =
                patientRows
                    .Select(
                        row =>
                            new DoctorNextPatientResponse
                            {
                                Id =
                                    GetLong(
                                        row,
                                        "IDLuotKham"
                                    ) ?? 0,

                                QueueNumber =
                                    GetInt(
                                        row,
                                        "SoThuTu"
                                    ),

                                PatientCode =
                                    GetString(
                                        row,
                                        "IDKhachHang"
                                    ),

                                PatientName =
                                    GetString(
                                        row,
                                        "TenKhachHang"
                                    ),

                                Time =
                                    GetString(
                                        row,
                                        "GioKham"
                                    ),

                                Reason =
                                    GetString(
                                        row,
                                        "Reason"
                                    ),

                                Status =
                                    GetString(
                                        row,
                                        "TrangThai"
                                    )
                            }
                    )
                    .ToList();

            return new DoctorDashboardResponse
            {
                WaitingCount =
                    waitingCount,

                TodayVisits =
                    todayVisits,

                PendingApprovals =
                    pendingApprovals,

                CompletedToday =
                    completedToday,

                CurrentRoom =
                    currentRoom,

                NextPatients =
                    nextPatients,

                PendingResults =
                    pendingResults
            };
        }
        finally
        {
            await CloseIfNeededAsync(
                connection,
                closeAfter
            );
        }
    }

    // =========================================================
    // TECHNICIAN
    // =========================================================

    public async Task<TechnicianDashboardResponse>
        GetTechnicianAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var actor =
            await GetActorAsync(
                user,
                cancellationToken
            );

        if (string.IsNullOrWhiteSpace(actor?.EmployeeId))
        {
            throw new ForbiddenException(
                "Tài khoản kỹ thuật viên chưa liên kết hồ sơ nhân viên."
            );
        }

        if (
            !string.Equals(
                actor.EmployeePosition,
                "ktv",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new ForbiddenException(
                "Nhân viên hiện tại không phải kỹ thuật viên."
            );
        }

        var technicianId =
            actor.EmployeeId;

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            await EnsureOpenAsync(
                connection,
                cancellationToken
            );

        try
        {
            // -------------------------------------------------
            // Mẫu đã bàn giao CHO ĐÚNG KTV HIỆN TẠI
            // -------------------------------------------------

            var waitingSpecimens =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM maubenhpham m

                    WHERE
                        m.TrangThai =
                            'da_ban_giao'

                        AND EXISTS
                        (
                            SELECT 1

                            FROM bangiaomau bg

                            WHERE
                                bg.IDMau =
                                    m.IDMau

                                AND bg.IDKTVTiepNhan =
                                    @technicianId

                                AND bg.TrangThai =
                                    'da_ban_giao'
                        );
                    """,
                    cancellationToken,
                    (
                        "@technicianId",
                        technicianId
                    )
                );

            // -------------------------------------------------
            // Đã tiếp nhận
            // -------------------------------------------------

            var receivedSpecimens =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM maubenhpham m

                    WHERE
                        m.TrangThai =
                            'ktv_tiep_nhan'

                        AND EXISTS
                        (
                            SELECT 1

                            FROM bangiaomau bg

                            WHERE
                                bg.IDMau =
                                    m.IDMau

                                AND bg.IDKTVTiepNhan =
                                    @technicianId

                                AND bg.TrangThai =
                                    'da_tiep_nhan'
                        );
                    """,
                    cancellationToken,
                    (
                        "@technicianId",
                        technicianId
                    )
                );

            var inProgress =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM worklist

                    WHERE
                        IDKTV = @technicianId
                        AND Status = 'running';
                    """,
                    cancellationToken,
                    (
                        "@technicianId",
                        technicianId
                    )
                );

            var pendingResultEntries =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM worklist

                    WHERE
                        IDKTV = @technicianId
                        AND Status = 'to_result';
                    """,
                    cancellationToken,
                    (
                        "@technicianId",
                        technicianId
                    )
                );

            var rows =
                await QueryAsync(
                    connection,
                    """
                    SELECT
                        w.id,
                        w.IDMau,
                        COALESCE(
                            m.MaBarcode,
                            w.IDMau
                        ) AS SpecimenCode,

                        kh.TenKhachHang,

                        xn.TenXetNghiem,

                        w.IDKTV,
                        w.Status,
                        w.ReceivedAt,
                        w.StartedAt,
                        w.FinishedAt

                    FROM worklist w

                    JOIN ctphieuxetnghiem ct
                        ON ct.IDCTPhieu =
                           w.IDCTPhieu

                    JOIN phieuxetnghiem px
                        ON px.IDPhieuXetNghiem =
                           ct.IDPhieuXetNghiem

                    JOIN khachhang kh
                        ON kh.IDKhachHang =
                           px.IDKhachHang

                    JOIN loaixetnghiem xn
                        ON xn.IDXetNghiem =
                           ct.IDXetNghiem

                    LEFT JOIN maubenhpham m
                        ON m.IDMau =
                           w.IDMau

                    WHERE
                        w.IDKTV = @technicianId

                        AND w.Status IN
                        (
                            'queue',
                            'running',
                            'to_result',
                            'rerun'
                        )

                    ORDER BY
                        CASE w.Status
                            WHEN 'running' THEN 0
                            WHEN 'to_result' THEN 1
                            WHEN 'rerun' THEN 2
                            WHEN 'queue' THEN 3
                            ELSE 4
                        END,

                        w.ReceivedAt

                    LIMIT 10;
                    """,
                    cancellationToken,
                    (
                        "@technicianId",
                        technicianId
                    )
                );

            var worklist =
                rows.Select(
                    row =>
                        new TechnicianWorklistDashboardResponse
                        {
                            Id =
                                GetLong(
                                    row,
                                    "id"
                                ) ?? 0,

                            SpecimenId =
                                NullableString(
                                    row,
                                    "IDMau"
                                ),

                            SpecimenCode =
                                GetString(
                                    row,
                                    "SpecimenCode"
                                ),

                            PatientName =
                                GetString(
                                    row,
                                    "TenKhachHang"
                                ),

                            TestName =
                                GetString(
                                    row,
                                    "TenXetNghiem"
                                ),

                            TechnicianId =
                                NullableString(
                                    row,
                                    "IDKTV"
                                ),

                            Priority =
                                "NORMAL",

                            Status =
                                GetString(
                                    row,
                                    "Status"
                                ),

                            ReceivedAt =
                                GetDateTime(
                                    row,
                                    "ReceivedAt"
                                ),

                            StartedAt =
                                GetDateTime(
                                    row,
                                    "StartedAt"
                                ),

                            FinishedAt =
                                GetDateTime(
                                    row,
                                    "FinishedAt"
                                )
                        }
                ).ToList();

            return new TechnicianDashboardResponse
            {
                TechnicianId =
                    technicianId,

                WaitingSpecimens =
                    waitingSpecimens,

                ReceivedSpecimens =
                    receivedSpecimens,

                InProgress =
                    inProgress,

                PendingResultEntries =
                    pendingResultEntries,

                Worklist =
                    worklist
            };
        }
        finally
        {
            await CloseIfNeededAsync(
                connection,
                closeAfter
            );
        }
    }

    // =========================================================
    // ADMIN
    // =========================================================

    public async Task<AdminDashboardResponse>
        GetAdminAsync(
            CancellationToken cancellationToken = default
        )
    {
        var now =
            VietnamTime.Now;

        var today =
            now.Date;

        var through =
            today.AddDays(7);

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            await EnsureOpenAsync(
                connection,
                cancellationToken
            );

        try
        {
            var totalCustomers =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)
                    FROM khachhang
                    WHERE Status = 'yes';
                    """,
                    cancellationToken
                );

            var totalDoctors =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)
                    FROM bacsi
                    WHERE TrangThai = 'active';
                    """,
                    cancellationToken
                );

            var totalEmployees =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)
                    FROM nhanvien
                    WHERE Status = 'yes';
                    """,
                    cancellationToken
                );

            var totalTechnicians =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM nhanvien

                    WHERE
                        Status = 'yes'
                        AND ViTri = 'ktv';
                    """,
                    cancellationToken
                );

            var todayAppointments =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)
                        FROM datlichkham
                        WHERE
                            NgayKham = @today
                            AND TrangThai <>
                                'cancelled'
                    )
                    +
                    (
                        SELECT COUNT(*)
                        FROM datlichxetnghiem
                        WHERE
                            NgayXetNghiem = @today
                            AND TrangThai <>
                                'cancelled'
                    );
                    """,
                    cancellationToken,
                    ("@today", today)
                );

            var checkedInToday =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)

                        FROM luotkham lk

                        JOIN datlichkham d
                            ON d.IDDatLichKham =
                               lk.IDDatLichKham

                        WHERE d.NgayKham =
                              @today
                    )
                    +
                    (
                        SELECT COUNT(*)

                        FROM luotxetnghiem lx

                        JOIN datlichxetnghiem d
                            ON d.IDDatLichXN =
                               lx.IDDatLichXN

                        WHERE d.NgayXetNghiem =
                              @today
                    );
                    """,
                    cancellationToken,
                    ("@today", today)
                );

            var waitingNow =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)
                        FROM luotkham
                        WHERE TrangThai IN
                        (
                            'da_tiep_nhan',
                            'cho_kham',
                            'da_den_luot',
                            'cho_goi_lai'
                        )
                    )
                    +
                    (
                        SELECT COUNT(*)
                        FROM luotxetnghiem
                        WHERE TrangThai IN
                        (
                            'da_tiep_nhan',
                            'cho_xet_nghiem',
                            'da_den_luot',
                            'dang_lay_mau',
                            'da_lay_mau',
                            'ktv_tiep_nhan',
                            'dang_xet_nghiem',
                            'cho_duyet_kq'
                        )
                    );
                    """,
                    cancellationToken
                );

            var pendingResults =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM ketquaxetnghiem

                    WHERE TrangThai =
                          'cho_duyet';
                    """,
                    cancellationToken
                );

            var activeWorklists =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM worklist

                    WHERE Status IN
                    (
                        'queue',
                        'running',
                        'to_result',
                        'rerun'
                    );
                    """,
                    cancellationToken
                );

            var recent =
                await GetUpcomingAppointmentsAsync(
                    connection,
                    today,
                    through,
                    10,
                    cancellationToken
                );

            return new AdminDashboardResponse
            {
                TotalCustomers =
                    totalCustomers,

                TotalDoctors =
                    totalDoctors,

                TotalEmployees =
                    totalEmployees,

                TotalTechnicians =
                    totalTechnicians,

                TodayAppointments =
                    todayAppointments,

                CheckedInToday =
                    checkedInToday,

                WaitingNow =
                    waitingNow,

                PendingResults =
                    pendingResults,

                ActiveWorklists =
                    activeWorklists,

                RecentAppointments =
                    recent
            };
        }
        finally
        {
            await CloseIfNeededAsync(
                connection,
                closeAfter
            );
        }
    }

    // =========================================================
    // CUSTOMER
    // =========================================================

    public async Task<CustomerDashboardResponse>
        GetCustomerAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var actor =
            await GetActorAsync(
                user,
                cancellationToken
            );

        if (string.IsNullOrWhiteSpace(actor?.CustomerId))
        {
            throw new ForbiddenException(
                "Tài khoản chưa liên kết hồ sơ khách hàng."
            );
        }

        var customerId =
            actor.CustomerId;

        var today =
            VietnamTime.Now.Date;

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            await EnsureOpenAsync(
                connection,
                cancellationToken
            );

        try
        {
            var todayAppointments =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)

                        FROM datlichkham

                        WHERE
                            IDKhachHang =
                                @customerId

                            AND NgayKham =
                                @today

                            AND TrangThai <>
                                'cancelled'
                    )
                    +
                    (
                        SELECT COUNT(*)

                        FROM datlichxetnghiem

                        WHERE
                            IDKhachHang =
                                @customerId

                            AND NgayXetNghiem =
                                @today

                            AND TrangThai <>
                                'cancelled'
                    );
                    """,
                    cancellationToken,
                    ("@customerId", customerId),
                    ("@today", today)
                );

            var upcomingCount =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)

                        FROM datlichkham

                        WHERE
                            IDKhachHang =
                                @customerId

                            AND NgayKham >=
                                @today

                            AND TrangThai IN
                            (
                                'pending',
                                'confirmed'
                            )
                    )
                    +
                    (
                        SELECT COUNT(*)

                        FROM datlichxetnghiem

                        WHERE
                            IDKhachHang =
                                @customerId

                            AND NgayXetNghiem >=
                                @today

                            AND TrangThai IN
                            (
                                'pending',
                                'confirmed'
                            )
                    );
                    """,
                    cancellationToken,
                    ("@customerId", customerId),
                    ("@today", today)
                );

            var activeVisits =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT
                    (
                        SELECT COUNT(*)

                        FROM luotkham

                        WHERE
                            IDKhachHang =
                                @customerId

                            AND TrangThai NOT IN
                            (
                                'hoan_tat',
                                'bo_luot'
                            )
                    )
                    +
                    (
                        SELECT COUNT(*)

                        FROM luotxetnghiem

                        WHERE
                            IDKhachHang =
                                @customerId

                            AND TrangThai NOT IN
                            (
                                'hoan_tat',
                                'huy'
                            )
                    );
                    """,
                    cancellationToken,
                    ("@customerId", customerId)
                );

            var approvedResults =
                await ScalarIntAsync(
                    connection,
                    """
                    SELECT COUNT(*)

                    FROM ketquaxetnghiem kq

                    JOIN ctphieuxetnghiem ct
                        ON ct.IDCTPhieu =
                           kq.IDCTPhieu

                    JOIN phieuxetnghiem px
                        ON px.IDPhieuXetNghiem =
                           ct.IDPhieuXetNghiem

                    WHERE
                        px.IDKhachHang =
                            @customerId

                        AND kq.TrangThai IN
                        (
                            'da_duyet',
                            'hoan_tat'
                        );
                    """,
                    cancellationToken,
                    ("@customerId", customerId)
                );

            var rows =
                await QueryAsync(
                    connection,
                    """
                    SELECT *

                    FROM
                    (
                        SELECT
                            dk.MaDatLich AS Id,

                            DATE_FORMAT(
                                dk.NgayKham,
                                '%Y-%m-%d'
                            ) AS Date,

                            TIME_FORMAT(
                                dk.GioKham,
                                '%H:%i'
                            ) AS Time,

                            kh.TenKhachHang
                                AS PatientName,

                            'EXAMINATION'
                                AS Type,

                            COALESCE(
                                ck.TenChuyenKhoa,
                                'Khám bệnh'
                            ) AS ServiceName,

                            dk.TrangThai
                                AS Status

                        FROM datlichkham dk

                        JOIN khachhang kh
                            ON kh.IDKhachHang =
                               dk.IDKhachHang

                        LEFT JOIN chuyenkhoa ck
                            ON ck.IDChuyenKhoa =
                               dk.IDChuyenKhoa

                        WHERE
                            dk.IDKhachHang =
                                @customerId

                            AND dk.NgayKham >=
                                @today

                            AND dk.TrangThai IN
                            (
                                'pending',
                                'confirmed'
                            )

                        UNION ALL

                        SELECT
                            dx.MaDatLich AS Id,

                            DATE_FORMAT(
                                dx.NgayXetNghiem,
                                '%Y-%m-%d'
                            ) AS Date,

                            TIME_FORMAT(
                                dx.GioXetNghiem,
                                '%H:%i'
                            ) AS Time,

                            kh.TenKhachHang
                                AS PatientName,

                            'TEST'
                                AS Type,

                            COALESCE
                            (
                                (
                                    SELECT
                                        GROUP_CONCAT(
                                            xn.TenXetNghiem
                                            ORDER BY
                                                xn.TenXetNghiem
                                            SEPARATOR ', '
                                        )

                                    FROM ctdatlichxetnghiem c

                                    JOIN loaixetnghiem xn
                                        ON xn.IDXetNghiem =
                                           c.IDXetNghiem

                                    WHERE
                                        c.IDDatLichXN =
                                        dx.IDDatLichXN
                                ),

                                'Xét nghiệm'
                            ) AS ServiceName,

                            dx.TrangThai
                                AS Status

                        FROM datlichxetnghiem dx

                        JOIN khachhang kh
                            ON kh.IDKhachHang =
                               dx.IDKhachHang

                        WHERE
                            dx.IDKhachHang =
                                @customerId

                            AND dx.NgayXetNghiem >=
                                @today

                            AND dx.TrangThai IN
                            (
                                'pending',
                                'confirmed'
                            )
                    ) q

                    ORDER BY
                        q.Date,
                        q.Time

                    LIMIT 5;
                    """,
                    cancellationToken,
                    ("@customerId", customerId),
                    ("@today", today)
                );

            return new CustomerDashboardResponse
            {
                TodayAppointments =
                    todayAppointments,

                UpcomingAppointmentsCount =
                    upcomingCount,

                ActiveVisits =
                    activeVisits,

                ApprovedResults =
                    approvedResults,

                UpcomingAppointments =
                    MapAppointments(rows)
            };
        }
        finally
        {
            await CloseIfNeededAsync(
                connection,
                closeAfter
            );
        }
    }

    // =========================================================
    // UPCOMING APPOINTMENTS
    // =========================================================

    private async Task<List<DashboardUpcomingAppointment>>
        GetUpcomingAppointmentsAsync(
            DbConnection connection,
            DateTime from,
            DateTime through,
            int limit,
            CancellationToken cancellationToken
        )
    {
        var rows =
            await QueryAsync(
                connection,
                """
                SELECT *

                FROM
                (
                    SELECT
                        dk.MaDatLich AS Id,

                        DATE_FORMAT(
                            dk.NgayKham,
                            '%Y-%m-%d'
                        ) AS Date,

                        TIME_FORMAT(
                            dk.GioKham,
                            '%H:%i'
                        ) AS Time,

                        kh.TenKhachHang
                            AS PatientName,

                        'EXAMINATION'
                            AS Type,

                        COALESCE(
                            ck.TenChuyenKhoa,
                            'Khám bệnh'
                        ) AS ServiceName,

                        dk.TrangThai
                            AS Status

                    FROM datlichkham dk

                    JOIN khachhang kh
                        ON kh.IDKhachHang =
                           dk.IDKhachHang

                    LEFT JOIN chuyenkhoa ck
                        ON ck.IDChuyenKhoa =
                           dk.IDChuyenKhoa

                    WHERE
                        dk.NgayKham >= @from
                        AND dk.NgayKham <= @through

                        AND dk.TrangThai IN
                        (
                            'pending',
                            'confirmed'
                        )

                    UNION ALL

                    SELECT
                        dx.MaDatLich AS Id,

                        DATE_FORMAT(
                            dx.NgayXetNghiem,
                            '%Y-%m-%d'
                        ) AS Date,

                        TIME_FORMAT(
                            dx.GioXetNghiem,
                            '%H:%i'
                        ) AS Time,

                        kh.TenKhachHang
                            AS PatientName,

                        'TEST'
                            AS Type,

                        COALESCE
                        (
                            (
                                SELECT
                                    GROUP_CONCAT(
                                        xn.TenXetNghiem
                                        ORDER BY
                                            xn.TenXetNghiem
                                        SEPARATOR ', '
                                    )

                                FROM ctdatlichxetnghiem c

                                JOIN loaixetnghiem xn
                                    ON xn.IDXetNghiem =
                                       c.IDXetNghiem

                                WHERE
                                    c.IDDatLichXN =
                                    dx.IDDatLichXN
                            ),

                            'Xét nghiệm'
                        ) AS ServiceName,

                        dx.TrangThai
                            AS Status

                    FROM datlichxetnghiem dx

                    JOIN khachhang kh
                        ON kh.IDKhachHang =
                           dx.IDKhachHang

                    WHERE
                        dx.NgayXetNghiem >= @from
                        AND dx.NgayXetNghiem <= @through

                        AND dx.TrangThai IN
                        (
                            'pending',
                            'confirmed'
                        )
                ) q

                ORDER BY
                    q.Date,
                    q.Time

                LIMIT @limit;
                """,
                cancellationToken,
                ("@from", from),
                ("@through", through),
                ("@limit", limit)
            );

        return MapAppointments(
            rows
        );
    }

    private static List<DashboardUpcomingAppointment>
        MapAppointments(
            IEnumerable<Dictionary<string, object?>> rows
        )
    {
        return rows.Select(
            row =>
                new DashboardUpcomingAppointment
                {
                    Id =
                        GetString(
                            row,
                            "Id"
                        ),

                    Date =
                        GetString(
                            row,
                            "Date"
                        ),

                    Time =
                        GetString(
                            row,
                            "Time"
                        ),

                    PatientName =
                        GetString(
                            row,
                            "PatientName"
                        ),

                    Type =
                        GetString(
                            row,
                            "Type"
                        ),

                    ServiceName =
                        GetString(
                            row,
                            "ServiceName"
                        ),

                    Status =
                        GetString(
                            row,
                            "Status"
                        )
                }
        ).ToList();
    }

    // =========================================================
    // ACTOR
    // =========================================================

    private async Task<DashboardActor?> GetActorAsync(
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
            user.FindFirstValue(
                ClaimTypes.Name
            )
            ??
            user.Identity?.Name;

        if (string.IsNullOrWhiteSpace(login))
        {
            return null;
        }

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            await EnsureOpenAsync(
                connection,
                cancellationToken
            );

        try
        {
            var rows =
                await QueryAsync(
                    connection,
                    """
                    SELECT
                        u.UserID,
                        u.IDKhachHang,
                        u.IDBacSi,
                        u.IDNhanVien,
                        u.Role,

                        nv.ViTri,
                        nv.Status AS EmployeeStatus

                    FROM users u

                    LEFT JOIN nhanvien nv
                        ON nv.IDNhanVien =
                           u.IDNhanVien

                    WHERE
                        u.IsActive = 1

                        AND
                        (
                            u.Email = @login
                            OR u.Username = @login
                        )

                    LIMIT 1;
                    """,
                    cancellationToken,
                    ("@login", login.Trim())
                );

            if (rows.Count == 0)
            {
                return null;
            }

            var row =
                rows[0];

            return new DashboardActor
            {
                UserId =
                    GetInt(
                        row,
                        "UserID"
                    ) ?? 0,

                CustomerId =
                    NullableString(
                        row,
                        "IDKhachHang"
                    ),

                DoctorId =
                    GetInt(
                        row,
                        "IDBacSi"
                    ),

                EmployeeId =
                    NullableString(
                        row,
                        "IDNhanVien"
                    ),

                Role =
                    GetString(
                        row,
                        "Role"
                    ),

                EmployeePosition =
                    NullableString(
                        row,
                        "ViTri"
                    ),

                EmployeeStatus =
                    NullableString(
                        row,
                        "EmployeeStatus"
                    )
            };
        }
        finally
        {
            await CloseIfNeededAsync(
                connection,
                closeAfter
            );
        }
    }

    // =========================================================
    // DATABASE HELPERS
    // =========================================================

    private static async Task<bool> EnsureOpenAsync(
        DbConnection connection,
        CancellationToken cancellationToken
    )
    {
        var closeAfter =
            connection.State !=
            ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        return closeAfter;
    }

    private static async Task CloseIfNeededAsync(
        DbConnection connection,
        bool close
    )
    {
        if (
            close
            &&
            connection.State ==
            ConnectionState.Open
        )
        {
            await connection.CloseAsync();
        }
    }

    private static async Task<int> ScalarIntAsync(
        DbConnection connection,
        string sql,
        CancellationToken cancellationToken,
        params (
            string Name,
            object? Value
        )[] parameters
    )
    {
        await using var command =
            connection.CreateCommand();

        command.CommandText =
            sql;

        AddParameters(
            command,
            parameters
        );

        var result =
            await command.ExecuteScalarAsync(
                cancellationToken
            );

        if (
            result is null
            ||
            result == DBNull.Value
        )
        {
            return 0;
        }

        return Convert.ToInt32(
            result
        );
    }

    private static async Task<List<Dictionary<string, object?>>>
        QueryAsync(
            DbConnection connection,
            string sql,
            CancellationToken cancellationToken,
            params (
                string Name,
                object? Value
            )[] parameters
        )
    {
        await using var command =
            connection.CreateCommand();

        command.CommandText =
            sql;

        AddParameters(
            command,
            parameters
        );

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken
            );

        var rows =
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
                    StringComparer.OrdinalIgnoreCase
                );

            for (
                var index = 0;
                index < reader.FieldCount;
                index++
            )
            {
                row[
                    reader.GetName(index)
                ] =
                    await reader.IsDBNullAsync(
                        index,
                        cancellationToken
                    )
                        ? null
                        : reader.GetValue(
                            index
                        );
            }

            rows.Add(
                row
            );
        }

        return rows;
    }

    private static void AddParameters(
        DbCommand command,
        IEnumerable<
            (
                string Name,
                object? Value
            )
        > parameters
    )
    {
        foreach (
            var (
                name,
                value
            )
            in parameters
        )
        {
            var parameter =
                command.CreateParameter();

            parameter.ParameterName =
                name;

            parameter.Value =
                value
                ?? DBNull.Value;

            command.Parameters.Add(
                parameter
            );
        }
    }

    private static string GetString(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        if (
            !row.TryGetValue(
                key,
                out var value
            )
            ||
            value is null
            ||
            value == DBNull.Value
        )
        {
            return string.Empty;
        }

        return Convert.ToString(
                   value
               )?.Trim()
               ?? string.Empty;
    }

    private static string? NullableString(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        var result =
            GetString(
                row,
                key
            );

        return string.IsNullOrWhiteSpace(result)
            ? null
            : result;
    }

    private static int? GetInt(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        if (
            !row.TryGetValue(
                key,
                out var value
            )
            ||
            value is null
            ||
            value == DBNull.Value
        )
        {
            return null;
        }

        return Convert.ToInt32(
            value
        );
    }

    private static long? GetLong(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        if (
            !row.TryGetValue(
                key,
                out var value
            )
            ||
            value is null
            ||
            value == DBNull.Value
        )
        {
            return null;
        }

        return Convert.ToInt64(
            value
        );
    }

    private static DateTime? GetDateTime(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        if (
            !row.TryGetValue(
                key,
                out var value
            )
            ||
            value is null
            ||
            value == DBNull.Value
        )
        {
            return null;
        }

        return DateTime.SpecifyKind(
            Convert.ToDateTime(value),
            DateTimeKind.Unspecified
        );
    }

    // =========================================================
    // INTERNAL ACTOR
    // =========================================================

    private sealed class DashboardActor
    {
        public int UserId { get; init; }

        public string? CustomerId { get; init; }

        public int? DoctorId { get; init; }

        public string? EmployeeId { get; init; }

        public string Role { get; init; } = string.Empty;

        public string? EmployeePosition { get; init; }

        public string? EmployeeStatus { get; init; }
    }
}