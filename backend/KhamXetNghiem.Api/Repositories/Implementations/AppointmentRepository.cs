using System.Data;
using System.Data.Common;
using System.Globalization;

using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Repositories.Models;
using KhamXetNghiem.Api.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class AppointmentRepository
    : IAppointmentRepository
{
    private readonly AppDbContext _dbContext;

    public AppointmentRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // OPTIONS
    // =====================================================

    public async Task<AppointmentOptionsResponse> GetOptionsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var specialties =
            await QueryAsync(
                """
                SELECT IDChuyenKhoa, TenChuyenKhoa
                FROM chuyenkhoa
                WHERE Status = 'yes'
                ORDER BY TenChuyenKhoa
                """,
                cancellationToken
            );

        var doctors =
            await QueryAsync(
                """
                SELECT
                    IDBacSi,
                    TenBacSi,
                    KhoaID,
                    CoSoID
                FROM bacsi
                WHERE TrangThai = 'active'
                ORDER BY TenBacSi
                """,
                cancellationToken
            );

        var tests =
            await QueryAsync(
                """
                SELECT
                    IDXetNghiem,
                    TenXetNghiem,
                    ChuyenKhoaID,
                    Gia,
                    ThoiGianDuKienPhut
                FROM loaixetnghiem
                WHERE Status = 'yes'
                ORDER BY TenXetNghiem
                """,
                cancellationToken
            );

        var facilities =
            await QueryAsync(
                """
                SELECT CoSoID, TenCoSo
                FROM coso
                WHERE TrangThai = 'active'
                ORDER BY TenCoSo
                """,
                cancellationToken
            );

        return new AppointmentOptionsResponse
        {
            Departments =
                specialties.ToDictionary(
                    x => GetString(x, "IDChuyenKhoa")!,
                    x => GetString(x, "TenChuyenKhoa")!
                ),

            Doctors =
                doctors.Select(
                    x => new AppointmentDoctorOption
                    {
                        Id = GetInt(x, "IDBacSi")!.Value,
                        Name = GetString(x, "TenBacSi")!,
                        SpecialtyId = GetString(x, "KhoaID")!,
                        FacilityId = GetString(x, "CoSoID")
                    }
                ).ToList(),

            Tests =
                tests.Select(
                    x => new AppointmentTestOption
                    {
                        Id = GetString(x, "IDXetNghiem")!,
                        Name = GetString(x, "TenXetNghiem")!,
                        SpecialtyId = GetString(x, "ChuyenKhoaID")!,
                        Price = GetDecimal(x, "Gia") ?? 0,
                        EstimatedMinutes =
                            GetInt(x, "ThoiGianDuKienPhut")
                    }
                ).ToList(),

            Facilities =
                facilities.Select(
                    x => new AppointmentFacilityOption
                    {
                        Id = GetString(x, "CoSoID")!,
                        Name = GetString(x, "TenCoSo")!
                    }
                ).ToList()
        };
    }

    // =====================================================
    // ACCOUNT / CUSTOMER
    // =====================================================

    public async Task<AppointmentAccountContext?> FindAccountAsync(
        string login,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT UserID, IDKhachHang, Email
                FROM users
                WHERE
                    IsActive = 1
                    AND (Email = @login OR Username = @login)
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

        return new AppointmentAccountContext(
            GetInt(row, "UserID")!.Value,
            GetString(row, "IDKhachHang"),
            GetString(row, "Email") ?? string.Empty
        );
    }

    public async Task<AppointmentCustomerData?> FindCustomerByIdAsync(
        string customerId,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDKhachHang,
                    TenKhachHang,
                    SoDienThoai,
                    Email,
                    NgaySinh,
                    GioiTinh
                FROM khachhang
                WHERE IDKhachHang = @id
                LIMIT 1
                """,
                cancellationToken,
                ("@id", customerId)
            );

        return rows.Count == 0
            ? null
            : MapCustomer(rows[0]);
    }

    public async Task<AppointmentCustomerData?> FindCustomerAsync(
        string fullName,
        string phone,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDKhachHang,
                    TenKhachHang,
                    SoDienThoai,
                    Email,
                    NgaySinh,
                    GioiTinh
                FROM khachhang
                WHERE
                    Status = 'yes'
                    AND SoDienThoai = @phone
                    AND LOWER(TRIM(TenKhachHang))
                        = LOWER(TRIM(@name))
                LIMIT 1
                """,
                cancellationToken,
                ("@phone", phone),
                ("@name", fullName)
            );

        return rows.Count == 0
            ? null
            : MapCustomer(rows[0]);
    }

    public async Task<AppointmentCustomerData> CreateCustomerAsync(
        string id,
        string fullName,
        string phone,
        string? email,
        DateTime? birthDate,
        string gender,
        CancellationToken cancellationToken = default
    )
    {
        await ExecuteAsync(
            """
            INSERT INTO khachhang
            (
                IDKhachHang,
                TenKhachHang,
                NgaySinh,
                SoDienThoai,
                GioiTinh,
                Email,
                Status
            )
            VALUES
            (
                @id,
                @name,
                @dob,
                @phone,
                @gender,
                @email,
                'yes'
            )
            """,
            cancellationToken,
            ("@id", id),
            ("@name", fullName),
            ("@dob", birthDate?.Date),
            ("@phone", phone),
            ("@gender", gender),
            ("@email", email)
        );

        return new AppointmentCustomerData(
            id,
            fullName,
            phone,
            email,
            birthDate,
            gender
        );
    }

    // =====================================================
    // DOCTOR / TEST / FACILITY
    // =====================================================

    public async Task<AppointmentDoctorData?> FindDoctorAsync(
        int doctorId,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDBacSi,
                    TenBacSi,
                    KhoaID,
                    CoSoID,
                    TrangThai
                FROM bacsi
                WHERE IDBacSi = @id
                LIMIT 1
                """,
                cancellationToken,
                ("@id", doctorId)
            );

        if (rows.Count == 0)
        {
            return null;
        }

        var row = rows[0];

        return new AppointmentDoctorData(
            GetInt(row, "IDBacSi")!.Value,
            GetString(row, "TenBacSi")!,
            GetString(row, "KhoaID")!,
            GetString(row, "CoSoID"),
            GetString(row, "TrangThai")!
        );
    }

    public async Task<AppointmentTestData?> FindTestAsync(
        string testId,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    IDXetNghiem,
                    TenXetNghiem,
                    ChuyenKhoaID,
                    Gia,
                    ThoiGianDuKienPhut,
                    Status
                FROM loaixetnghiem
                WHERE IDXetNghiem = @id
                LIMIT 1
                """,
                cancellationToken,
                ("@id", testId)
            );

        if (rows.Count == 0)
        {
            return null;
        }

        var row = rows[0];

        if (
            !string.Equals(
                GetString(row, "Status"),
                "yes",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return null;
        }

        return new AppointmentTestData(
            GetString(row, "IDXetNghiem")!,
            GetString(row, "TenXetNghiem")!,
            GetString(row, "ChuyenKhoaID")!,
            GetDecimal(row, "Gia") ?? 0,
            GetInt(row, "ThoiGianDuKienPhut")
        );
    }

    public async Task<bool> IsFacilityActiveAsync(
        string facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var value =
            await ScalarAsync(
                """
                SELECT COUNT(*)
                FROM coso
                WHERE
                    CoSoID = @id
                    AND TrangThai = 'active'
                """,
                cancellationToken,
                ("@id", facilityId)
            );

        return Convert.ToInt64(value ?? 0) > 0;
    }

    // =====================================================
    // WORK SCHEDULE + SLOTS
    // =====================================================

    public async Task<List<AppointmentWorkWindow>>
        GetDoctorWorkWindowsAsync(
            int doctorId,
            DateTime date,
            CancellationToken cancellationToken = default
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT GioBatDau, GioKetThuc
                FROM lichlamviec
                WHERE
                    IDBacSi = @doctorId
                    AND Ngay = @date
                    AND TrangThai = 'duoc_duyet'
                ORDER BY GioBatDau
                """,
                cancellationToken,
                ("@doctorId", doctorId),
                ("@date", date.Date)
            );

        return rows.Select(
            row => new AppointmentWorkWindow(
                GetTime(row, "GioBatDau"),
                GetTime(row, "GioKetThuc")
            )
        ).ToList();
    }

    public async Task<List<string>> GetDoctorOccupiedTimesAsync(
        int doctorId,
        DateTime date,
        string? excludeAppointmentCode = null,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT AppointmentTime
                FROM
                (
                    SELECT
                        GioKham AS AppointmentTime,
                        MaDatLich
                    FROM datlichkham
                    WHERE
                        IDBacSi = @doctorId
                        AND NgayKham = @date
                        AND TrangThai NOT IN ('cancelled', 'no_show')

                    UNION ALL

                    SELECT
                        GioXetNghiem AS AppointmentTime,
                        MaDatLich
                    FROM datlichxetnghiem
                    WHERE
                        IDBacSi = @doctorId
                        AND NgayXetNghiem = @date
                        AND TrangThai NOT IN ('cancelled', 'no_show')
                ) x
                WHERE
                    @excludeCode IS NULL
                    OR x.MaDatLich <> @excludeCode
                """,
                cancellationToken,
                ("@doctorId", doctorId),
                ("@date", date.Date),
                ("@excludeCode", excludeAppointmentCode)
            );

        return rows
            .Select(x => FormatTime(GetTime(x, "AppointmentTime")))
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public async Task<List<string>> GetTakenTimesAsync(
        string type,
        int doctorId,
        DateTime date,
        CancellationToken cancellationToken = default
    )
    {
        var exam =
            string.Equals(
                type,
                "EXAMINATION",
                StringComparison.OrdinalIgnoreCase
            );

        var sql =
            exam
                ? """
                  SELECT GioKham AS T
                  FROM datlichkham
                  WHERE
                      IDBacSi = @doctorId
                      AND NgayKham = @date
                      AND TrangThai NOT IN ('cancelled', 'no_show')
                  """
                : """
                  SELECT GioXetNghiem AS T
                  FROM datlichxetnghiem
                  WHERE
                      IDBacSi = @doctorId
                      AND NgayXetNghiem = @date
                      AND TrangThai NOT IN ('cancelled', 'no_show')
                  """;

        var rows =
            await QueryAsync(
                sql,
                cancellationToken,
                ("@doctorId", doctorId),
                ("@date", date.Date)
            );

        return rows
            .Select(x => FormatTime(GetTime(x, "T")))
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    // =====================================================
    // CREATE EXAMINATION
    // =====================================================

    public async Task<AppointmentCreateDbResult>
        CreateExaminationAsync(
            NewExaminationBooking booking,
            CancellationToken cancellationToken = default
        )
    {
        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken
                );

        var connection =
            _dbContext.Database.GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        await EnsureSlotAvailableAsync(
            connection,
            dbTransaction,
            booking.CustomerId,
            booking.DoctorId,
            booking.Date,
            booking.Time,
            null,
            cancellationToken
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
                @userId,
                @customerId,
                @specialtyId,
                @doctorId,
                @facilityId,
                @date,
                @time,
                @note,
                'pending',
                'pending',
                @qr
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@code", booking.Code),
            ("@userId", booking.UserId),
            ("@customerId", booking.CustomerId),
            ("@specialtyId", booking.SpecialtyId),
            ("@doctorId", booking.DoctorId),
            ("@facilityId", booking.FacilityId),
            ("@date", booking.Date.Date),
            ("@time", booking.Time),
            ("@note", booking.Note),
            ("@qr", booking.QrCode)
        );

        var id =
            Convert.ToInt64(
                await ScalarAsync(
                    "SELECT LAST_INSERT_ID();",
                    cancellationToken,
                    connection,
                    dbTransaction
                )
            );

        await transaction.CommitAsync(
            cancellationToken
        );

        return new AppointmentCreateDbResult(
            id,
            booking.Code,
            booking.QrCode
        );
    }

    // =====================================================
    // CREATE TEST
    // =====================================================

    public async Task<AppointmentCreateDbResult> CreateTestAsync(
        NewTestBooking booking,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken
                );

        var connection =
            _dbContext.Database.GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        await EnsureSlotAvailableAsync(
            connection,
            dbTransaction,
            booking.CustomerId,
            booking.DoctorId,
            booking.Date,
            booking.Time,
            null,
            cancellationToken
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
                @userId,
                @customerId,
                @doctorId,
                @facilityId,
                @date,
                @time,
                @note,
                'pending',
                'pending',
                @qr
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@code", booking.Code),
            ("@userId", booking.UserId),
            ("@customerId", booking.CustomerId),
            ("@doctorId", booking.DoctorId),
            ("@facilityId", booking.FacilityId),
            ("@date", booking.Date.Date),
            ("@time", booking.Time),
            ("@note", booking.Note),
            ("@qr", booking.QrCode)
        );

        var id =
            Convert.ToInt64(
                await ScalarAsync(
                    "SELECT LAST_INSERT_ID();",
                    cancellationToken,
                    connection,
                    dbTransaction
                )
            );

        foreach (var test in booking.Tests)
        {
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
                ("@appointmentId", id),
                ("@testId", test.Id),
                ("@price", test.Price),
                (
                    "@note",
                    Truncate(
                        booking.Note,
                        255
                    )
                )
            );
        }

        await transaction.CommitAsync(
            cancellationToken
        );

        return new AppointmentCreateDbResult(
            id,
            booking.Code,
            booking.QrCode
        );
    }

    // =====================================================
    // MY APPOINTMENTS
    // =====================================================

    public async Task<List<AppointmentResponse>>
        GetMyAppointmentsAsync(
            int userId,
            string? customerId,
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
                        d.MaDatLich AS Id,
                        'EXAMINATION' AS Type,
                        d.IDKhachHang AS CustomerId,
                        k.TenKhachHang AS CustomerName,
                        k.SoDienThoai AS CustomerPhone,
                        CAST(d.IDBacSi AS CHAR) AS DoctorId,
                        b.TenBacSi AS DoctorName,
                        d.NgayKham AS AppointmentDate,
                        d.GioKham AS AppointmentTime,
                        COALESCE(lk.TrangThai, d.TrangThai) AS RealStatus,
                        d.MaQR AS QrCode,
                        d.GhiChu AS Note
                    FROM datlichkham d
                    JOIN khachhang k
                        ON d.IDKhachHang = k.IDKhachHang
                    LEFT JOIN bacsi b
                        ON d.IDBacSi = b.IDBacSi
                    LEFT JOIN luotkham lk
                        ON d.IDDatLichKham = lk.IDDatLichKham
                    WHERE
                        d.UserID = @userId
                        OR (
                            @customerId IS NOT NULL
                            AND d.IDKhachHang = @customerId
                        )

                    UNION ALL

                    SELECT
                        d.MaDatLich AS Id,
                        'TEST' AS Type,
                        d.IDKhachHang AS CustomerId,
                        k.TenKhachHang AS CustomerName,
                        k.SoDienThoai AS CustomerPhone,
                        CAST(d.IDBacSi AS CHAR) AS DoctorId,
                        b.TenBacSi AS DoctorName,
                        d.NgayXetNghiem AS AppointmentDate,
                        d.GioXetNghiem AS AppointmentTime,
                        COALESCE(lxn.TrangThai, d.TrangThai) AS RealStatus,
                        d.MaQR AS QrCode,
                        d.GhiChu AS Note
                    FROM datlichxetnghiem d
                    JOIN khachhang k
                        ON d.IDKhachHang = k.IDKhachHang
                    LEFT JOIN bacsi b
                        ON d.IDBacSi = b.IDBacSi
                    LEFT JOIN luotxetnghiem lxn
                        ON d.IDDatLichXN = lxn.IDDatLichXN
                    WHERE
                        d.UserID = @userId
                        OR (
                            @customerId IS NOT NULL
                            AND d.IDKhachHang = @customerId
                        )
                ) a
                ORDER BY
                    AppointmentDate DESC,
                    AppointmentTime DESC
                """,
                cancellationToken,
                ("@userId", userId),
                ("@customerId", customerId)
            );

        return rows
            .Select(MapAppointmentResponse)
            .ToList();
    }

    // =====================================================
    // ADMIN / RECEPTION LIST
    // =====================================================

    public async Task<List<AppointmentResponse>>
        GetAllAppointmentsAsync(
            DateTime date,
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
                        d.MaDatLich AS Id,
                        'EXAMINATION' AS Type,
                        d.IDKhachHang AS CustomerId,
                        k.TenKhachHang AS CustomerName,
                        k.SoDienThoai AS CustomerPhone,
                        CAST(d.IDBacSi AS CHAR) AS DoctorId,
                        b.TenBacSi AS DoctorName,
                        d.NgayKham AS AppointmentDate,
                        d.GioKham AS AppointmentTime,
                        d.TrangThai AS RealStatus,
                        d.MaQR AS QrCode,
                        d.GhiChu AS Note
                    FROM datlichkham d
                    JOIN khachhang k
                        ON d.IDKhachHang = k.IDKhachHang
                    LEFT JOIN bacsi b
                        ON d.IDBacSi = b.IDBacSi
                    WHERE d.NgayKham = @date

                    UNION ALL

                    SELECT
                        d.MaDatLich AS Id,
                        'TEST' AS Type,
                        d.IDKhachHang AS CustomerId,
                        k.TenKhachHang AS CustomerName,
                        k.SoDienThoai AS CustomerPhone,
                        CAST(d.IDBacSi AS CHAR) AS DoctorId,
                        b.TenBacSi AS DoctorName,
                        d.NgayXetNghiem AS AppointmentDate,
                        d.GioXetNghiem AS AppointmentTime,
                        d.TrangThai AS RealStatus,
                        d.MaQR AS QrCode,
                        d.GhiChu AS Note
                    FROM datlichxetnghiem d
                    JOIN khachhang k
                        ON d.IDKhachHang = k.IDKhachHang
                    LEFT JOIN bacsi b
                        ON d.IDBacSi = b.IDBacSi
                    WHERE d.NgayXetNghiem = @date
                ) a
                ORDER BY AppointmentTime
                """,
                cancellationToken,
                ("@date", date.Date)
            );

        return rows
            .Select(MapAppointmentResponse)
            .ToList();
    }

    // =====================================================
    // EDIT INFO
    // =====================================================

    public async Task<AppointmentEditData?> GetEditDataAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var exam =
            await QueryAsync(
                """
                SELECT
                    IDDatLichKham AS DbId,
                    MaDatLich,
                    UserID,
                    IDKhachHang,
                    IDBacSi,
                    IDChuyenKhoa,
                    CoSoID,
                    NgayKham AS D,
                    GioKham AS T,
                    TrangThai,
                    GhiChu
                FROM datlichkham
                WHERE MaDatLich = @code
                LIMIT 1
                """,
                cancellationToken,
                ("@code", code)
            );

        if (exam.Count > 0)
        {
            var row = exam[0];

            return new AppointmentEditData(
                GetLong(row, "DbId")!.Value,
                GetString(row, "MaDatLich")!,
                "EXAMINATION",
                GetInt(row, "UserID"),
                GetString(row, "IDKhachHang")!,
                GetInt(row, "IDBacSi"),
                GetString(row, "IDChuyenKhoa"),
                GetString(row, "CoSoID")!,
                GetDate(row, "D"),
                GetTime(row, "T"),
                GetString(row, "TrangThai")!,
                GetString(row, "GhiChu")
            );
        }

        var test =
            await QueryAsync(
                """
                SELECT
                    IDDatLichXN AS DbId,
                    MaDatLich,
                    UserID,
                    IDKhachHang,
                    IDBacSi,
                    CoSoID,
                    NgayXetNghiem AS D,
                    GioXetNghiem AS T,
                    TrangThai,
                    GhiChu
                FROM datlichxetnghiem
                WHERE MaDatLich = @code
                LIMIT 1
                """,
                cancellationToken,
                ("@code", code)
            );

        if (test.Count == 0)
        {
            return null;
        }

        var t = test[0];

        return new AppointmentEditData(
            GetLong(t, "DbId")!.Value,
            GetString(t, "MaDatLich")!,
            "TEST",
            GetInt(t, "UserID"),
            GetString(t, "IDKhachHang")!,
            GetInt(t, "IDBacSi"),
            null,
            GetString(t, "CoSoID")!,
            GetDate(t, "D"),
            GetTime(t, "T"),
            GetString(t, "TrangThai")!,
            GetString(t, "GhiChu")
        );
    }

    public async Task<List<string>> GetTestIdsAsync(
        string appointmentCode,
        CancellationToken cancellationToken = default
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT ct.IDXetNghiem
                FROM ctdatlichxetnghiem ct
                JOIN datlichxetnghiem d
                    ON ct.IDDatLichXN = d.IDDatLichXN
                WHERE d.MaDatLich = @code
                """,
                cancellationToken,
                ("@code", appointmentCode)
            );

        return rows
            .Select(x => GetString(x, "IDXetNghiem")!)
            .ToList();
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<AppointmentDetailResponse?> GetDetailAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var edit =
            await GetEditDataAsync(
                code,
                cancellationToken
            );

        if (edit is null)
        {
            return null;
        }

        return edit.Type == "EXAMINATION"
            ? await GetExaminationDetailAsync(
                edit,
                cancellationToken
            )
            : await GetTestDetailAsync(
                edit,
                cancellationToken
            );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task UpdateAppointmentAsync(
        AppointmentEditData current,
        int doctorId,
        DateTime newDate,
        TimeSpan newTime,
        string? note,
        int? actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken
                );

        var connection =
            _dbContext.Database.GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        await EnsureSlotAvailableAsync(
            connection,
            dbTransaction,
            current.CustomerId,
            doctorId,
            newDate,
            newTime,
            current.Code,
            cancellationToken
        );

        if (current.Type == "EXAMINATION")
        {
            await ExecuteAsync(
                """
                UPDATE datlichkham
                SET
                    IDBacSi = @doctorId,
                    NgayKham = @date,
                    GioKham = @time,
                    GhiChu = @note,
                    TrangThai = 'pending',
                    StatusMail = 'pending'
                WHERE MaDatLich = @code
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@doctorId", doctorId),
                ("@date", newDate.Date),
                ("@time", newTime),
                ("@note", note),
                ("@code", current.Code)
            );
        }
        else
        {
            await ExecuteAsync(
                """
                UPDATE datlichxetnghiem
                SET
                    IDBacSi = @doctorId,
                    NgayXetNghiem = @date,
                    GioXetNghiem = @time,
                    GhiChu = @note,
                    TrangThai = 'pending',
                    StatusMail = 'pending'
                WHERE MaDatLich = @code
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@doctorId", doctorId),
                ("@date", newDate.Date),
                ("@time", newTime),
                ("@note", note),
                ("@code", current.Code)
            );
        }

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            current.CustomerId,
            current.Type == "EXAMINATION"
                ? "datlichkham"
                : "datlichxetnghiem",
            current.DbId.ToString(),
            "Khách hàng thay đổi lịch hẹn",
            actorUserId,
            $"Dời lịch sang {FormatTime(newTime)} ngày {newDate:yyyy-MM-dd}",
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );
    }

    // =====================================================
    // CANCEL
    // =====================================================

    public async Task CancelAppointmentAsync(
        AppointmentEditData current,
        int? actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        var connection =
            _dbContext.Database.GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        var table =
            current.Type == "EXAMINATION"
                ? "datlichkham"
                : "datlichxetnghiem";

        await ExecuteAsync(
            $"""
            UPDATE {table}
            SET TrangThai = 'cancelled'
            WHERE MaDatLich = @code
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@code", current.Code)
        );

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            current.CustomerId,
            current.Type == "EXAMINATION"
                ? "datlichkham"
                : "datlichxetnghiem",
            current.DbId.ToString(),
            "Khách hàng hủy lịch hẹn",
            actorUserId,
            null,
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );
    }

    // =====================================================
    // AUTO NO-SHOW
    // =====================================================

    public async Task<int> MarkExpiredAppointmentsNoShowAsync(
        DateTime today,
        CancellationToken cancellationToken = default
    )
    {
        var exam =
            await ExecuteAsync(
                """
                UPDATE datlichkham
                SET TrangThai = 'no_show'
                WHERE
                    NgayKham < @today
                    AND TrangThai IN ('pending', 'confirmed')
                """,
                cancellationToken,
                ("@today", today.Date)
            );

        var test =
            await ExecuteAsync(
                """
                UPDATE datlichxetnghiem
                SET TrangThai = 'no_show'
                WHERE
                    NgayXetNghiem < @today
                    AND TrangThai IN ('pending', 'confirmed')
                """,
                cancellationToken,
                ("@today", today.Date)
            );

        return exam + test;
    }

    // =====================================================
    // ATOMIC SLOT VALIDATION
    // =====================================================

    private async Task EnsureSlotAvailableAsync(
        DbConnection connection,
        DbTransaction transaction,
        string customerId,
        int doctorId,
        DateTime date,
        TimeSpan time,
        string? excludeCode,
        CancellationToken cancellationToken
    )
    {
        var workSchedule =
            await QueryAsync(
                """
                SELECT LichID
                FROM lichlamviec
                WHERE
                    IDBacSi = @doctorId
                    AND Ngay = @date
                    AND TrangThai = 'duoc_duyet'
                    AND @time >= GioBatDau
                    AND @time < GioKetThuc
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                transaction,
                ("@doctorId", doctorId),
                ("@date", date.Date),
                ("@time", time)
            );

        if (workSchedule.Count == 0)
        {
            throw new BadRequestException(
                "Bác sĩ không có lịch làm việc tại khung giờ đã chọn."
            );
        }

        var doctorConflict =
            await QueryAsync(
                """
                SELECT Code
                FROM
                (
                    SELECT MaDatLich AS Code
                    FROM datlichkham
                    WHERE
                        IDBacSi = @doctorId
                        AND NgayKham = @date
                        AND GioKham = @time
                        AND TrangThai NOT IN ('cancelled', 'no_show')

                    UNION ALL

                    SELECT MaDatLich AS Code
                    FROM datlichxetnghiem
                    WHERE
                        IDBacSi = @doctorId
                        AND NgayXetNghiem = @date
                        AND GioXetNghiem = @time
                        AND TrangThai NOT IN ('cancelled', 'no_show')
                ) x
                WHERE
                    @excludeCode IS NULL
                    OR x.Code <> @excludeCode
                LIMIT 1
                """,
                cancellationToken,
                connection,
                transaction,
                ("@doctorId", doctorId),
                ("@date", date.Date),
                ("@time", time),
                ("@excludeCode", excludeCode)
            );

        if (doctorConflict.Count > 0)
        {
            throw new BadRequestException(
                "Khung giờ này bác sĩ đã có lịch hẹn."
            );
        }

        var customerConflict =
            await QueryAsync(
                """
                SELECT Code
                FROM
                (
                    SELECT MaDatLich AS Code
                    FROM datlichkham
                    WHERE
                        IDKhachHang = @customerId
                        AND NgayKham = @date
                        AND GioKham = @time
                        AND TrangThai NOT IN ('cancelled', 'no_show')

                    UNION ALL

                    SELECT MaDatLich AS Code
                    FROM datlichxetnghiem
                    WHERE
                        IDKhachHang = @customerId
                        AND NgayXetNghiem = @date
                        AND GioXetNghiem = @time
                        AND TrangThai NOT IN ('cancelled', 'no_show')
                ) x
                WHERE
                    @excludeCode IS NULL
                    OR x.Code <> @excludeCode
                LIMIT 1
                """,
                cancellationToken,
                connection,
                transaction,
                ("@customerId", customerId),
                ("@date", date.Date),
                ("@time", time),
                ("@excludeCode", excludeCode)
            );

        if (customerConflict.Count > 0)
        {
            throw new BadRequestException(
                "Khách hàng đã có lịch hẹn ở khung giờ này."
            );
        }
    }

    // =====================================================
    // DETAIL - EXAMINATION
    // =====================================================

    private async Task<AppointmentDetailResponse>
        GetExaminationDetailAsync(
            AppointmentEditData edit,
            CancellationToken cancellationToken
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    d.MaDatLich,
                    d.IDKhachHang,
                    d.IDChuyenKhoa,
                    d.IDBacSi,
                    d.CoSoID,
                    d.NgayKham,
                    d.GioKham,
                    d.GhiChu,
                    d.MaQR,
                    d.CreatedAt,
                    k.TenKhachHang,
                    k.SoDienThoai,
                    b.TenBacSi,
                    COALESCE(lk.TrangThai, d.TrangThai) AS RealStatus,
                    lk.IDLuotKham,
                    lk.ThoiGianTiepNhan,
                    lk.ThoiGianBatDau,
                    lk.ThoiGianKetThuc
                FROM datlichkham d
                JOIN khachhang k
                    ON d.IDKhachHang = k.IDKhachHang
                LEFT JOIN bacsi b
                    ON d.IDBacSi = b.IDBacSi
                LEFT JOIN luotkham lk
                    ON d.IDDatLichKham = lk.IDDatLichKham
                WHERE d.MaDatLich = @code
                LIMIT 1
                """,
                cancellationToken,
                ("@code", edit.Code)
            );

        var row = rows[0];

        var timeline =
            await ReadTimelineAsync(
                "datlichkham",
                edit.DbId.ToString(),
                cancellationToken
            );

        AddTimeline(
            timeline,
            row,
            "CreatedAt",
            "Đặt lịch khám thành công"
        );

        AddTimeline(
            timeline,
            row,
            "ThoiGianTiepNhan",
            "Check-in tại quầy"
        );

        AddTimeline(
            timeline,
            row,
            "ThoiGianBatDau",
            "Bác sĩ bắt đầu khám"
        );

        AddTimeline(
            timeline,
            row,
            "ThoiGianKetThuc",
            "Hoàn tất khám"
        );

        var testOrders =
            await ReadExaminationTestOrdersAsync(
                edit.DbId,
                cancellationToken
            );

        return new AppointmentDetailResponse
        {
            Id = edit.Code,
            Type = "EXAMINATION",
            CustomerId = edit.CustomerId,
            CustomerName = GetString(row, "TenKhachHang"),
            CustomerPhone = GetString(row, "SoDienThoai"),
            DoctorId = edit.DoctorId?.ToString(),
            DoctorName = GetString(row, "TenBacSi"),
            SpecialtyId = edit.SpecialtyId,
            FacilityId = edit.FacilityId,
            Date = $"{edit.Date:yyyy-MM-dd}",
            Time = FormatTime(edit.Time),
            Status = GetString(row, "RealStatus") ?? edit.Status,
            QrCode = GetString(row, "MaQR"),
            Note = GetString(row, "GhiChu"),
            Timeline = SortTimeline(timeline),
            TestOrders = testOrders
        };
    }

    // =====================================================
    // DETAIL - TEST
    // =====================================================

    private async Task<AppointmentDetailResponse> GetTestDetailAsync(
        AppointmentEditData edit,
        CancellationToken cancellationToken
    )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    d.MaDatLich,
                    d.IDKhachHang,
                    d.IDBacSi,
                    d.CoSoID,
                    d.NgayXetNghiem,
                    d.GioXetNghiem,
                    d.GhiChu,
                    d.MaQR,
                    d.CreatedAt,
                    k.TenKhachHang,
                    k.SoDienThoai,
                    b.TenBacSi,
                    COALESCE(lxn.TrangThai, d.TrangThai) AS RealStatus,
                    lxn.ThoiGianTiepNhan,
                    lxn.ThoiGianBatDau,
                    lxn.ThoiGianKetThuc
                FROM datlichxetnghiem d
                JOIN khachhang k
                    ON d.IDKhachHang = k.IDKhachHang
                LEFT JOIN bacsi b
                    ON d.IDBacSi = b.IDBacSi
                LEFT JOIN luotxetnghiem lxn
                    ON d.IDDatLichXN = lxn.IDDatLichXN
                WHERE d.MaDatLich = @code
                LIMIT 1
                """,
                cancellationToken,
                ("@code", edit.Code)
            );

        var row = rows[0];

        var timeline =
            await ReadTimelineAsync(
                "datlichxetnghiem",
                edit.DbId.ToString(),
                cancellationToken
            );

        AddTimeline(
            timeline,
            row,
            "CreatedAt",
            "Đặt lịch xét nghiệm thành công"
        );

        AddTimeline(
            timeline,
            row,
            "ThoiGianTiepNhan",
            "Check-in tại quầy"
        );

        AddTimeline(
            timeline,
            row,
            "ThoiGianBatDau",
            "Bắt đầu quy trình xét nghiệm"
        );

        AddTimeline(
            timeline,
            row,
            "ThoiGianKetThuc",
            "Hoàn tất quy trình"
        );

        var registeredTests =
            await QueryAsync(
                """
                SELECT l.TenXetNghiem
                FROM ctdatlichxetnghiem ct
                JOIN loaixetnghiem l
                    ON ct.IDXetNghiem = l.IDXetNghiem
                WHERE ct.IDDatLichXN = @id
                ORDER BY l.TenXetNghiem
                """,
                cancellationToken,
                ("@id", edit.DbId)
            );

        return new AppointmentDetailResponse
        {
            Id = edit.Code,
            Type = "TEST",
            CustomerId = edit.CustomerId,
            CustomerName = GetString(row, "TenKhachHang"),
            CustomerPhone = GetString(row, "SoDienThoai"),
            DoctorId = edit.DoctorId?.ToString(),
            DoctorName =
                GetString(row, "TenBacSi")
                ?? "Được sắp xếp khi đến nơi",
            FacilityId = edit.FacilityId,
            Date = $"{edit.Date:yyyy-MM-dd}",
            Time = FormatTime(edit.Time),
            Status = GetString(row, "RealStatus") ?? edit.Status,
            QrCode = GetString(row, "MaQR"),
            Note = GetString(row, "GhiChu"),
            Timeline = SortTimeline(timeline),
            RegisteredTests =
                registeredTests
                    .Select(
                        x => GetString(
                            x,
                            "TenXetNghiem"
                        )!
                    )
                    .ToList()
        };
    }

    // =====================================================
    // TIMELINE
    // =====================================================

    private async Task<List<AppointmentTimelineItem>>
        ReadTimelineAsync(
            string objectType,
            string objectId,
            CancellationToken cancellationToken
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    ThoiGian,
                    COALESCE(NULLIF(MoTa, ''), HanhDong) AS Event
                FROM truyvet
                WHERE
                    LoaiDoiTuong = @type
                    AND IDDoiTuong = @id
                ORDER BY ThoiGian
                """,
                cancellationToken,
                ("@type", objectType),
                ("@id", objectId)
            );

        return rows.Select(
            x => new AppointmentTimelineItem
            {
                Time =
                    FormatDateTime(
                        GetDateTime(
                            x,
                            "ThoiGian"
                        )
                    ),

                Event =
                    GetString(
                        x,
                        "Event"
                    )
                    ?? "Cập nhật lịch hẹn"
            }
        ).ToList();
    }

    private async Task<List<AppointmentTestOrderSummary>>
        ReadExaminationTestOrdersAsync(
            long appointmentId,
            CancellationToken cancellationToken
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    p.IDPhieuXetNghiem,
                    p.TrangThai,
                    p.NgayTao,
                    l.TenXetNghiem
                FROM luotkham lk
                JOIN kham k
                    ON lk.IDLuotKham = k.IDLuotKham
                JOIN phieuxetnghiem p
                    ON p.IDKham = k.IDKham
                LEFT JOIN ctphieuxetnghiem ct
                    ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem
                LEFT JOIN loaixetnghiem l
                    ON ct.IDXetNghiem = l.IDXetNghiem
                WHERE lk.IDDatLichKham = @id
                ORDER BY p.NgayTao
                """,
                cancellationToken,
                ("@id", appointmentId)
            );

        return rows
            .GroupBy(
                x => GetString(
                    x,
                    "IDPhieuXetNghiem"
                )!
            )
            .Select(
                group => new AppointmentTestOrderSummary
                {
                    IDPhieuXetNghiem = group.Key,

                    TrangThai =
                        GetString(
                            group.First(),
                            "TrangThai"
                        ),

                    NgayTao =
                        FormatDateTime(
                            GetDateTime(
                                group.First(),
                                "NgayTao"
                            )
                        ),

                    Tests =
                        group
                            .Select(
                                x =>
                                    GetString(
                                        x,
                                        "TenXetNghiem"
                                    )
                            )
                            .Where(
                                x =>
                                    !string.IsNullOrWhiteSpace(
                                        x
                                    )
                            )
                            .Cast<string>()
                            .Distinct()
                            .ToList()
                }
            )
            .ToList();
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
        int? userId,
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
                @userId,
                'user',
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
            ("@userId", userId),
            ("@time", VietnamTime.Now),
            ("@description", description)
        );
    }

    // =====================================================
    // COMMON DB HELPERS
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
            ?? _dbContext.Database.GetDbConnection();

        var closeAfter =
            existingConnection is null
            &&
            connection.State != ConnectionState.Open;

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

            command.CommandText = sql;
            command.Transaction = transaction;

            AddParameters(
                command,
                parameters
            );

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken
                );

            var result =
                new List<Dictionary<string, object?>>();

            while (
                await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                var row =
                    new Dictionary<string, object?>(
                        StringComparer.OrdinalIgnoreCase
                    );

                for (
                    var i = 0;
                    i < reader.FieldCount;
                    i++
                )
                {
                    row[reader.GetName(i)] =
                        reader.IsDBNull(i)
                            ? null
                            : reader.GetValue(i);
                }

                result.Add(row);
            }

            return result;
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
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
            ?? _dbContext.Database.GetDbConnection();

        var closeAfter =
            existingConnection is null
            &&
            connection.State != ConnectionState.Open;

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

            command.CommandText = sql;
            command.Transaction = transaction;

            AddParameters(
                command,
                parameters
            );

            return await command.ExecuteScalarAsync(
                cancellationToken
            );
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
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
            ?? _dbContext.Database.GetDbConnection();

        var closeAfter =
            existingConnection is null
            &&
            connection.State != ConnectionState.Open;

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

            command.CommandText = sql;
            command.Transaction = transaction;

            AddParameters(
                command,
                parameters
            );

            return await command.ExecuteNonQueryAsync(
                cancellationToken
            );
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static void AddParameters(
        DbCommand command,
        IEnumerable<(string Name, object? Value)> parameters
    )
    {
        foreach (var item in parameters)
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
    // MAPPERS
    // =====================================================

    private static AppointmentCustomerData MapCustomer(
        IReadOnlyDictionary<string, object?> row
    )
    {
        return new AppointmentCustomerData(
            GetString(row, "IDKhachHang")!,
            GetString(row, "TenKhachHang")!,
            GetString(row, "SoDienThoai"),
            GetString(row, "Email"),
            GetNullableDate(row, "NgaySinh"),
            GetString(row, "GioiTinh")
        );
    }

    private static AppointmentResponse MapAppointmentResponse(
        IReadOnlyDictionary<string, object?> row
    )
    {
        return new AppointmentResponse
        {
            Id = GetString(row, "Id")!,
            Type = GetString(row, "Type")!,
            CustomerId = GetString(row, "CustomerId")!,
            CustomerName = GetString(row, "CustomerName"),
            CustomerPhone = GetString(row, "CustomerPhone"),
            DoctorId = GetString(row, "DoctorId"),
            DoctorName =
                GetString(row, "DoctorName")
                ?? (
                    GetString(row, "Type") == "TEST"
                        ? "Được sắp xếp khi đến nơi"
                        : null
                ),

            AppointmentDate =
                $"{GetDate(row, "AppointmentDate"):yyyy-MM-dd}",

            AppointmentTime =
                FormatTime(
                    GetTime(
                        row,
                        "AppointmentTime"
                    )
                ),

            Status =
                GetString(
                    row,
                    "RealStatus"
                )
                ?? string.Empty,

            QrCode =
                GetString(
                    row,
                    "QrCode"
                ),

            Note =
                GetString(
                    row,
                    "Note"
                )
        };
    }

    private static string? GetString(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(
            key,
            out var value
        )
        && value is not null
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
        return row.TryGetValue(key, out var value)
               && value is not null
            ? Convert.ToInt32(value)
            : null;
    }

    private static long? GetLong(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(key, out var value)
               && value is not null
            ? Convert.ToInt64(value)
            : null;
    }

    private static decimal? GetDecimal(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(key, out var value)
               && value is not null
            ? Convert.ToDecimal(value)
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

    private static DateTime? GetNullableDate(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(key, out var value)
               && value is not null
            ? Convert.ToDateTime(value).Date
            : null;
    }

    private static DateTime? GetDateTime(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        return row.TryGetValue(key, out var value)
               && value is not null
            ? Convert.ToDateTime(value)
            : null;
    }

    private static TimeSpan GetTime(
        IReadOnlyDictionary<string, object?> row,
        string key
    )
    {
        var value = row[key]!;

        return value is TimeSpan time
            ? time
            : TimeSpan.Parse(
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

    private static string FormatDateTime(
        DateTime? value
    )
    {
        return value.HasValue
            ? value.Value.ToString(
                "yyyy-MM-dd HH:mm:ss"
            )
            : string.Empty;
    }

    private static void AddTimeline(
        ICollection<AppointmentTimelineItem> timeline,
        IReadOnlyDictionary<string, object?> row,
        string column,
        string eventName
    )
    {
        var time =
            GetDateTime(
                row,
                column
            );

        if (!time.HasValue)
        {
            return;
        }

        timeline.Add(
            new AppointmentTimelineItem
            {
                Time =
                    FormatDateTime(
                        time
                    ),

                Event =
                    eventName
            }
        );
    }

    private static List<AppointmentTimelineItem> SortTimeline(
        IEnumerable<AppointmentTimelineItem> timeline
    )
    {
        return timeline
            .Where(
                x =>
                    !string.IsNullOrWhiteSpace(
                        x.Time
                    )
            )
            .GroupBy(
                x =>
                    $"{x.Time}|{x.Event}"
            )
            .Select(x => x.First())
            .OrderBy(x => x.Time)
            .ToList();
    }

    private static string? Truncate(
        string? value,
        int maxLength
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

        var trimmed =
            value.Trim();

        return trimmed.Length <= maxLength
            ? trimmed
            : trimmed[..maxLength];
    }
}