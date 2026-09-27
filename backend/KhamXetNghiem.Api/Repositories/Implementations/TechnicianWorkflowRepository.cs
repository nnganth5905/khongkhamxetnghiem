using System.Data;
using System.Data.Common;
using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class TechnicianWorkflowRepository
    : ITechnicianWorkflowRepository
{
    private readonly AppDbContext _dbContext;

    public TechnicianWorkflowRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // CURRENT TECHNICIAN
    // =====================================================

    public async Task<TechnicianIdentity?>
        FindCurrentTechnicianAsync(
            string login,
            CancellationToken cancellationToken = default
        )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
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

            command.CommandText =
                """
                SELECT
                    u.UserID,
                    nv.IDNhanVien,
                    nv.TenNhanVien,
                    nv.Email,
                    nv.SoDienThoai,
                    nv.CoSoID

                FROM users u

                INNER JOIN nhanvien nv
                    ON u.IDNhanVien =
                       nv.IDNhanVien

                WHERE
                    (
                        u.Email = @login
                        OR u.Username = @login
                    )
                    AND u.Role = 'ktv'
                    AND u.IsActive = 1
                    AND nv.ViTri = 'ktv'
                    AND nv.Status = 'yes'

                LIMIT 1
                """;

            AddParameter(
                command,
                "@login",
                login
            );

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken
                );

            if (
                !await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                return null;
            }

            return new TechnicianIdentity(
                Convert.ToInt32(
                    reader["UserID"]
                ),
                Convert.ToString(
                    reader["IDNhanVien"]
                )!,
                Convert.ToString(
                    reader["TenNhanVien"]
                )!,
                ReadNullableString(
                    reader,
                    "Email"
                ),
                ReadNullableString(
                    reader,
                    "SoDienThoai"
                ),
                ReadNullableString(
                    reader,
                    "CoSoID"
                )
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

    // =====================================================
    // SPECIMEN LIST
    // =====================================================

    public async Task<List<SpecimenListItemResponse>>
        GetSpecimensAsync(
            string? status,
            CancellationToken cancellationToken = default
        )
    {
        var dbStatus =
            MapApiSpecimenStatusToDb(
                status
            );

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
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

            command.CommandText =
                """
                SELECT
                    m.IDMau,
                    m.MaBarcode,
                    k.TenKhachHang,
                    m.LoaiMau,
                    m.ThoiGianLayMau,
                    m.TrangThai

                FROM maubenhpham m

                INNER JOIN ctphieuxetnghiem ct
                    ON m.IDCTPhieu =
                       ct.IDCTPhieu

                INNER JOIN phieuxetnghiem p
                    ON ct.IDPhieuXetNghiem =
                       p.IDPhieuXetNghiem

                INNER JOIN khachhang k
                    ON p.IDKhachHang =
                       k.IDKhachHang

                WHERE
                    (
                        @status IS NOT NULL
                        AND m.TrangThai = @status
                    )
                    OR
                    (
                        @status IS NULL
                        AND m.TrangThai NOT IN
                        (
                            'moi_tao',
                            'da_lay_mau',
                            'huy'
                        )
                    )

                ORDER BY
                    m.ThoiGianLayMau DESC,
                    m.IDMau DESC
                """;

            AddParameter(
                command,
                "@status",
                dbStatus
            );

            var result =
                new List<SpecimenListItemResponse>();

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken
                );

            while (
                await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                var id =
                    Convert.ToString(
                        reader["IDMau"]
                    )!;

                result.Add(
                    new SpecimenListItemResponse
                    {
                        Id = id,

                        MaVach =
                            Convert.ToString(
                                reader["MaBarcode"]
                            )
                            ?? string.Empty,

                        TenKhachHang =
                            ReadNullableString(
                                reader,
                                "TenKhachHang"
                            ),

                        LoaiMau =
                            Convert.ToString(
                                reader["LoaiMau"]
                            )
                            ?? string.Empty,

                        ThoiGianLay =
                            ReadNullableDateTime(
                                reader,
                                "ThoiGianLayMau"
                            ),

                        TrangThai =
                            MapDbSpecimenStatusToApi(
                                Convert.ToString(
                                    reader["TrangThai"]
                                )
                            )
                    }
                );
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

    // =====================================================
    // SPECIMEN DETAIL
    // =====================================================

    public async Task<SpecimenDetailResponse?>
        GetSpecimenAsync(
            string specimenId,
            CancellationToken cancellationToken = default
        )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
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

            command.CommandText =
                """
                SELECT
                    m.IDMau,
                    m.MaBarcode,
                    k.TenKhachHang,
                    m.LoaiMau,
                    m.ThoiGianLayMau,
                    m.TrangThai,
                    m.GhiChu,

                    bs.TenBacSi AS HandoverBy

                FROM maubenhpham m

                INNER JOIN ctphieuxetnghiem ct
                    ON m.IDCTPhieu =
                       ct.IDCTPhieu

                INNER JOIN phieuxetnghiem p
                    ON ct.IDPhieuXetNghiem =
                       p.IDPhieuXetNghiem

                INNER JOIN khachhang k
                    ON p.IDKhachHang =
                       k.IDKhachHang

                LEFT JOIN bangiaomau bg
                    ON bg.IDBanGiao =
                    (
                        SELECT MAX(bg2.IDBanGiao)
                        FROM bangiaomau bg2
                        WHERE bg2.IDMau =
                              m.IDMau
                    )

                LEFT JOIN bacsi bs
                    ON bg.IDBacSiBanGiao =
                       bs.IDBacSi

                WHERE
                    m.IDMau =
                        @specimenId

                LIMIT 1
                """;

            AddParameter(
                command,
                "@specimenId",
                specimenId
            );

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken
                );

            if (
                !await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                return null;
            }

            return new SpecimenDetailResponse
            {
                Code =
                    Convert.ToString(
                        reader["IDMau"]
                    )!,

                Barcode =
                    Convert.ToString(
                        reader["MaBarcode"]
                    )
                    ?? string.Empty,

                PatientName =
                    ReadNullableString(
                        reader,
                        "TenKhachHang"
                    ),

                SpecimenType =
                    Convert.ToString(
                        reader["LoaiMau"]
                    )
                    ?? string.Empty,

                CollectedAt =
                    ReadNullableDateTime(
                        reader,
                        "ThoiGianLayMau"
                    ),

                Status =
                    MapDbSpecimenStatusToApi(
                        Convert.ToString(
                            reader["TrangThai"]
                        )
                    ),

                Notes =
                    ReadNullableString(
                        reader,
                        "GhiChu"
                    ),

                HandoverBy =
                    ReadNullableString(
                        reader,
                        "HandoverBy"
                    )
            };
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // RECEIVE SPECIMEN
    // =====================================================

    public async Task ReceiveSpecimenAsync(
        string specimenId,
        ReceiveSpecimenRequest request,
        TechnicianIdentity technician,
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

        try
        {
            var context =
                await GetSpecimenContextForUpdateAsync(
                    connection,
                    dbTransaction,
                    specimenId,
                    cancellationToken
                );

            if (context is null)
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy mẫu bệnh phẩm: {specimenId}"
                );
            }

            if (
                context.HandoverId is null
            )
            {
                throw new BadRequestException(
                    "Mẫu chưa có phiếu bàn giao."
                );
            }

            if (
                !string.Equals(
                    context.HandoverStatus,
                    "da_ban_giao",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    "Mẫu này không còn ở trạng thái chờ tiếp nhận."
                );
            }

            if (
                !string.IsNullOrWhiteSpace(
                    context.AssignedTechnicianId
                )
                &&
                !string.Equals(
                    context.AssignedTechnicianId,
                    technician.EmployeeId,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new ForbiddenException(
                    "Mẫu này được bàn giao cho kỹ thuật viên khác."
                );
            }

            var now =
                VietnamTime.Now;

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE bangiaomau

                SET
                    IDKTVTiepNhan =
                        @employeeId,

                    ThoiGianTiepNhan =
                        @now,

                    TrangThai =
                        'da_tiep_nhan',

                    LyDoTuChoi =
                        NULL,

                    GhiChu =
                        @notes

                WHERE
                    IDBanGiao =
                        @handoverId
                """,

                cancellationToken,

                ("@employeeId", technician.EmployeeId),
                ("@now", now),
                ("@notes", NormalizeNullable(request.Notes)),
                ("@handoverId", context.HandoverId.Value)
            );

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE maubenhpham

                SET TrangThai =
                    'ktv_tiep_nhan'

                WHERE IDMau =
                    @specimenId
                """,

                cancellationToken,

                ("@specimenId", specimenId)
            );

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE ctphieuxetnghiem

                SET TrangThai =
                    'ktv_tiep_nhan'

                WHERE IDCTPhieu =
                    @detailId
                """,

                cancellationToken,

                ("@detailId", context.DetailId)
            );

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE phieuxetnghiem

                SET TrangThai =
                    'ktv_tiep_nhan'

                WHERE IDPhieuXetNghiem =
                    @orderId
                """,

                cancellationToken,

                ("@orderId", context.TestOrderId)
            );

            if (
                context.VisitId.HasValue
            )
            {
                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE luotxetnghiem

                    SET TrangThai =
                        'ktv_tiep_nhan'

                    WHERE IDLuotXetNghiem =
                        @visitId
                    """,

                    cancellationToken,

                    ("@visitId", context.VisitId.Value)
                );
            }

            /*
             * Tạo worklist thật.
             * Không tạo trùng nếu mẫu đã có worklist.
             */
            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                INSERT INTO worklist
                (
                    IDCTPhieu,
                    IDMau,
                    IDKTV,
                    Status,
                    ReceivedAt
                )

                SELECT
                    @detailId,
                    @specimenId,
                    @employeeId,
                    'queue',
                    @now

                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM worklist
                    WHERE IDMau =
                        @specimenId
                )
                """,

                cancellationToken,

                ("@detailId", context.DetailId),
                ("@specimenId", specimenId),
                ("@employeeId", technician.EmployeeId),
                ("@now", now)
            );

            await InsertTrackingAsync(
                connection,
                dbTransaction,

                context.CustomerId,
                "maubenhpham",
                specimenId,
                "KTV tiếp nhận mẫu",
                context.SpecimenStatus,
                "ktv_tiep_nhan",
                technician.UserId,
                now,
                $"Kỹ thuật viên {technician.FullName} tiếp nhận mẫu.",

                cancellationToken
            );

            await transaction.CommitAsync(
                cancellationToken
            );
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken
            );

            throw;
        }
    }

    // =====================================================
    // REJECT SPECIMEN
    // =====================================================

    public async Task RejectSpecimenAsync(
        string specimenId,
        RejectSpecimenRequest request,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                request.Reason
            )
        )
        {
            throw new BadRequestException(
                "Vui lòng nhập lý do từ chối mẫu."
            );
        }

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

        try
        {
            var context =
                await GetSpecimenContextForUpdateAsync(
                    connection,
                    dbTransaction,
                    specimenId,
                    cancellationToken
                );

            if (context is null)
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy mẫu bệnh phẩm: {specimenId}"
                );
            }

            if (
                context.HandoverId is null
            )
            {
                throw new BadRequestException(
                    "Mẫu chưa có phiếu bàn giao."
                );
            }

            if (
                !string.Equals(
                    context.HandoverStatus,
                    "da_ban_giao",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    "Mẫu này không còn ở trạng thái có thể từ chối."
                );
            }

            var now =
                VietnamTime.Now;

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE bangiaomau

                SET
                    IDKTVTiepNhan =
                        @employeeId,

                    ThoiGianTiepNhan =
                        @now,

                    TrangThai =
                        'tu_choi',

                    LyDoTuChoi =
                        @reason

                WHERE IDBanGiao =
                    @handoverId
                """,

                cancellationToken,

                ("@employeeId", technician.EmployeeId),
                ("@now", now),
                ("@reason", request.Reason.Trim()),
                ("@handoverId", context.HandoverId.Value)
            );

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE maubenhpham

                SET TrangThai =
                    'tu_choi_mau'

                WHERE IDMau =
                    @specimenId
                """,

                cancellationToken,

                ("@specimenId", specimenId)
            );

            await InsertTrackingAsync(
                connection,
                dbTransaction,

                context.CustomerId,
                "maubenhpham",
                specimenId,
                "KTV từ chối mẫu",
                context.SpecimenStatus,
                "tu_choi_mau",
                technician.UserId,
                now,
                request.Reason.Trim(),

                cancellationToken
            );

            await transaction.CommitAsync(
                cancellationToken
            );
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken
            );

            throw;
        }
    }

    // =====================================================
    // WORKLIST
    // =====================================================

    public async Task<List<WorklistItemResponse>>
        GetWorklistAsync(
            string? status,
            TechnicianIdentity technician,
            CancellationToken cancellationToken = default
        )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
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
                """
                SELECT
                    w.id,
                    m.MaBarcode,
                    xn.TenXetNghiem,
                    k.TenKhachHang,
                    w.ReceivedAt,
                    w.Status AS WorkStatus,

                    kq.IDKetQua,
                    kq.TrangThai AS ResultStatus

                FROM worklist w

                INNER JOIN ctphieuxetnghiem ct
                    ON w.IDCTPhieu =
                       ct.IDCTPhieu

                INNER JOIN phieuxetnghiem p
                    ON ct.IDPhieuXetNghiem =
                       p.IDPhieuXetNghiem

                INNER JOIN khachhang k
                    ON p.IDKhachHang =
                       k.IDKhachHang

                INNER JOIN loaixetnghiem xn
                    ON ct.IDXetNghiem =
                       xn.IDXetNghiem

                LEFT JOIN maubenhpham m
                    ON w.IDMau =
                       m.IDMau

                LEFT JOIN ketquaxetnghiem kq
                    ON kq.IDCTPhieu =
                       ct.IDCTPhieu

                WHERE
                    (
                        w.IDKTV =
                            @employeeId
                        OR
                        w.IDKTV IS NULL
                    )

                ORDER BY
                    w.ReceivedAt DESC,
                    w.id DESC
                """;

            AddParameter(
                command,
                "@employeeId",
                technician.EmployeeId
            );

            var result =
                new List<WorklistItemResponse>();

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken
                );

            while (
                await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                var apiStatus =
                    MapWorkStatus(
                        ReadNullableString(
                            reader,
                            "WorkStatus"
                        ),
                        ReadNullableString(
                            reader,
                            "ResultStatus"
                        )
                    );

                if (
                    !string.IsNullOrWhiteSpace(
                        status
                    )
                    &&
                    !string.Equals(
                        apiStatus,
                        status.Trim(),
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    continue;
                }

                result.Add(
                    new WorklistItemResponse
                    {
                        Id =
                            Convert.ToInt64(
                                reader["id"]
                            ),

                        SpecimenCode =
                            ReadNullableString(
                                reader,
                                "MaBarcode"
                            )
                            ?? "—",

                        TestName =
                            ReadNullableString(
                                reader,
                                "TenXetNghiem"
                            )
                            ?? "Xét nghiệm",

                        PatientName =
                            ReadNullableString(
                                reader,
                                "TenKhachHang"
                            )
                            ?? "Khách hàng",

                        AssignedAt =
                            ReadNullableDateTime(
                                reader,
                                "ReceivedAt"
                            ),

                        Status =
                            apiStatus
                    }
                );
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

    // =====================================================
    // START WORK
    // =====================================================

    public async Task StartWorkAsync(
        long worklistId,
        TechnicianIdentity technician,
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

        try
        {
            var context =
                await GetWorkContextForUpdateAsync(
                    connection,
                    dbTransaction,
                    worklistId,
                    cancellationToken
                );

            if (context is null)
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy worklist: {worklistId}"
                );
            }

            EnsureTechnicianCanUseWork(
                context,
                technician
            );

            if (
                context.WorkStatus != "queue"
                &&
                context.WorkStatus != "rerun"
            )
            {
                throw new BadRequestException(
                    "Công việc không ở trạng thái chờ thực hiện."
                );
            }

            var now =
                VietnamTime.Now;

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE worklist

                SET
                    IDKTV =
                        @employeeId,

                    Status =
                        'running',

                    StartedAt =
                        COALESCE(
                            StartedAt,
                            @now
                        )

                WHERE
                    id = @id
                    AND Status IN
                    (
                        'queue',
                        'rerun'
                    )
                """,

                cancellationToken,

                ("@employeeId", technician.EmployeeId),
                ("@now", now),
                ("@id", worklistId)
            );

            if (
                !string.IsNullOrWhiteSpace(
                    context.SpecimenId
                )
            )
            {
                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE maubenhpham

                    SET TrangThai =
                        'dang_xu_ly'

                    WHERE IDMau =
                        @id
                    """,

                    cancellationToken,

                    ("@id", context.SpecimenId)
                );
            }

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE ctphieuxetnghiem

                SET TrangThai =
                    'dang_xet_nghiem'

                WHERE IDCTPhieu =
                    @detailId
                """,

                cancellationToken,

                ("@detailId", context.DetailId)
            );

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE phieuxetnghiem

                SET TrangThai =
                    'dang_xet_nghiem'

                WHERE IDPhieuXetNghiem =
                    @orderId
                """,

                cancellationToken,

                ("@orderId", context.TestOrderId)
            );

            if (
                context.VisitId.HasValue
            )
            {
                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE luotxetnghiem

                    SET
                        TrangThai =
                            'dang_xet_nghiem',

                        ThoiGianBatDau =
                            COALESCE(
                                ThoiGianBatDau,
                                @now
                            )

                    WHERE
                        IDLuotXetNghiem =
                            @visitId
                    """,

                    cancellationToken,

                    ("@now", now),
                    ("@visitId", context.VisitId.Value)
                );
            }

            await InsertTrackingAsync(
                connection,
                dbTransaction,

                context.CustomerId,
                "luotxetnghiem",
                context.VisitId?.ToString()
                    ?? worklistId.ToString(),

                "Bắt đầu xét nghiệm",
                null,
                "dang_xet_nghiem",
                technician.UserId,
                now,
                $"KTV {technician.FullName} bắt đầu thực hiện xét nghiệm.",

                cancellationToken
            );

            await transaction.CommitAsync(
                cancellationToken
            );
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken
            );

            throw;
        }
    }

    // =====================================================
    // COMPLETE WORK
    // =====================================================

    public async Task CompleteWorkAsync(
        long worklistId,
        TechnicianIdentity technician,
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

        try
        {
            var context =
                await GetWorkContextForUpdateAsync(
                    connection,
                    dbTransaction,
                    worklistId,
                    cancellationToken
                );

            if (context is null)
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy worklist: {worklistId}"
                );
            }

            EnsureTechnicianCanUseWork(
                context,
                technician
            );

            if (
                context.WorkStatus != "running"
            )
            {
                throw new BadRequestException(
                    "Chỉ công việc đang thực hiện mới có thể hoàn tất."
                );
            }

            var now =
                VietnamTime.Now;

            await ExecuteAsync(
                connection,
                dbTransaction,

                """
                UPDATE worklist

                SET
                    Status =
                        'to_result',

                    FinishedAt =
                        COALESCE(
                            FinishedAt,
                            @now
                        )

                WHERE
                    id = @id
                    AND Status =
                        'running'
                """,

                cancellationToken,

                ("@now", now),
                ("@id", worklistId)
            );

            await transaction.CommitAsync(
                cancellationToken
            );
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken
            );

            throw;
        }
    }

    // =====================================================
    // GET RESULT ENTRY
    // =====================================================

    public async Task<ResultEntryResponse?>
        GetResultEntryAsync(
            long worklistId,
            TechnicianIdentity technician,
            CancellationToken cancellationToken = default
        )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
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
            long detailId;
            string testId;
            string patientName;
            string specimenCode;
            string testName;
            string? resultId;
            string? resultStatus;
            string? notes;

            await using (
                var command =
                    connection.CreateCommand()
            )
            {
                command.CommandText =
                    """
                    SELECT
                        w.IDCTPhieu,
                        w.IDKTV,

                        ct.IDXetNghiem,

                        k.TenKhachHang,

                        m.MaBarcode,

                        xn.TenXetNghiem,

                        kq.IDKetQua,
                        kq.TrangThai AS ResultStatus,
                        kq.GhiChu

                    FROM worklist w

                    INNER JOIN ctphieuxetnghiem ct
                        ON w.IDCTPhieu =
                           ct.IDCTPhieu

                    INNER JOIN phieuxetnghiem p
                        ON ct.IDPhieuXetNghiem =
                           p.IDPhieuXetNghiem

                    INNER JOIN khachhang k
                        ON p.IDKhachHang =
                           k.IDKhachHang

                    INNER JOIN loaixetnghiem xn
                        ON ct.IDXetNghiem =
                           xn.IDXetNghiem

                    LEFT JOIN maubenhpham m
                        ON w.IDMau =
                           m.IDMau

                    LEFT JOIN ketquaxetnghiem kq
                        ON kq.IDCTPhieu =
                           ct.IDCTPhieu

                    WHERE
                        w.id =
                            @id

                    LIMIT 1
                    """;

                AddParameter(
                    command,
                    "@id",
                    worklistId
                );

                await using var reader =
                    await command.ExecuteReaderAsync(
                        cancellationToken
                    );

                if (
                    !await reader.ReadAsync(
                        cancellationToken
                    )
                )
                {
                    return null;
                }

                var assignedTechnician =
                    ReadNullableString(
                        reader,
                        "IDKTV"
                    );

                if (
                    !string.IsNullOrWhiteSpace(
                        assignedTechnician
                    )
                    &&
                    !string.Equals(
                        assignedTechnician,
                        technician.EmployeeId,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new ForbiddenException(
                        "Worklist thuộc kỹ thuật viên khác."
                    );
                }

                detailId =
                    Convert.ToInt64(
                        reader["IDCTPhieu"]
                    );

                testId =
                    Convert.ToString(
                        reader["IDXetNghiem"]
                    )!;

                patientName =
                    Convert.ToString(
                        reader["TenKhachHang"]
                    )
                    ?? "Khách hàng";

                specimenCode =
                    ReadNullableString(
                        reader,
                        "MaBarcode"
                    )
                    ?? "—";

                testName =
                    Convert.ToString(
                        reader["TenXetNghiem"]
                    )
                    ?? "Xét nghiệm";

                resultId =
                    ReadNullableString(
                        reader,
                        "IDKetQua"
                    );

                resultStatus =
                    ReadNullableString(
                        reader,
                        "ResultStatus"
                    );

                notes =
                    ReadNullableString(
                        reader,
                        "GhiChu"
                    );
            }

            var response =
                new ResultEntryResponse
                {
                    WorklistId =
                        worklistId,

                    ResultId =
                        resultId,

                    PatientName =
                        patientName,

                    SpecimenCode =
                        specimenCode,

                    TestName =
                        testName,

                    Notes =
                        notes,

                    Status =
                        resultStatus
                        ?? string.Empty
                };

            await using (
                var indicatorCommand =
                    connection.CreateCommand()
            )
            {
                indicatorCommand.CommandText =
                    """
                    SELECT
                        cs.IDChiSo,
                        cs.TenChiSo,
                        cs.DonVi,

                        kqc.GiaTriText,

                        kqc.GiaTriThamChieu,

                        kqc.DanhGia

                    FROM chisoxetnghiem cs

                    LEFT JOIN ketquachiso kqc
                        ON kqc.IDChiSo =
                           cs.IDChiSo

                        AND kqc.IDKetQua =
                            @resultId

                    WHERE
                        cs.IDXetNghiem =
                            @testId

                        AND cs.Status =
                            'yes'

                    ORDER BY
                        cs.IDChiSo
                    """;

                AddParameter(
                    indicatorCommand,
                    "@resultId",
                    resultId
                );

                AddParameter(
                    indicatorCommand,
                    "@testId",
                    testId
                );

                await using var reader =
                    await indicatorCommand
                        .ExecuteReaderAsync(
                            cancellationToken
                        );

                while (
                    await reader.ReadAsync(
                        cancellationToken
                    )
                )
                {
                    var evaluation =
                        ReadNullableString(
                            reader,
                            "DanhGia"
                        );

                    response.Indicators.Add(
                        new ResultIndicatorResponse
                        {
                            IndicatorId =
                                Convert.ToString(
                                    reader["IDChiSo"]
                                )!,

                            Name =
                                Convert.ToString(
                                    reader["TenChiSo"]
                                )
                                ?? string.Empty,

                            Value =
                                ReadNullableString(
                                    reader,
                                    "GiaTriText"
                                ),

                            Unit =
                                ReadNullableString(
                                    reader,
                                    "DonVi"
                                ),

                            Reference =
                                ReadNullableString(
                                    reader,
                                    "GiaTriThamChieu"
                                ),

                            Abnormal =
                                evaluation is
                                    "bat_thuong"
                                    or "cao"
                                    or "thap"
                        }
                    );
                }
            }

            return response;
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // SAVE / SUBMIT RESULT
    // =====================================================

    public async Task SaveResultAsync(
        long worklistId,
        ResultEntryRequest request,
        TechnicianIdentity technician,
        bool submit,
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

        try
        {
            var context =
                await GetWorkContextForUpdateAsync(
                    connection,
                    dbTransaction,
                    worklistId,
                    cancellationToken
                );

            if (context is null)
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy worklist: {worklistId}"
                );
            }

            EnsureTechnicianCanUseWork(
                context,
                technician
            );

            if (
                context.WorkStatus != "running"
                &&
                context.WorkStatus != "to_result"
                &&
                context.WorkStatus != "finished"
            )
            {
                throw new BadRequestException(
                    "Worklist chưa sẵn sàng để nhập kết quả."
                );
            }

            var validIndicators =
                await GetValidIndicatorIdsAsync(
                    connection,
                    dbTransaction,
                    context.TestId,
                    cancellationToken
                );

            foreach (
                var indicator
                in request.Indicators
            )
            {
                if (
                    string.IsNullOrWhiteSpace(
                        indicator.IndicatorId
                    )
                    ||
                    !validIndicators.Contains(
                        indicator.IndicatorId
                    )
                )
                {
                    throw new BadRequestException(
                        $"Chỉ số '{indicator.IndicatorId}' không thuộc xét nghiệm này."
                    );
                }

                if (
                    submit
                    &&
                    string.IsNullOrWhiteSpace(
                        indicator.Value
                    )
                )
                {
                    throw new BadRequestException(
                        "Không được để trống kết quả chỉ số khi gửi duyệt."
                    );
                }
            }

            if (
                submit
                &&
                request.Indicators
                    .Select(
                        x => x.IndicatorId
                    )
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase
                    )
                    .Count()
                !=
                validIndicators.Count
            )
            {
                throw new BadRequestException(
                    "Vui lòng nhập đầy đủ tất cả chỉ số xét nghiệm trước khi gửi duyệt."
                );
            }

            var now =
                VietnamTime.Now;

            var resultInfo =
                await FindResultByDetailIdAsync(
                    connection,
                    dbTransaction,
                    context.DetailId,
                    cancellationToken
                );

            string resultId;

            if (
                resultInfo is null
            )
            {
                resultId =
                    await GenerateResultIdAsync(
                        connection,
                        dbTransaction,
                        cancellationToken
                    );

                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    INSERT INTO ketquaxetnghiem
                    (
                        IDKetQua,
                        IDCTPhieu,
                        IDMau,
                        IDKTV,
                        ThoiGianBatDau,
                        ThoiGianNhap,
                        TrangThai,
                        GhiChu
                    )
                    VALUES
                    (
                        @resultId,
                        @detailId,
                        @specimenId,
                        @employeeId,
                        @performedAt,
                        @enteredAt,
                        'dang_thuc_hien',
                        @notes
                    )
                    """,

                    cancellationToken,

                    ("@resultId", resultId),
                    ("@detailId", context.DetailId),
                    ("@specimenId", context.SpecimenId),
                    ("@employeeId", technician.EmployeeId),
                    (
                        "@performedAt",
                        context.StartedAt
                        ?? now
                    ),
                    ("@enteredAt", now),
                    ("@notes", NormalizeNullable(request.Notes))
                );
            }
            else
            {
                resultId =
                    resultInfo.Value.Id;

                if (
                    resultInfo.Value.Status
                        is "cho_duyet"
                        or "da_duyet"
                        or "hoan_tat"
                )
                {
                    throw new BadRequestException(
                        "Kết quả đã gửi duyệt hoặc đã được duyệt, không thể sửa."
                    );
                }

                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE ketquaxetnghiem

                    SET
                        IDKTV =
                            @employeeId,

                        GhiChu =
                            @notes

                    WHERE IDKetQua =
                        @resultId
                    """,

                    cancellationToken,

                    ("@employeeId", technician.EmployeeId),
                    ("@notes", NormalizeNullable(request.Notes)),
                    ("@resultId", resultId)
                );
            }

            foreach (
                var indicator
                in request.Indicators
            )
            {
                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    INSERT INTO ketquachiso
                    (
                        IDKetQua,
                        IDChiSo,
                        GiaTriText,
                        DanhGia,
                        Status
                    )
                    VALUES
                    (
                        @resultId,
                        @indicatorId,
                        @value,
                        @evaluation,
                        'yes'
                    )

                    ON DUPLICATE KEY UPDATE

                        GiaTriText =
                            VALUES(GiaTriText),

                        DanhGia =
                            VALUES(DanhGia),

                        Status =
                            'yes'
                    """,

                    cancellationToken,

                    ("@resultId", resultId),
                    ("@indicatorId", indicator.IndicatorId),
                    ("@value", NormalizeNullable(indicator.Value)),
                    (
                        "@evaluation",
                        indicator.Abnormal
                            ? "bat_thuong"
                            : "binh_thuong"
                    )
                );
            }

            if (submit)
            {
                var completedAt =
                    context.FinishedAt
                    ?? now;

                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE ketquaxetnghiem

                    SET
                        TrangThai =
                            'cho_duyet',

                        ThoiGianHoanThanh =
                            @completedAt,

                        GhiChu =
                            @notes

                    WHERE IDKetQua =
                        @resultId
                    """,

                    cancellationToken,

                    ("@completedAt", completedAt),
                    ("@notes", NormalizeNullable(request.Notes)),
                    ("@resultId", resultId)
                );

                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE worklist

                    SET
                        Status =
                            'finished',

                        FinishedAt =
                            COALESCE(
                                FinishedAt,
                                @completedAt
                            )

                    WHERE id =
                        @id
                    """,

                    cancellationToken,

                    ("@completedAt", completedAt),
                    ("@id", worklistId)
                );

                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE ctphieuxetnghiem

                    SET TrangThai =
                        'cho_duyet'

                    WHERE IDCTPhieu =
                        @detailId
                    """,

                    cancellationToken,

                    ("@detailId", context.DetailId)
                );

                await ExecuteAsync(
                    connection,
                    dbTransaction,

                    """
                    UPDATE phieuxetnghiem

                    SET TrangThai =
                        'cho_duyet'

                    WHERE IDPhieuXetNghiem =
                        @orderId
                    """,

                    cancellationToken,

                    ("@orderId", context.TestOrderId)
                );

                if (
                    context.VisitId.HasValue
                )
                {
                    await ExecuteAsync(
                        connection,
                        dbTransaction,

                        """
                        UPDATE luotxetnghiem

                        SET TrangThai =
                            'cho_duyet_kq'

                        WHERE IDLuotXetNghiem =
                            @visitId
                        """,

                        cancellationToken,

                        ("@visitId", context.VisitId.Value)
                    );
                }

                await InsertTrackingAsync(
                    connection,
                    dbTransaction,

                    context.CustomerId,

                    "luotxetnghiem",

                    context.VisitId?.ToString()
                        ?? worklistId.ToString(),

                    "KTV gửi kết quả chờ bác sĩ duyệt",

                    null,
                    "cho_duyet_kq",

                    technician.UserId,
                    now,

                    $"KTV {technician.FullName} đã gửi kết quả xét nghiệm sang bác sĩ duyệt.",

                    cancellationToken
                );
            }

            await transaction.CommitAsync(
                cancellationToken
            );
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken
            );

            throw;
        }
    }

    // =====================================================
    // WORKLIST BY SPECIMEN
    // =====================================================

    public async Task<long?>
        FindWorklistIdBySpecimenAsync(
            string specimenId,
            TechnicianIdentity technician,
            CancellationToken cancellationToken = default
        )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
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

            command.CommandText =
                """
                SELECT id

                FROM worklist

                WHERE
                    IDMau = @specimenId

                    AND
                    (
                        IDKTV =
                            @employeeId

                        OR IDKTV IS NULL
                    )

                ORDER BY id DESC

                LIMIT 1
                """;

            AddParameter(
                command,
                "@specimenId",
                specimenId
            );

            AddParameter(
                command,
                "@employeeId",
                technician.EmployeeId
            );

            var value =
                await command.ExecuteScalarAsync(
                    cancellationToken
                );

            if (
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
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private static string MapDbSpecimenStatusToApi(
        string? status
    )
    {
        return status switch
        {
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

            _ =>
                status?.ToUpperInvariant()
                ?? "UNKNOWN"
        };
    }

    private static string? MapApiSpecimenStatusToDb(
        string? status
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                status
            )
        )
        {
            return null;
        }

        return status
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "HANDED_OVER" =>
                "da_ban_giao",

            "RECEIVED" =>
                "ktv_tiep_nhan",

            "REJECTED" =>
                "tu_choi_mau",

            "PROCESSING" or
            "IN_PROGRESS" =>
                "dang_xu_ly",

            "COMPLETED" =>
                "hoan_tat",

            _ =>
                throw new BadRequestException(
                    $"Trạng thái mẫu '{status}' không hợp lệ."
                )
        };
    }

    private static string MapWorkStatus(
        string? workStatus,
        string? resultStatus
    )
    {
        if (
            resultStatus
                is "cho_duyet"
                or "da_duyet"
                or "hoan_tat"
        )
        {
            return "SUBMITTED";
        }

        if (
            resultStatus
                is "dang_thuc_hien"
                or "can_lam_lai"
        )
        {
            return "RESULT_ENTERED";
        }

        return workStatus switch
        {
            "queue" =>
                "PENDING",

            "rerun" =>
                "PENDING",

            "running" =>
                "IN_PROGRESS",

            "to_result" =>
                "COMPLETED",

            "finished" =>
                "COMPLETED",

            "cancelled" =>
                "CANCELLED",

            _ =>
                "PENDING"
        };
    }

    private static void EnsureTechnicianCanUseWork(
        WorkContext context,
        TechnicianIdentity technician
    )
    {
        if (
            !string.IsNullOrWhiteSpace(
                context.TechnicianId
            )
            &&
            !string.Equals(
                context.TechnicianId,
                technician.EmployeeId,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new ForbiddenException(
                "Worklist thuộc kỹ thuật viên khác."
            );
        }
    }

    private static async Task<HashSet<string>>
        GetValidIndicatorIdsAsync(
            DbConnection connection,
            DbTransaction transaction,
            string testId,
            CancellationToken cancellationToken
        )
    {
        var result =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            );

        await using var command =
            connection.CreateCommand();

        command.Transaction =
            transaction;

        command.CommandText =
            """
            SELECT IDChiSo

            FROM chisoxetnghiem

            WHERE
                IDXetNghiem =
                    @testId
                AND Status =
                    'yes'
            """;

        AddParameter(
            command,
            "@testId",
            testId
        );

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken
            );

        while (
            await reader.ReadAsync(
                cancellationToken
            )
        )
        {
            result.Add(
                Convert.ToString(
                    reader["IDChiSo"]
                )!
            );
        }

        return result;
    }

    private static async Task<(string Id, string Status)?>
        FindResultByDetailIdAsync(
            DbConnection connection,
            DbTransaction transaction,
            long detailId,
            CancellationToken cancellationToken
        )
    {
        await using var command =
            connection.CreateCommand();

        command.Transaction =
            transaction;

        command.CommandText =
            """
            SELECT
                IDKetQua,
                TrangThai

            FROM ketquaxetnghiem

            WHERE IDCTPhieu =
                @detailId

            LIMIT 1
            """;

        AddParameter(
            command,
            "@detailId",
            detailId
        );

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken
            );

        if (
            !await reader.ReadAsync(
                cancellationToken
            )
        )
        {
            return null;
        }

        return (
            Convert.ToString(
                reader["IDKetQua"]
            )!,
            Convert.ToString(
                reader["TrangThai"]
            )!
        );
    }

    private static async Task<string>
        GenerateResultIdAsync(
            DbConnection connection,
            DbTransaction transaction,
            CancellationToken cancellationToken
        )
    {
        for (
            var attempt = 0;
            attempt < 10;
            attempt++
        )
        {
            /*
             * Schema chỉ quy định varchar(20),
             * không có AUTO_INCREMENT/generator.
             *
             * KQ + 18 hex = đúng 20 ký tự.
             */
            var id =
                "KQ"
                +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 18)
                    .ToUpperInvariant();

            await using var command =
                connection.CreateCommand();

            command.Transaction =
                transaction;

            command.CommandText =
                """
                SELECT COUNT(*)

                FROM ketquaxetnghiem

                WHERE IDKetQua =
                    @id
                """;

            AddParameter(
                command,
                "@id",
                id
            );

            var count =
                Convert.ToInt32(
                    await command.ExecuteScalarAsync(
                        cancellationToken
                    )
                );

            if (count == 0)
            {
                return id;
            }
        }

        throw new InvalidOperationException(
            "Không thể tạo mã kết quả xét nghiệm duy nhất."
        );
    }

    private static async Task<SpecimenContext?>
        GetSpecimenContextForUpdateAsync(
            DbConnection connection,
            DbTransaction transaction,
            string specimenId,
            CancellationToken cancellationToken
        )
    {
        await using var command =
            connection.CreateCommand();

        command.Transaction =
            transaction;

        command.CommandText =
            """
            SELECT
                m.IDMau,
                m.IDCTPhieu,
                m.TrangThai AS SpecimenStatus,

                ct.IDPhieuXetNghiem,

                p.IDKhachHang,
                p.IDLuotXetNghiem,

                bg.IDBanGiao,
                bg.IDKTVTiepNhan,
                bg.TrangThai AS HandoverStatus

            FROM maubenhpham m

            INNER JOIN ctphieuxetnghiem ct
                ON m.IDCTPhieu =
                   ct.IDCTPhieu

            INNER JOIN phieuxetnghiem p
                ON ct.IDPhieuXetNghiem =
                   p.IDPhieuXetNghiem

            LEFT JOIN bangiaomau bg
                ON bg.IDBanGiao =
                (
                    SELECT MAX(bg2.IDBanGiao)

                    FROM bangiaomau bg2

                    WHERE bg2.IDMau =
                          m.IDMau
                )

            WHERE
                m.IDMau =
                    @specimenId

            LIMIT 1

            FOR UPDATE
            """;

        AddParameter(
            command,
            "@specimenId",
            specimenId
        );

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken
            );

        if (
            !await reader.ReadAsync(
                cancellationToken
            )
        )
        {
            return null;
        }

        return new SpecimenContext
        {
            DetailId =
                Convert.ToInt64(
                    reader["IDCTPhieu"]
                ),

            TestOrderId =
                Convert.ToString(
                    reader["IDPhieuXetNghiem"]
                )!,

            CustomerId =
                Convert.ToString(
                    reader["IDKhachHang"]
                )!,

            VisitId =
                ReadNullableInt64(
                    reader,
                    "IDLuotXetNghiem"
                ),

            SpecimenStatus =
                Convert.ToString(
                    reader["SpecimenStatus"]
                )
                ?? string.Empty,

            HandoverId =
                ReadNullableInt64(
                    reader,
                    "IDBanGiao"
                ),

            AssignedTechnicianId =
                ReadNullableString(
                    reader,
                    "IDKTVTiepNhan"
                ),

            HandoverStatus =
                ReadNullableString(
                    reader,
                    "HandoverStatus"
                )
        };
    }

    private static async Task<WorkContext?>
        GetWorkContextForUpdateAsync(
            DbConnection connection,
            DbTransaction transaction,
            long worklistId,
            CancellationToken cancellationToken
        )
    {
        await using var command =
            connection.CreateCommand();

        command.Transaction =
            transaction;

        command.CommandText =
            """
            SELECT
                w.id,
                w.IDCTPhieu,
                w.IDMau,
                w.IDKTV,
                w.Status AS WorkStatus,
                w.StartedAt,
                w.FinishedAt,

                ct.IDPhieuXetNghiem,
                ct.IDXetNghiem,

                p.IDKhachHang,
                p.IDLuotXetNghiem

            FROM worklist w

            INNER JOIN ctphieuxetnghiem ct
                ON w.IDCTPhieu =
                   ct.IDCTPhieu

            INNER JOIN phieuxetnghiem p
                ON ct.IDPhieuXetNghiem =
                   p.IDPhieuXetNghiem

            WHERE
                w.id =
                    @id

            LIMIT 1

            FOR UPDATE
            """;

        AddParameter(
            command,
            "@id",
            worklistId
        );

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken
            );

        if (
            !await reader.ReadAsync(
                cancellationToken
            )
        )
        {
            return null;
        }

        return new WorkContext
        {
            WorklistId =
                Convert.ToInt64(
                    reader["id"]
                ),

            DetailId =
                Convert.ToInt64(
                    reader["IDCTPhieu"]
                ),

            SpecimenId =
                ReadNullableString(
                    reader,
                    "IDMau"
                ),

            TechnicianId =
                ReadNullableString(
                    reader,
                    "IDKTV"
                ),

            WorkStatus =
                Convert.ToString(
                    reader["WorkStatus"]
                )
                ?? string.Empty,

            StartedAt =
                ReadNullableDateTime(
                    reader,
                    "StartedAt"
                ),

            FinishedAt =
                ReadNullableDateTime(
                    reader,
                    "FinishedAt"
                ),

            TestOrderId =
                Convert.ToString(
                    reader["IDPhieuXetNghiem"]
                )!,

            TestId =
                Convert.ToString(
                    reader["IDXetNghiem"]
                )!,

            CustomerId =
                Convert.ToString(
                    reader["IDKhachHang"]
                )!,

            VisitId =
                ReadNullableInt64(
                    reader,
                    "IDLuotXetNghiem"
                )
        };
    }

    private static async Task InsertTrackingAsync(
        DbConnection connection,
        DbTransaction transaction,
        string customerId,
        string objectType,
        string objectId,
        string action,
        string? oldStatus,
        string? newStatus,
        int userId,
        DateTime time,
        string? description,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                customerId
            )
        )
        {
            return;
        }

        await ExecuteAsync(
            connection,
            transaction,

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

            ("@customerId", customerId),
            ("@objectType", objectType),
            ("@objectId", objectId),
            ("@action", action),
            ("@oldStatus", oldStatus),
            ("@newStatus", newStatus),
            ("@userId", userId),
            ("@time", time),
            ("@description", description)
        );
    }

    private static async Task<int> ExecuteAsync(
        DbConnection connection,
        DbTransaction transaction,
        string sql,
        CancellationToken cancellationToken,
        params (string Name, object? Value)[] parameters
    )
    {
        await using var command =
            connection.CreateCommand();

        command.Transaction =
            transaction;

        command.CommandText =
            sql;

        foreach (
            var parameter in parameters
        )
        {
            AddParameter(
                command,
                parameter.Name,
                parameter.Value
            );
        }

        return await command.ExecuteNonQueryAsync(
            cancellationToken
        );
    }

    private static void AddParameter(
        DbCommand command,
        string name,
        object? value
    )
    {
        var parameter =
            command.CreateParameter();

        parameter.ParameterName =
            name;

        parameter.Value =
            value ?? DBNull.Value;

        command.Parameters.Add(
            parameter
        );
    }

    private static string? ReadNullableString(
        DbDataReader reader,
        string column
    )
    {
        var value =
            reader[column];

        return value == DBNull.Value
            ? null
            : Convert.ToString(value);
    }

    private static DateTime? ReadNullableDateTime(
        DbDataReader reader,
        string column
    )
    {
        var value =
            reader[column];

        return value == DBNull.Value
            ? null
            : Convert.ToDateTime(value);
    }

    private static long? ReadNullableInt64(
        DbDataReader reader,
        string column
    )
    {
        var value =
            reader[column];

        return value == DBNull.Value
            ? null
            : Convert.ToInt64(value);
    }

    private static string? NormalizeNullable(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private sealed class SpecimenContext
    {
        public long DetailId { get; init; }

        public string TestOrderId { get; init; }
            = string.Empty;

        public string CustomerId { get; init; }
            = string.Empty;

        public long? VisitId { get; init; }

        public string SpecimenStatus { get; init; }
            = string.Empty;

        public long? HandoverId { get; init; }

        public string? AssignedTechnicianId { get; init; }

        public string? HandoverStatus { get; init; }
    }

    private sealed class WorkContext
    {
        public long WorklistId { get; init; }

        public long DetailId { get; init; }

        public string? SpecimenId { get; init; }

        public string? TechnicianId { get; init; }

        public string WorkStatus { get; init; }
            = string.Empty;

        public DateTime? StartedAt { get; init; }

        public DateTime? FinishedAt { get; init; }

        public string TestOrderId { get; init; }
            = string.Empty;

        public string TestId { get; init; }
            = string.Empty;

        public string CustomerId { get; init; }
            = string.Empty;

        public long? VisitId { get; init; }
    }
}