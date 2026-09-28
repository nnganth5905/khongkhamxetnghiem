using System.Data;
using System.Data.Common;
using System.Globalization;

using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Repositories.Models;
using KhamXetNghiem.Api.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class SpecimenRepository
    : ISpecimenRepository
{
    private readonly AppDbContext _dbContext;

    public SpecimenRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // ACTOR
    // =====================================================

    public async Task<SpecimenActorContext?> GetActorAsync(
        string login,
        CancellationToken cancellationToken = default
    )
    {
        var rows = await QueryAsync(
            """
            SELECT
                u.UserID,
                u.IDBacSi,
                u.IDNhanVien,
                u.Role,
                n.ViTri,
                n.Status AS EmployeeStatus
            FROM users u
            LEFT JOIN nhanvien n
                ON u.IDNhanVien = n.IDNhanVien
            WHERE
                u.IsActive = 1
                AND
                (
                    u.Email = @login
                    OR u.Username = @login
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

        return new SpecimenActorContext(
            GetInt(row, "UserID")!.Value,
            GetInt(row, "IDBacSi"),
            GetString(row, "IDNhanVien"),
            GetString(row, "Role") ?? string.Empty,
            GetString(row, "ViTri"),
            GetString(row, "EmployeeStatus")
        );
    }

    // =====================================================
    // TECHNICIANS
    // =====================================================

    public async Task<List<TechnicianOptionResponse>>
        GetTechniciansAsync(
            CancellationToken cancellationToken = default
        )
    {
        var rows = await QueryAsync(
            """
            SELECT
                IDNhanVien,
                TenNhanVien
            FROM nhanvien
            WHERE
                ViTri = 'ktv'
                AND Status = 'yes'
            ORDER BY TenNhanVien
            """,
            cancellationToken
        );

        return rows.Select(
            x => new TechnicianOptionResponse
            {
                Id = GetString(
                    x,
                    "IDNhanVien"
                )!,

                Name = GetString(
                    x,
                    "TenNhanVien"
                )!
            }
        ).ToList();
    }

    // =====================================================
    // PENDING TEST ORDER ITEMS
    // =====================================================

    public async Task<List<PendingSpecimenItemResponse>>
        GetPendingItemsAsync(
            string appointmentId,
            CancellationToken cancellationToken = default
        )
    {
        var rows = await QueryAsync(
            """
            SELECT
                ct.IDCTPhieu,
                ct.IDPhieuXetNghiem,
                ct.IDXetNghiem,
                ct.TrangThai,
                xn.TenXetNghiem,
                xn.LoaiMauMacDinh,
                p.IDKhachHang,
                k.TenKhachHang
            FROM datlichxetnghiem d
            JOIN luotxetnghiem lx
                ON lx.IDDatLichXN = d.IDDatLichXN
            JOIN phieuxetnghiem p
                ON p.IDLuotXetNghiem = lx.IDLuotXetNghiem
            JOIN ctphieuxetnghiem ct
                ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem
            JOIN loaixetnghiem xn
                ON xn.IDXetNghiem = ct.IDXetNghiem
            JOIN khachhang k
                ON k.IDKhachHang = p.IDKhachHang

            LEFT JOIN maubenhpham m
                ON m.IDCTPhieu = ct.IDCTPhieu
                AND m.TrangThai NOT IN
                (
                    'tu_choi_mau',
                    'huy'
                )

            WHERE
                d.MaDatLich = @appointmentId
                AND ct.Status = 'yes'
                AND ct.TrangThai = 'cho_lay_mau'
                AND m.IDMau IS NULL

            ORDER BY ct.IDCTPhieu
            """,
            cancellationToken,
            (
                "@appointmentId",
                appointmentId
            )
        );

        return rows.Select(
            row => new PendingSpecimenItemResponse
            {
                TestOrderItemId =
                    GetLong(
                        row,
                        "IDCTPhieu"
                    )!.Value,

                TestOrderId =
                    GetString(
                        row,
                        "IDPhieuXetNghiem"
                    )!,

                TestId =
                    GetString(
                        row,
                        "IDXetNghiem"
                    )!,

                TestName =
                    GetString(
                        row,
                        "TenXetNghiem"
                    )!,

                CustomerId =
                    GetString(
                        row,
                        "IDKhachHang"
                    )!,

                PatientName =
                    GetString(
                        row,
                        "TenKhachHang"
                    )!,

                DefaultSpecimenType =
                    GetString(
                        row,
                        "LoaiMauMacDinh"
                    ),

                Status =
                    GetString(
                        row,
                        "TrangThai"
                    )!
            }
        ).ToList();
    }

    // =====================================================
    // COLLECT
    // =====================================================

    public async Task<SpecimenResponse> CollectAsync(
        CollectSpecimenRequest request,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        var now = VietnamTime.Now;

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

        var rows = await QueryAsync(
            """
            SELECT
                ct.IDCTPhieu,
                ct.IDPhieuXetNghiem,
                ct.IDXetNghiem,
                ct.TrangThai AS CTStatus,

                xn.TenXetNghiem,
                xn.LoaiMauMacDinh,

                p.IDKhachHang,
                p.IDLuotXetNghiem,

                k.TenKhachHang

            FROM datlichxetnghiem d
            JOIN luotxetnghiem lx
                ON lx.IDDatLichXN = d.IDDatLichXN
            JOIN phieuxetnghiem p
                ON p.IDLuotXetNghiem = lx.IDLuotXetNghiem
            JOIN ctphieuxetnghiem ct
                ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem
            JOIN loaixetnghiem xn
                ON xn.IDXetNghiem = ct.IDXetNghiem
            JOIN khachhang k
                ON k.IDKhachHang = p.IDKhachHang

            WHERE
                d.MaDatLich = @appointmentId
                AND ct.IDCTPhieu = @ctId
                AND ct.Status = 'yes'

            FOR UPDATE
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@appointmentId",
                request.AppointmentId
            ),
            (
                "@ctId",
                request.TestOrderItemId
            )
        );

        if (rows.Count == 0)
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy chỉ định xét nghiệm cần lấy mẫu."
            );
        }

        var row = rows[0];

        var ctStatus =
            GetString(
                row,
                "CTStatus"
            )!;

        if (ctStatus != "cho_lay_mau")
        {
            throw new BadRequestException(
                $"Chỉ định đang ở trạng thái {ctStatus}, không thể lấy mẫu."
            );
        }

        var activeSpecimen = await ScalarAsync(
            """
            SELECT IDMau
            FROM maubenhpham
            WHERE
                IDCTPhieu = @ctId
                AND TrangThai NOT IN
                (
                    'tu_choi_mau',
                    'huy'
                )
            LIMIT 1
            FOR UPDATE
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@ctId",
                request.TestOrderItemId
            )
        );

        if (activeSpecimen is not null)
        {
            throw new BadRequestException(
                "Chỉ định xét nghiệm này đã có mẫu bệnh phẩm."
            );
        }

        var specimenType =
            string.IsNullOrWhiteSpace(
                request.SpecimenType
            )
                ? GetString(
                    row,
                    "LoaiMauMacDinh"
                )
                : request.SpecimenType.Trim();

        if (string.IsNullOrWhiteSpace(specimenType))
        {
            throw new BadRequestException(
                "Vui lòng chọn loại mẫu bệnh phẩm."
            );
        }

        var barcode =
            await ResolveBarcodeAsync(
                request.Barcode,
                connection,
                dbTransaction,
                cancellationToken
            );

        var specimenId =
            GenerateSpecimenId();

        await ExecuteAsync(
            """
            INSERT INTO maubenhpham
            (
                IDMau,
                IDCTPhieu,
                MaBarcode,
                LoaiMau,
                IDBacSiLayMau,
                ThoiGianLayMau,
                TrangThai,
                GhiChu
            )
            VALUES
            (
                @id,
                @ctId,
                @barcode,
                @type,
                @doctorId,
                @now,
                'da_lay_mau',
                @note
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@id", specimenId),
            (
                "@ctId",
                request.TestOrderItemId
            ),
            ("@barcode", barcode),
            ("@type", specimenType),
            ("@doctorId", doctorId),
            ("@now", now),
            (
                "@note",
                Truncate(
                    request.Notes,
                    255
                )
            )
        );

        await ExecuteAsync(
            """
            UPDATE ctphieuxetnghiem
            SET TrangThai = 'da_lay_mau'
            WHERE IDCTPhieu = @ctId
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@ctId",
                request.TestOrderItemId
            )
        );

        var testOrderId =
            GetString(
                row,
                "IDPhieuXetNghiem"
            )!;

        var visitId =
            GetLong(
                row,
                "IDLuotXetNghiem"
            );

        await SyncCollectionStatesAsync(
            testOrderId,
            visitId,
            connection,
            dbTransaction,
            cancellationToken
        );

        var customerId =
            GetString(
                row,
                "IDKhachHang"
            )!;

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            customerId,
            "maubenhpham",
            specimenId,
            "Bác sĩ xác nhận lấy mẫu",
            "moi_tao",
            "da_lay_mau",
            actorUserId,
            $"Barcode: {barcode}; Xét nghiệm: {GetString(row, "TenXetNghiem")}",
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );

        return await GetSpecimenInternalAsync(
                   specimenId,
                   null,
                   cancellationToken
               )
               ?? throw new ResourceNotFoundException(
                   "Không đọc được mẫu vừa tạo."
               );
    }

    // =====================================================
    // HANDOVER
    // =====================================================

    public async Task<SpecimenActionResponse> HandoverAsync(
        string specimenReference,
        HandoverSpecimenRequest request,
        int doctorId,
        int actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        var now = VietnamTime.Now;

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        var connection =
            _dbContext.Database.GetDbConnection();

        var dbTransaction =
            transaction.GetDbTransaction();

        var specimen =
            await FindSpecimenForUpdateAsync(
                specimenReference,
                connection,
                dbTransaction,
                cancellationToken
            )
            ?? throw new ResourceNotFoundException(
                "Không tìm thấy mẫu bệnh phẩm."
            );

        if (specimen.Status != "da_lay_mau")
        {
            throw new BadRequestException(
                $"Mẫu đang ở trạng thái {MapStatusToApi(specimen.Status)}, không thể bàn giao."
            );
        }

        var technicianRows =
            await QueryAsync(
                """
                SELECT
                    IDNhanVien,
                    TenNhanVien
                FROM nhanvien
                WHERE
                    IDNhanVien = @id
                    AND ViTri = 'ktv'
                    AND Status = 'yes'
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@id",
                    request.ReceiverId
                )
            );

        if (technicianRows.Count == 0)
        {
            throw new BadRequestException(
                "Kỹ thuật viên không tồn tại hoặc đã ngừng hoạt động."
            );
        }

        var existingHandover =
            await ScalarAsync(
                """
                SELECT IDBanGiao
                FROM bangiaomau
                WHERE
                    IDMau = @id
                    AND TrangThai IN
                    (
                        'da_ban_giao',
                        'da_tiep_nhan'
                    )
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", specimen.Id)
            );

        if (existingHandover is not null)
        {
            throw new BadRequestException(
                "Mẫu đã được bàn giao trước đó."
            );
        }

        await ExecuteAsync(
            """
            INSERT INTO bangiaomau
            (
                IDMau,
                IDBacSiBanGiao,
                IDKTVTiepNhan,
                ThoiGianBanGiao,
                TrangThai,
                GhiChu
            )
            VALUES
            (
                @specimenId,
                @doctorId,
                @technicianId,
                @now,
                'da_ban_giao',
                @note
            )
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@specimenId",
                specimen.Id
            ),
            ("@doctorId", doctorId),
            (
                "@technicianId",
                request.ReceiverId
            ),
            ("@now", now),
            (
                "@note",
                Truncate(
                    request.Note,
                    255
                )
            )
        );

        // QUAN TRỌNG:
        // Bàn giao mới chỉ là da_ban_giao.
        // Không được set ktv_tiep_nhan ở đây.
        await ExecuteAsync(
            """
            UPDATE maubenhpham
            SET TrangThai = 'da_ban_giao'
            WHERE IDMau = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@id", specimen.Id)
        );

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            specimen.CustomerId,
            "maubenhpham",
            specimen.Id,
            "Bác sĩ bàn giao mẫu cho KTV",
            "da_lay_mau",
            "da_ban_giao",
            actorUserId,
            $"KTV nhận dự kiến: {request.ReceiverId}",
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );

        return new SpecimenActionResponse
        {
            Message =
                "Bàn giao mẫu thành công. Đang chờ kỹ thuật viên tiếp nhận.",

            Id =
                specimen.Id,

            Status =
                "HANDED_OVER"
        };
    }

    // =====================================================
    // LIST FOR TECHNICIAN
    // =====================================================

    public async Task<List<SpecimenResponse>> GetSpecimensAsync(
        string technicianId,
        string? status,
        CancellationToken cancellationToken = default
    )
    {
        var dbStatus =
            MapApiStatusToDb(
                status
            );

        var sql =
            """
            SELECT
                m.IDMau,
                m.IDCTPhieu,
                m.MaBarcode,
                m.LoaiMau,
                m.IDBacSiLayMau,
                m.ThoiGianLayMau,
                m.TrangThai,
                m.GhiChu,

                ct.IDPhieuXetNghiem,
                ct.IDXetNghiem,

                xn.TenXetNghiem,

                p.IDKhachHang,
                p.IDLuotXetNghiem,

                k.TenKhachHang,

                bs.TenBacSi AS CollectorName,

                bg.IDBanGiao,
                bg.IDBacSiBanGiao,
                bg.IDKTVTiepNhan,
                bg.ThoiGianBanGiao,
                bg.ThoiGianTiepNhan,
                bg.TrangThai AS HandoverStatus,

                bgb.TenBacSi AS HandoverDoctorName,
                nv.TenNhanVien AS TechnicianName

            FROM maubenhpham m

            JOIN ctphieuxetnghiem ct
                ON ct.IDCTPhieu = m.IDCTPhieu

            JOIN phieuxetnghiem p
                ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem

            JOIN khachhang k
                ON k.IDKhachHang = p.IDKhachHang

            JOIN loaixetnghiem xn
                ON xn.IDXetNghiem = ct.IDXetNghiem

            LEFT JOIN bacsi bs
                ON bs.IDBacSi = m.IDBacSiLayMau

            JOIN bangiaomau bg
                ON bg.IDBanGiao =
                (
                    SELECT MAX(bg2.IDBanGiao)
                    FROM bangiaomau bg2
                    WHERE bg2.IDMau = m.IDMau
                )

            LEFT JOIN bacsi bgb
                ON bgb.IDBacSi = bg.IDBacSiBanGiao

            LEFT JOIN nhanvien nv
                ON nv.IDNhanVien = bg.IDKTVTiepNhan

            WHERE
                bg.IDKTVTiepNhan = @technicianId
            """;

        var parameters =
            new List<(string Name, object? Value)>
            {
                (
                    "@technicianId",
                    technicianId
                )
            };

        if (!string.IsNullOrWhiteSpace(dbStatus))
        {
            sql +=
                " AND m.TrangThai = @status ";

            parameters.Add(
                (
                    "@status",
                    dbStatus
                )
            );
        }
        else
        {
            sql +=
                """
                AND m.TrangThai NOT IN
                (
                    'moi_tao',
                    'da_lay_mau',
                    'huy'
                )
                """;
        }

        sql +=
            " ORDER BY m.ThoiGianLayMau DESC ";

        var rows =
            await QueryAsync(
                sql,
                cancellationToken,
                parameters.ToArray()
            );

        return rows
            .Select(MapSpecimen)
            .ToList();
    }

    // =====================================================
    // DETAIL FOR TECHNICIAN
    // =====================================================

    public Task<SpecimenResponse?> GetSpecimenAsync(
        string specimenReference,
        string technicianId,
        CancellationToken cancellationToken = default
    )
    {
        return GetSpecimenInternalAsync(
            specimenReference,
            technicianId,
            cancellationToken
        );
    }

    // =====================================================
    // RECEIVE
    // =====================================================

    public async Task<SpecimenActionResponse> ReceiveAsync(
        string specimenReference,
        ReceiveSpecimenRequest request,
        string technicianId,
        int actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        var now = VietnamTime.Now;

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

        var specimen =
            await FindSpecimenForUpdateAsync(
                specimenReference,
                connection,
                dbTransaction,
                cancellationToken
            )
            ?? throw new ResourceNotFoundException(
                "Không tìm thấy mẫu bệnh phẩm."
            );

        if (specimen.Status != "da_ban_giao")
        {
            throw new BadRequestException(
                $"Mẫu đang ở trạng thái {MapStatusToApi(specimen.Status)}, không thể tiếp nhận."
            );
        }

        if (!specimen.HandoverId.HasValue)
        {
            throw new BadRequestException(
                "Mẫu chưa có thông tin bàn giao."
            );
        }

        if (
            !string.Equals(
                specimen.ReceiverId,
                technicianId,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new ForbiddenException(
                "Mẫu này được bàn giao cho kỹ thuật viên khác."
            );
        }

        if (specimen.HandoverStatus != "da_ban_giao")
        {
            throw new BadRequestException(
                "Phiếu bàn giao không còn ở trạng thái chờ tiếp nhận."
            );
        }

        var condition =
            string.IsNullOrWhiteSpace(
                request.Condition
            )
                ? "GOOD"
                : request.Condition
                    .Trim()
                    .ToUpperInvariant();

        if (
            condition is not
                "GOOD"
                and not "WARNING"
        )
        {
            throw new BadRequestException(
                "Tình trạng mẫu không hợp lệ."
            );
        }

        var note =
            BuildTechnicianNote(
                specimen.Note,
                condition,
                request.Notes
            );

        await ExecuteAsync(
            """
            UPDATE maubenhpham
            SET
                TrangThai = 'ktv_tiep_nhan',
                GhiChu = @note
            WHERE IDMau = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@note", note),
            ("@id", specimen.Id)
        );

        await ExecuteAsync(
            """
            UPDATE bangiaomau
            SET
                TrangThai = 'da_tiep_nhan',
                ThoiGianTiepNhan = @now
            WHERE IDBanGiao = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@now", now),
            (
                "@id",
                specimen.HandoverId.Value
            )
        );

        await ExecuteAsync(
            """
            UPDATE ctphieuxetnghiem
            SET TrangThai = 'ktv_tiep_nhan'
            WHERE IDCTPhieu = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@id",
                specimen.TestOrderItemId
            )
        );

        await SyncReceiveStatesAsync(
            specimen.TestOrderId,
            specimen.VisitId,
            connection,
            dbTransaction,
            cancellationToken
        );

        // ---------------------------------------------
        // WORKLIST
        // ---------------------------------------------

        var workRows =
            await QueryAsync(
                """
                SELECT
                    id,
                    Status,
                    IDKTV,
                    IDMau
                FROM worklist
                WHERE
                    IDCTPhieu = @ctId
                    AND Status <> 'cancelled'
                ORDER BY id DESC
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@ctId",
                    specimen.TestOrderItemId
                )
            );

        long worklistId;

        if (workRows.Count == 0)
        {
            await ExecuteAsync(
                """
                INSERT INTO worklist
                (
                    IDCTPhieu,
                    IDMau,
                    IDKTV,
                    Status,
                    ReceivedAt
                )
                VALUES
                (
                    @ctId,
                    @specimenId,
                    @technicianId,
                    'queue',
                    @now
                )
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@ctId",
                    specimen.TestOrderItemId
                ),
                (
                    "@specimenId",
                    specimen.Id
                ),
                (
                    "@technicianId",
                    technicianId
                ),
                ("@now", now)
            );

            worklistId =
                Convert.ToInt64(
                    await ScalarAsync(
                        "SELECT LAST_INSERT_ID();",
                        cancellationToken,
                        connection,
                        dbTransaction
                    )
                );
        }
        else
        {
            var existingStatus =
                GetString(
                    workRows[0],
                    "Status"
                )!;

            if (existingStatus != "queue")
            {
                throw new BadRequestException(
                    "Chỉ định đã được đưa vào quá trình xét nghiệm trước đó."
                );
            }

            worklistId =
                GetLong(
                    workRows[0],
                    "id"
                )!.Value;

            await ExecuteAsync(
                """
                UPDATE worklist
                SET
                    IDMau = @specimenId,
                    IDKTV = @technicianId
                WHERE id = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@specimenId",
                    specimen.Id
                ),
                (
                    "@technicianId",
                    technicianId
                ),
                ("@id", worklistId)
            );
        }

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            specimen.CustomerId,
            "maubenhpham",
            specimen.Id,
            "KTV tiếp nhận mẫu",
            "da_ban_giao",
            "ktv_tiep_nhan",
            actorUserId,
            $"KTV: {technicianId}; Tình trạng: {condition}",
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );

        return new SpecimenActionResponse
        {
            Message =
                "Tiếp nhận mẫu thành công và đã đưa vào worklist.",

            Id =
                specimen.Id,

            Status =
                "RECEIVED",

            WorklistId =
                worklistId
        };
    }

    // =====================================================
    // REJECT
    // =====================================================

    public async Task<SpecimenActionResponse> RejectAsync(
        string specimenReference,
        string reason,
        string technicianId,
        int actorUserId,
        CancellationToken cancellationToken = default
    )
    {
        var now = VietnamTime.Now;

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

        var specimen =
            await FindSpecimenForUpdateAsync(
                specimenReference,
                connection,
                dbTransaction,
                cancellationToken
            )
            ?? throw new ResourceNotFoundException(
                "Không tìm thấy mẫu bệnh phẩm."
            );

        if (specimen.Status != "da_ban_giao")
        {
            throw new BadRequestException(
                "Chỉ có thể từ chối mẫu đang chờ KTV tiếp nhận."
            );
        }

        if (
            !string.Equals(
                specimen.ReceiverId,
                technicianId,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new ForbiddenException(
                "Mẫu này được bàn giao cho kỹ thuật viên khác."
            );
        }

        if (!specimen.HandoverId.HasValue)
        {
            throw new BadRequestException(
                "Không tìm thấy phiếu bàn giao."
            );
        }

        await ExecuteAsync(
            """
            UPDATE maubenhpham
            SET TrangThai = 'tu_choi_mau'
            WHERE IDMau = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@id", specimen.Id)
        );

        await ExecuteAsync(
            """
            UPDATE bangiaomau
            SET
                TrangThai = 'tu_choi',
                LyDoTuChoi = @reason,
                ThoiGianTiepNhan = @now
            WHERE IDBanGiao = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@reason",
                Truncate(
                    reason,
                    255
                )
            ),
            ("@now", now),
            (
                "@id",
                specimen.HandoverId.Value
            )
        );

        // Cho phép lấy lại mẫu mới.
        await ExecuteAsync(
            """
            UPDATE ctphieuxetnghiem
            SET TrangThai = 'cho_lay_mau'
            WHERE IDCTPhieu = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@id",
                specimen.TestOrderItemId
            )
        );

        await ExecuteAsync(
            """
            UPDATE phieuxetnghiem
            SET TrangThai = 'cho_lay_mau'
            WHERE IDPhieuXetNghiem = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@id",
                specimen.TestOrderId
            )
        );

        if (specimen.VisitId.HasValue)
        {
            await ExecuteAsync(
                """
                UPDATE luotxetnghiem
                SET TrangThai = 'dang_lay_mau'
                WHERE IDLuotXetNghiem = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@id",
                    specimen.VisitId.Value
                )
            );
        }

        await ExecuteAsync(
            """
            UPDATE worklist
            SET Status = 'cancelled'
            WHERE
                IDCTPhieu = @ctId
                AND Status = 'queue'
            """,
            cancellationToken,
            connection,
            dbTransaction,
            (
                "@ctId",
                specimen.TestOrderItemId
            )
        );

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            specimen.CustomerId,
            "maubenhpham",
            specimen.Id,
            "KTV từ chối mẫu",
            "da_ban_giao",
            "tu_choi_mau",
            actorUserId,
            reason,
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );

        return new SpecimenActionResponse
        {
            Message =
                "Đã từ chối mẫu. Chỉ định được chuyển lại trạng thái chờ lấy mẫu.",

            Id =
                specimen.Id,

            Status =
                "REJECTED"
        };
    }

    // =====================================================
    // SYNC COLLECTION STATES
    // =====================================================

    private async Task SyncCollectionStatesAsync(
        string testOrderId,
        long? visitId,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var pending =
            Convert.ToInt32(
                await ScalarAsync(
                    """
                    SELECT COUNT(*)
                    FROM ctphieuxetnghiem
                    WHERE
                        IDPhieuXetNghiem = @id
                        AND Status = 'yes'
                        AND TrangThai = 'cho_lay_mau'
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@id", testOrderId)
                )
                ?? 0
            );

        var orderStatus =
            pending == 0
                ? "da_lay_mau"
                : "cho_lay_mau";

        await ExecuteAsync(
            """
            UPDATE phieuxetnghiem
            SET TrangThai = @status
            WHERE IDPhieuXetNghiem = @id
            """,
            cancellationToken,
            connection,
            transaction,
            ("@status", orderStatus),
            ("@id", testOrderId)
        );

        if (visitId.HasValue)
        {
            await ExecuteAsync(
                """
                UPDATE luotxetnghiem
                SET TrangThai = @status
                WHERE IDLuotXetNghiem = @id
                """,
                cancellationToken,
                connection,
                transaction,
                (
                    "@status",
                    pending == 0
                        ? "da_lay_mau"
                        : "dang_lay_mau"
                ),
                ("@id", visitId.Value)
            );
        }
    }

    // =====================================================
    // SYNC RECEIVE STATES
    // =====================================================

    private async Task SyncReceiveStatesAsync(
        string testOrderId,
        long? visitId,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var rows = await QueryAsync(
            """
            SELECT
                SUM(
                    CASE
                        WHEN TrangThai = 'cho_lay_mau'
                        THEN 1 ELSE 0
                    END
                ) AS WaitingCollect,

                SUM(
                    CASE
                        WHEN TrangThai = 'da_lay_mau'
                        THEN 1 ELSE 0
                    END
                ) AS WaitingReceive

            FROM ctphieuxetnghiem

            WHERE
                IDPhieuXetNghiem = @id
                AND Status = 'yes'
            """,
            cancellationToken,
            connection,
            transaction,
            ("@id", testOrderId)
        );

        var waitingCollect =
            GetInt(
                rows[0],
                "WaitingCollect"
            ) ?? 0;

        var waitingReceive =
            GetInt(
                rows[0],
                "WaitingReceive"
            ) ?? 0;

        string orderStatus;
        string visitStatus;

        if (waitingCollect > 0)
        {
            orderStatus =
                "cho_lay_mau";

            visitStatus =
                "dang_lay_mau";
        }
        else if (waitingReceive > 0)
        {
            orderStatus =
                "da_lay_mau";

            visitStatus =
                "da_lay_mau";
        }
        else
        {
            orderStatus =
                "ktv_tiep_nhan";

            visitStatus =
                "ktv_tiep_nhan";
        }

        await ExecuteAsync(
            """
            UPDATE phieuxetnghiem
            SET TrangThai = @status
            WHERE IDPhieuXetNghiem = @id
            """,
            cancellationToken,
            connection,
            transaction,
            ("@status", orderStatus),
            ("@id", testOrderId)
        );

        if (visitId.HasValue)
        {
            await ExecuteAsync(
                """
                UPDATE luotxetnghiem
                SET TrangThai = @status
                WHERE IDLuotXetNghiem = @id
                """,
                cancellationToken,
                connection,
                transaction,
                ("@status", visitStatus),
                ("@id", visitId.Value)
            );
        }
    }

    // =====================================================
    // GET SPECIMEN INTERNAL
    // =====================================================

    private async Task<SpecimenResponse?>
        GetSpecimenInternalAsync(
            string reference,
            string? technicianId,
            CancellationToken cancellationToken
        )
    {
        var sql =
            """
            SELECT
                m.IDMau,
                m.IDCTPhieu,
                m.MaBarcode,
                m.LoaiMau,
                m.IDBacSiLayMau,
                m.ThoiGianLayMau,
                m.TrangThai,
                m.GhiChu,

                ct.IDPhieuXetNghiem,
                ct.IDXetNghiem,

                xn.TenXetNghiem,

                p.IDKhachHang,
                p.IDLuotXetNghiem,

                k.TenKhachHang,

                bs.TenBacSi AS CollectorName,

                bg.IDBanGiao,
                bg.IDBacSiBanGiao,
                bg.IDKTVTiepNhan,
                bg.ThoiGianBanGiao,
                bg.ThoiGianTiepNhan,
                bg.TrangThai AS HandoverStatus,

                bgb.TenBacSi AS HandoverDoctorName,
                nv.TenNhanVien AS TechnicianName

            FROM maubenhpham m

            JOIN ctphieuxetnghiem ct
                ON ct.IDCTPhieu = m.IDCTPhieu

            JOIN phieuxetnghiem p
                ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem

            JOIN khachhang k
                ON k.IDKhachHang = p.IDKhachHang

            JOIN loaixetnghiem xn
                ON xn.IDXetNghiem = ct.IDXetNghiem

            LEFT JOIN bacsi bs
                ON bs.IDBacSi = m.IDBacSiLayMau

            LEFT JOIN bangiaomau bg
                ON bg.IDBanGiao =
                (
                    SELECT MAX(bg2.IDBanGiao)
                    FROM bangiaomau bg2
                    WHERE bg2.IDMau = m.IDMau
                )

            LEFT JOIN bacsi bgb
                ON bgb.IDBacSi = bg.IDBacSiBanGiao

            LEFT JOIN nhanvien nv
                ON nv.IDNhanVien = bg.IDKTVTiepNhan

            WHERE
                (
                    m.IDMau = @reference
                    OR m.MaBarcode = @reference
                )
            """;

        var parameters =
            new List<(string Name, object? Value)>
            {
                (
                    "@reference",
                    reference
                )
            };

        if (!string.IsNullOrWhiteSpace(technicianId))
        {
            sql +=
                " AND bg.IDKTVTiepNhan = @technicianId ";

            parameters.Add(
                (
                    "@technicianId",
                    technicianId
                )
            );
        }

        sql += " LIMIT 1 ";

        var rows =
            await QueryAsync(
                sql,
                cancellationToken,
                parameters.ToArray()
            );

        return rows.Count == 0
            ? null
            : MapSpecimen(
                rows[0]
            );
    }

    // =====================================================
    // LOCK SPECIMEN
    // =====================================================

    private async Task<SpecimenDbRow?>
        FindSpecimenForUpdateAsync(
            string reference,
            DbConnection connection,
            DbTransaction transaction,
            CancellationToken cancellationToken
        )
    {
        var rows =
            await QueryAsync(
                """
                SELECT
                    m.IDMau,
                    m.IDCTPhieu,
                    m.MaBarcode,
                    m.LoaiMau,
                    m.IDBacSiLayMau,
                    m.ThoiGianLayMau,
                    m.TrangThai,
                    m.GhiChu,

                    ct.IDPhieuXetNghiem,
                    ct.IDXetNghiem,

                    xn.TenXetNghiem,

                    p.IDKhachHang,
                    p.IDLuotXetNghiem,

                    k.TenKhachHang,

                    bs.TenBacSi AS CollectorName,

                    bg.IDBanGiao,
                    bg.IDBacSiBanGiao,
                    bg.IDKTVTiepNhan,
                    bg.ThoiGianBanGiao,
                    bg.ThoiGianTiepNhan,
                    bg.TrangThai AS HandoverStatus,

                    bgb.TenBacSi AS HandoverDoctorName,
                    nv.TenNhanVien AS TechnicianName

                FROM maubenhpham m

                JOIN ctphieuxetnghiem ct
                    ON ct.IDCTPhieu = m.IDCTPhieu

                JOIN phieuxetnghiem p
                    ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem

                JOIN khachhang k
                    ON k.IDKhachHang = p.IDKhachHang

                JOIN loaixetnghiem xn
                    ON xn.IDXetNghiem = ct.IDXetNghiem

                LEFT JOIN bacsi bs
                    ON bs.IDBacSi = m.IDBacSiLayMau

                LEFT JOIN bangiaomau bg
                    ON bg.IDBanGiao =
                    (
                        SELECT MAX(bg2.IDBanGiao)
                        FROM bangiaomau bg2
                        WHERE bg2.IDMau = m.IDMau
                    )

                LEFT JOIN bacsi bgb
                    ON bgb.IDBacSi = bg.IDBacSiBanGiao

                LEFT JOIN nhanvien nv
                    ON nv.IDNhanVien = bg.IDKTVTiepNhan

                WHERE
                    (
                        m.IDMau = @reference
                        OR m.MaBarcode = @reference
                    )

                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                transaction,
                ("@reference", reference)
            );

        if (rows.Count == 0)
        {
            return null;
        }

        var row = rows[0];

        return new SpecimenDbRow(
            GetString(row, "IDMau")!,
            GetLong(row, "IDCTPhieu")!.Value,
            GetString(row, "IDPhieuXetNghiem")!,
            GetString(row, "IDXetNghiem")!,
            GetString(row, "TenXetNghiem")!,
            GetString(row, "IDKhachHang")!,
            GetString(row, "TenKhachHang")!,
            GetLong(row, "IDLuotXetNghiem"),
            GetString(row, "LoaiMau")!,
            GetString(row, "MaBarcode")!,
            GetDateTime(row, "ThoiGianLayMau"),
            GetInt(row, "IDBacSiLayMau"),
            GetString(row, "CollectorName"),
            GetString(row, "TrangThai")!,
            GetString(row, "GhiChu"),
            GetLong(row, "IDBanGiao"),
            GetInt(row, "IDBacSiBanGiao"),
            GetString(row, "HandoverDoctorName"),
            GetString(row, "IDKTVTiepNhan"),
            GetString(row, "TechnicianName"),
            GetDateTime(row, "ThoiGianBanGiao"),
            GetDateTime(row, "ThoiGianTiepNhan"),
            GetString(row, "HandoverStatus")
        );
    }

    // =====================================================
    // BARCODE
    // =====================================================

    private async Task<string> ResolveBarcodeAsync(
        string? requestedBarcode,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        if (!string.IsNullOrWhiteSpace(requestedBarcode))
        {
            var value =
                requestedBarcode.Trim();

            if (value.Length > 64)
            {
                throw new BadRequestException(
                    "Barcode tối đa 64 ký tự."
                );
            }

            var exists =
                await ScalarAsync(
                    """
                    SELECT IDMau
                    FROM maubenhpham
                    WHERE MaBarcode = @barcode
                    LIMIT 1
                    FOR UPDATE
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@barcode", value)
                );

            if (exists is not null)
            {
                throw new BadRequestException(
                    "Barcode đã tồn tại."
                );
            }

            return value;
        }

        for (var index = 0; index < 5; index++)
        {
            var value =
                "BC"
                + VietnamTime.Now
                    .ToString(
                        "yyyyMMddHHmmssfff"
                    )
                + Random.Shared.Next(
                    1000,
                    9999
                );

            var exists =
                await ScalarAsync(
                    """
                    SELECT IDMau
                    FROM maubenhpham
                    WHERE MaBarcode = @barcode
                    LIMIT 1
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@barcode", value)
                );

            if (exists is null)
            {
                return value;
            }
        }

        throw new BadRequestException(
            "Không thể sinh barcode duy nhất."
        );
    }

    private static string GenerateSpecimenId()
    {
        return
            "M"
            + VietnamTime.Now.ToString(
                "yyyyMMddHHmmssfff"
            )
            + Random.Shared.Next(
                100,
                999
            );
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
        int actorUserId,
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
            ("@oldStatus", oldStatus),
            ("@newStatus", newStatus),
            ("@userId", actorUserId),
            ("@time", VietnamTime.Now),
            (
                "@description",
                Truncate(
                    description,
                    1000
                )
            )
        );
    }

    // =====================================================
    // MAPPING
    // =====================================================

    private static SpecimenResponse MapSpecimen(
        IReadOnlyDictionary<string, object?> row
    )
    {
        return new SpecimenResponse
        {
            Id =
                GetString(
                    row,
                    "IDMau"
                )!,

            TestOrderItemId =
                GetLong(
                    row,
                    "IDCTPhieu"
                )!.Value,

            TestOrderId =
                GetString(
                    row,
                    "IDPhieuXetNghiem"
                )!,

            TestId =
                GetString(
                    row,
                    "IDXetNghiem"
                )!,

            TestName =
                GetString(
                    row,
                    "TenXetNghiem"
                )!,

            CustomerId =
                GetString(
                    row,
                    "IDKhachHang"
                )!,

            PatientName =
                GetString(
                    row,
                    "TenKhachHang"
                )!,

            SpecimenType =
                GetString(
                    row,
                    "LoaiMau"
                )!,

            Barcode =
                GetString(
                    row,
                    "MaBarcode"
                )!,

            CollectedAt =
                GetDateTime(
                    row,
                    "ThoiGianLayMau"
                ),

            CollectorDoctorId =
                GetInt(
                    row,
                    "IDBacSiLayMau"
                ),

            CollectorName =
                GetString(
                    row,
                    "CollectorName"
                ),

            HandedOverAt =
                GetDateTime(
                    row,
                    "ThoiGianBanGiao"
                ),

            HandoverBy =
                GetString(
                    row,
                    "HandoverDoctorName"
                ),

            ReceivedAt =
                GetDateTime(
                    row,
                    "ThoiGianTiepNhan"
                ),

            TechnicianId =
                GetString(
                    row,
                    "IDKTVTiepNhan"
                ),

            TechnicianName =
                GetString(
                    row,
                    "TechnicianName"
                ),

            Status =
                MapStatusToApi(
                    GetString(
                        row,
                        "TrangThai"
                    )
                ),

            Note =
                GetString(
                    row,
                    "GhiChu"
                )
        };
    }

    private static string MapStatusToApi(
        string? status
    )
    {
        return status switch
        {
            "moi_tao" =>
                "CREATED",

            "da_lay_mau" =>
                "COLLECTED",

            "da_ban_giao" =>
                "HANDED_OVER",

            "ktv_tiep_nhan" =>
                "RECEIVED",

            "tu_choi_mau" =>
                "REJECTED",

            "dang_xu_ly" =>
                "IN_PROGRESS",

            "hoan_tat" =>
                "COMPLETED",

            "huy" =>
                "CANCELLED",

            null =>
                "UNKNOWN",

            _ =>
                status.ToUpperInvariant()
        };
    }

    private static string? MapApiStatusToDb(
        string? status
    )
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return null;
        }

        return status
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "CREATED" =>
                "moi_tao",

            "COLLECTED" =>
                "da_lay_mau",

            "HANDED_OVER" =>
                "da_ban_giao",

            "RECEIVED" =>
                "ktv_tiep_nhan",

            "REJECTED" =>
                "tu_choi_mau",

            "PROCESSING" =>
                "dang_xu_ly",

            "IN_PROGRESS" =>
                "dang_xu_ly",

            "COMPLETED" =>
                "hoan_tat",

            "CANCELLED" =>
                "huy",

            _ =>
                throw new BadRequestException(
                    "Trạng thái mẫu không hợp lệ."
                )
        };
    }

    private static string BuildTechnicianNote(
        string? oldNote,
        string condition,
        string? newNote
    )
    {
        var parts =
            new List<string>();

        if (!string.IsNullOrWhiteSpace(oldNote))
        {
            parts.Add(
                oldNote.Trim()
            );
        }

        parts.Add(
            condition == "WARNING"
                ? "[KTV] Mẫu có lưu ý nhưng vẫn đủ điều kiện xử lý."
                : "[KTV] Mẫu đạt yêu cầu."
        );

        if (!string.IsNullOrWhiteSpace(newNote))
        {
            parts.Add(
                newNote.Trim()
            );
        }

        return Truncate(
                   string.Join(
                       " | ",
                       parts
                   ),
                   255
               )
               ?? string.Empty;
    }

    private static string? Truncate(
        string? value,
        int length
    )
    {
        if (string.IsNullOrWhiteSpace(value))
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
    // RAW DB
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

    private async Task<int> ExecuteAsync(
        string sql,
        CancellationToken cancellationToken,
        DbConnection connection,
        DbTransaction transaction,
        params (string Name, object? Value)[] parameters
    )
    {
        await using var command =
            connection.CreateCommand();

        command.CommandText = sql;
        command.Transaction = transaction;

        AddParameters(
            command,
            parameters
        );

        return await command
            .ExecuteNonQueryAsync(
                cancellationToken
            );
    }

    private async Task<object?> ScalarAsync(
        string sql,
        CancellationToken cancellationToken,
        DbConnection connection,
        DbTransaction transaction,
        params (string Name, object? Value)[] parameters
    )
    {
        await using var command =
            connection.CreateCommand();

        command.CommandText = sql;
        command.Transaction = transaction;

        AddParameters(
            command,
            parameters
        );

        return await command
            .ExecuteScalarAsync(
                cancellationToken
            );
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
        return row.TryGetValue(
                   key,
                   out var value
               )
               && value is not null
            ? Convert.ToInt32(value)
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
               && value is not null
            ? Convert.ToInt64(value)
            : null;
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
               && value is not null
            ? DateTime.SpecifyKind(
                Convert.ToDateTime(value),
                DateTimeKind.Unspecified
            )
            : null;
    }
}