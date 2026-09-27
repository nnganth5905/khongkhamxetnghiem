using System.Data;
using System.Data.Common;
using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class DoctorResultRepository
    : IDoctorResultRepository
{
    private readonly AppDbContext _dbContext;

    public DoctorResultRepository(
        AppDbContext dbContext
    )
    {
        _dbContext =
            dbContext;
    }

    // =====================================================
    // DOCTOR IDENTITY
    // =====================================================

    public async Task<DoctorIdentity?> FindDoctorIdentityAsync(
        string login,
        CancellationToken cancellationToken = default
    )
    {
        var connection =
            _dbContext.Database
                .GetDbConnection();

        var closeAfter =
            connection.State !=
            ConnectionState.Open;

        if (closeAfter)
        {
            await connection
                .OpenAsync(
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
                    UserID,
                    IDBacSi,
                    Email,
                    Username
                FROM users
                WHERE
                    (Username = @login OR Email = @login)
                    AND IDBacSi IS NOT NULL
                    AND IsActive = 1
                LIMIT 1
                """;

            AddParameter(
                command,
                "@login",
                login
            );

            await using var reader =
                await command
                    .ExecuteReaderAsync(
                        cancellationToken
                    );

            if (
                !await reader
                    .ReadAsync(
                        cancellationToken
                    )
            )
            {
                return null;
            }

            return new DoctorIdentity(
                Convert.ToInt32(
                    reader["UserID"]
                ),

                Convert.ToInt32(
                    reader["IDBacSi"]
                ),

                Convert.ToString(
                    reader["Email"]
                )
                ?? string.Empty,

                reader["Username"] ==
                DBNull.Value
                    ? null
                    : Convert.ToString(
                        reader["Username"]
                    )
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

    // =====================================================
    // PENDING RESULTS
    // =====================================================

    public async Task<List<PendingDoctorResultResponse>>
        GetPendingAsync(
            int doctorId,
            CancellationToken cancellationToken = default
        )
    {
        var results =
            new List<
                PendingDoctorResultResponse
            >();

        var connection =
            _dbContext.Database
                .GetDbConnection();

        var closeAfter =
            connection.State !=
            ConnectionState.Open;

        if (closeAfter)
        {
            await connection
                .OpenAsync(
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
                    kq.IDKetQua,
                    m.MaBarcode,
                    k.TenKhachHang,
                    lxn.TenXetNghiem,
                    nv.TenNhanVien AS TechnicianName,
                    kq.ThoiGianHoanThanh AS SubmittedAt

                FROM ketquaxetnghiem kq

                INNER JOIN ctphieuxetnghiem ct
                    ON kq.IDCTPhieu = ct.IDCTPhieu

                INNER JOIN phieuxetnghiem p
                    ON ct.IDPhieuXetNghiem =
                       p.IDPhieuXetNghiem

                LEFT JOIN luotxetnghiem luot
                    ON p.IDLuotXetNghiem =
                       luot.IDLuotXetNghiem

                LEFT JOIN datlichxetnghiem dl
                    ON luot.IDDatLichXN =
                       dl.IDDatLichXN

                LEFT JOIN khachhang k
                    ON p.IDKhachHang =
                       k.IDKhachHang

                LEFT JOIN loaixetnghiem lxn
                    ON ct.IDXetNghiem =
                       lxn.IDXetNghiem

                LEFT JOIN maubenhpham m
                    ON kq.IDMau =
                       m.IDMau

                LEFT JOIN nhanvien nv
                    ON kq.IDKTV =
                       nv.IDNhanVien

                WHERE
                    kq.TrangThai = 'cho_duyet'

                    AND
                    (
                        p.IDBacSiPhuTrach =
                            @doctorId

                        OR
                        (
                            p.IDBacSiPhuTrach IS NULL

                            AND
                            (
                                dl.IDBacSi =
                                    @doctorId

                                OR
                                dl.IDBacSi IS NULL
                            )
                        )
                    )

                ORDER BY
                    kq.ThoiGianHoanThanh DESC,
                    kq.ThoiGianNhap DESC
                """;

            AddParameter(
                command,
                "@doctorId",
                doctorId
            );

            await using var reader =
                await command
                    .ExecuteReaderAsync(
                        cancellationToken
                    );

            while (
                await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                results.Add(
                    new PendingDoctorResultResponse
                    {
                        Id =
                            Convert.ToString(
                                reader[
                                    "IDKetQua"
                                ]
                            )
                            ?? string.Empty,

                        SpecimenCode =
                            reader[
                                "MaBarcode"
                            ] == DBNull.Value
                                ? "Chưa rõ"
                                : Convert.ToString(
                                    reader[
                                        "MaBarcode"
                                    ]
                                  )
                                  ?? "Chưa rõ",

                        PatientName =
                            reader[
                                "TenKhachHang"
                            ] == DBNull.Value
                                ? "Khách hàng"
                                : Convert.ToString(
                                    reader[
                                        "TenKhachHang"
                                    ]
                                  )
                                  ?? "Khách hàng",

                        TestName =
                            reader[
                                "TenXetNghiem"
                            ] == DBNull.Value
                                ? "Xét nghiệm"
                                : Convert.ToString(
                                    reader[
                                        "TenXetNghiem"
                                    ]
                                  )
                                  ?? "Xét nghiệm",

                        TechnicianName =
                            reader[
                                "TechnicianName"
                            ] == DBNull.Value
                                ? "KTV hệ thống"
                                : Convert.ToString(
                                    reader[
                                        "TechnicianName"
                                    ]
                                  )
                                  ?? "KTV hệ thống",

                        SubmittedAt =
                            reader[
                                "SubmittedAt"
                            ] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    reader[
                                        "SubmittedAt"
                                    ]
                                )
                    }
                );
            }

            return results;
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

    // =====================================================
    // RESULT DETAIL
    // =====================================================

    public async Task<DoctorResultDetailResponse?> GetDetailAsync(
        string resultId,
        int doctorId,
        CancellationToken cancellationToken = default
    )
    {
        var connection =
            _dbContext.Database
                .GetDbConnection();

        var closeAfter =
            connection.State !=
            ConnectionState.Open;

        if (closeAfter)
        {
            await connection
                .OpenAsync(
                    cancellationToken
                );
        }

        try
        {
            DoctorResultDetailResponse?
                result = null;

            await using (
                var command =
                    connection.CreateCommand()
            )
            {
                command.CommandText =
                    """
                    SELECT
                        kq.IDKetQua,
                        m.MaBarcode,
                        k.TenKhachHang,
                        lxn.TenXetNghiem,
                        kq.GhiChu AS TechnicianNotes

                    FROM ketquaxetnghiem kq

                    INNER JOIN ctphieuxetnghiem ct
                        ON kq.IDCTPhieu =
                           ct.IDCTPhieu

                    INNER JOIN phieuxetnghiem p
                        ON ct.IDPhieuXetNghiem =
                           p.IDPhieuXetNghiem

                    LEFT JOIN luotxetnghiem luot
                        ON p.IDLuotXetNghiem =
                           luot.IDLuotXetNghiem

                    LEFT JOIN datlichxetnghiem dl
                        ON luot.IDDatLichXN =
                           dl.IDDatLichXN

                    INNER JOIN khachhang k
                        ON p.IDKhachHang =
                           k.IDKhachHang

                    INNER JOIN loaixetnghiem lxn
                        ON ct.IDXetNghiem =
                           lxn.IDXetNghiem

                    LEFT JOIN maubenhpham m
                        ON kq.IDMau =
                           m.IDMau

                    WHERE
                        kq.IDKetQua =
                            @resultId

                        AND
                        (
                            p.IDBacSiPhuTrach =
                                @doctorId

                            OR
                            (
                                p.IDBacSiPhuTrach
                                    IS NULL

                                AND
                                (
                                    dl.IDBacSi =
                                        @doctorId

                                    OR
                                    dl.IDBacSi IS NULL
                                )
                            )
                        )

                    LIMIT 1
                    """;

                AddParameter(
                    command,
                    "@resultId",
                    resultId
                );

                AddParameter(
                    command,
                    "@doctorId",
                    doctorId
                );

                await using var reader =
                    await command
                        .ExecuteReaderAsync(
                            cancellationToken
                        );

                if (
                    await reader
                        .ReadAsync(
                            cancellationToken
                        )
                )
                {
                    result =
                        new DoctorResultDetailResponse
                        {
                            Id =
                                Convert.ToString(
                                    reader[
                                        "IDKetQua"
                                    ]
                                )
                                ?? string.Empty,

                            SpecimenCode =
                                reader[
                                    "MaBarcode"
                                ] == DBNull.Value
                                    ? "Chưa rõ"
                                    : Convert.ToString(
                                        reader[
                                            "MaBarcode"
                                        ]
                                      )
                                      ?? "Chưa rõ",

                            PatientName =
                                Convert.ToString(
                                    reader[
                                        "TenKhachHang"
                                    ]
                                )
                                ?? "Khách hàng",

                            TestName =
                                Convert.ToString(
                                    reader[
                                        "TenXetNghiem"
                                    ]
                                )
                                ?? "Xét nghiệm",

                            TechnicianNotes =
                                reader[
                                    "TechnicianNotes"
                                ] == DBNull.Value
                                    ? null
                                    : Convert.ToString(
                                        reader[
                                            "TechnicianNotes"
                                        ]
                                    )
                        };
                }
            }

            if (result is null)
            {
                return null;
            }

            await using (
                var indicatorCommand =
                    connection.CreateCommand()
            )
            {
                indicatorCommand.CommandText =
                    """
                    SELECT
                        cs.TenChiSo AS Name,

                        COALESCE(
                            kqc.GiaTriText,
                            CAST(kqc.GiaTriSo AS CHAR)
                        ) AS Value,

                        cs.DonVi AS Unit,

                        kqc.DanhGia AS Evaluation

                    FROM ketquachiso kqc

                    INNER JOIN chisoxetnghiem cs
                        ON kqc.IDChiSo =
                           cs.IDChiSo

                    WHERE
                        kqc.IDKetQua =
                            @resultId

                        AND kqc.Status = 'yes'

                    ORDER BY
                        kqc.IDKetQuaChiSo
                    """;

                AddParameter(
                    indicatorCommand,
                    "@resultId",
                    resultId
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
                        reader[
                            "Evaluation"
                        ] == DBNull.Value
                            ? null
                            : Convert.ToString(
                                reader[
                                    "Evaluation"
                                ]
                            );

                    result.Indicators.Add(
                        new DoctorResultIndicatorResponse
                        {
                            Name =
                                Convert.ToString(
                                    reader[
                                        "Name"
                                    ]
                                )
                                ?? string.Empty,

                            Value =
                                reader[
                                    "Value"
                                ] == DBNull.Value
                                    ? null
                                    : Convert.ToString(
                                        reader[
                                            "Value"
                                        ]
                                    ),

                            Unit =
                                reader[
                                    "Unit"
                                ] == DBNull.Value
                                    ? null
                                    : Convert.ToString(
                                        reader[
                                            "Unit"
                                        ]
                                    ),

                            Evaluation =
                                evaluation,

                            Abnormal =
                                evaluation is
                                    "thap"
                                    or "cao"
                                    or "bat_thuong"
                        }
                    );
                }
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

    // =====================================================
    // APPROVE
    // =====================================================

    public async Task<ApproveResultResponse> ApproveAsync(
        string resultId,
        string conclusion,
        DoctorIdentity doctor,
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

        try
        {
            var context =
                await GetApprovalContextAsync(
                    connection,
                    transaction.GetDbTransaction(),
                    resultId,
                    cancellationToken
                );

            if (context is null)
            {
                throw new ResourceNotFoundException(
                    $"Không tìm thấy kết quả xét nghiệm: {resultId}"
                );
            }

            if (
                !string.Equals(
                    context.ResultStatus,
                    "cho_duyet",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    "Kết quả này không còn ở trạng thái chờ duyệt."
                );
            }

            /*
             * FIX security:
             * Không cho bác sĩ A duyệt kết quả
             * đã được giao cho bác sĩ B.
             */
            if (
                context.ResponsibleDoctorId.HasValue
                &&
                context.ResponsibleDoctorId.Value
                    != doctor.DoctorId
            )
            {
                throw new ForbiddenException(
                    "Kết quả này đang được phân công cho bác sĩ khác."
                );
            }

            if (
                !context.ResponsibleDoctorId.HasValue
                &&
                context.AppointmentDoctorId.HasValue
                &&
                context.AppointmentDoctorId.Value
                    != doctor.DoctorId
            )
            {
                throw new ForbiddenException(
                    "Lịch xét nghiệm này thuộc bác sĩ khác."
                );
            }

            var now =
                VietnamTime.Now;

            var affected =
                await ExecuteAsync(
                    connection,
                    transaction.GetDbTransaction(),

                    """
                    UPDATE ketquaxetnghiem

                    SET
                        TrangThai = 'da_duyet',
                        KetLuanBacSi = @conclusion,
                        IDBacSiDuyet = @doctorId,
                        ThoiGianDuyet = @now

                    WHERE
                        IDKetQua = @resultId
                        AND TrangThai = 'cho_duyet'
                    """,

                    cancellationToken,

                    ("@conclusion", conclusion),
                    ("@doctorId", doctor.DoctorId),
                    ("@now", now),
                    ("@resultId", resultId)
                );

            if (affected != 1)
            {
                throw new BadRequestException(
                    "Kết quả đã được xử lý bởi người khác. Vui lòng tải lại."
                );
            }

            await ExecuteAsync(
                connection,
                transaction.GetDbTransaction(),

                """
                UPDATE ctphieuxetnghiem

                SET TrangThai = 'da_duyet'

                WHERE IDCTPhieu = @id
                """,

                cancellationToken,

                ("@id", context.DetailId)
            );

            await ExecuteAsync(
                connection,
                transaction.GetDbTransaction(),

                """
                UPDATE phieuxetnghiem

                SET TrangThai = 'da_co_kq'

                WHERE IDPhieuXetNghiem = @id
                """,

                cancellationToken,

                ("@id", context.TestFormId)
            );

            if (
                !string.IsNullOrWhiteSpace(
                    context.SpecimenId
                )
            )
            {
                await ExecuteAsync(
                    connection,
                    transaction.GetDbTransaction(),

                    """
                    UPDATE maubenhpham

                    SET TrangThai = 'hoan_tat'

                    WHERE IDMau = @id
                    """,

                    cancellationToken,

                    ("@id", context.SpecimenId)
                );
            }

            if (
                context.VisitId.HasValue
            )
            {
                /*
                 * FIX timeline:
                 * lưu cả thời gian kết thúc,
                 * không chỉ đổi status.
                 */
                await ExecuteAsync(
                    connection,
                    transaction.GetDbTransaction(),

                    """
                    UPDATE luotxetnghiem

                    SET
                        TrangThai = 'hoan_tat',
                        ThoiGianKetThuc =
                            COALESCE(
                                ThoiGianKetThuc,
                                @now
                            )

                    WHERE
                        IDLuotXetNghiem =
                            @id
                    """,

                    cancellationToken,

                    ("@now", now),
                    ("@id", context.VisitId.Value)
                );
            }

            if (
                context.AppointmentId.HasValue
            )
            {
                await ExecuteAsync(
                    connection,
                    transaction.GetDbTransaction(),

                    """
                    UPDATE datlichxetnghiem

                    SET TrangThai = 'completed'

                    WHERE IDDatLichXN = @id
                    """,

                    cancellationToken,

                    ("@id", context.AppointmentId.Value)
                );

                /*
                 * FIX:
                 * hành động do bác sĩ thực hiện
                 * nên NguonThucHien = user,
                 * không phải system.
                 */
                await ExecuteAsync(
                    connection,
                    transaction.GetDbTransaction(),

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
                        'datlichxetnghiem',
                        @objectId,
                        'Đã có kết quả xét nghiệm',
                        @oldStatus,
                        'completed',
                        @userId,
                        'user',
                        @now,
                        'Bác sĩ đã phê duyệt và trả kết quả.'
                    )
                    """,

                    cancellationToken,

                    (
                        "@customerId",
                        context.CustomerId
                    ),

                    (
                        "@objectId",
                        context.AppointmentId
                            .Value
                            .ToString()
                    ),

                    (
                        "@oldStatus",
                        context.AppointmentStatus
                    ),

                    (
                        "@userId",
                        doctor.UserId
                    ),

                    ("@now", now)
                );
            }

            await transaction
                .CommitAsync(
                    cancellationToken
                );

            return new ApproveResultResponse
            {
                ApprovedAt =
                    now
            };
        }
        catch
        {
            await transaction
                .RollbackAsync(
                    cancellationToken
                );

            throw;
        }
    }

    // =====================================================
    // APPROVAL CONTEXT
    // =====================================================

    private static async Task<ApprovalContext?>
        GetApprovalContextAsync(
            DbConnection connection,
            DbTransaction transaction,
            string resultId,
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
                kq.IDCTPhieu,
                kq.IDMau,
                kq.TrangThai AS ResultStatus,

                ct.IDPhieuXetNghiem,

                p.IDLuotXetNghiem,
                p.IDKhachHang,
                p.IDBacSiPhuTrach,

                dl.IDDatLichXN,
                dl.IDBacSi AS AppointmentDoctorId,
                dl.TrangThai AS AppointmentStatus

            FROM ketquaxetnghiem kq

            INNER JOIN ctphieuxetnghiem ct
                ON kq.IDCTPhieu =
                   ct.IDCTPhieu

            INNER JOIN phieuxetnghiem p
                ON ct.IDPhieuXetNghiem =
                   p.IDPhieuXetNghiem

            LEFT JOIN luotxetnghiem luot
                ON p.IDLuotXetNghiem =
                   luot.IDLuotXetNghiem

            LEFT JOIN datlichxetnghiem dl
                ON luot.IDDatLichXN =
                   dl.IDDatLichXN

            WHERE
                kq.IDKetQua =
                    @resultId

            LIMIT 1
            FOR UPDATE
            """;

        AddParameter(
            command,
            "@resultId",
            resultId
        );

        await using var reader =
            await command
                .ExecuteReaderAsync(
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

        return new ApprovalContext
        {
            DetailId =
                Convert.ToInt64(
                    reader[
                        "IDCTPhieu"
                    ]
                ),

            SpecimenId =
                reader[
                    "IDMau"
                ] == DBNull.Value
                    ? null
                    : Convert.ToString(
                        reader[
                            "IDMau"
                        ]
                    ),

            ResultStatus =
                Convert.ToString(
                    reader[
                        "ResultStatus"
                    ]
                )
                ?? string.Empty,

            TestFormId =
                Convert.ToString(
                    reader[
                        "IDPhieuXetNghiem"
                    ]
                )
                ?? string.Empty,

            VisitId =
                reader[
                    "IDLuotXetNghiem"
                ] == DBNull.Value
                    ? null
                    : Convert.ToInt64(
                        reader[
                            "IDLuotXetNghiem"
                        ]
                    ),

            CustomerId =
                Convert.ToString(
                    reader[
                        "IDKhachHang"
                    ]
                )
                ?? string.Empty,

            ResponsibleDoctorId =
                reader[
                    "IDBacSiPhuTrach"
                ] == DBNull.Value
                    ? null
                    : Convert.ToInt32(
                        reader[
                            "IDBacSiPhuTrach"
                        ]
                    ),

            AppointmentId =
                reader[
                    "IDDatLichXN"
                ] == DBNull.Value
                    ? null
                    : Convert.ToInt64(
                        reader[
                            "IDDatLichXN"
                        ]
                    ),

            AppointmentDoctorId =
                reader[
                    "AppointmentDoctorId"
                ] == DBNull.Value
                    ? null
                    : Convert.ToInt32(
                        reader[
                            "AppointmentDoctorId"
                        ]
                    ),

            AppointmentStatus =
                reader[
                    "AppointmentStatus"
                ] == DBNull.Value
                    ? null
                    : Convert.ToString(
                        reader[
                            "AppointmentStatus"
                        ]
                    )
        };
    }

    // =====================================================
    // EXECUTE
    // =====================================================

    private static async Task<int> ExecuteAsync(
        DbConnection connection,
        DbTransaction transaction,
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

        command.Transaction =
            transaction;

        command.CommandText =
            sql;

        foreach (
            var parameter
            in parameters
        )
        {
            AddParameter(
                command,
                parameter.Name,
                parameter.Value
            );
        }

        return await command
            .ExecuteNonQueryAsync(
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
            value ??
            DBNull.Value;

        command.Parameters.Add(
            parameter
        );
    }

    private sealed class ApprovalContext
    {
        public long DetailId { get; init; }

        public string? SpecimenId { get; init; }

        public string ResultStatus { get; init; }
            = string.Empty;

        public string TestFormId { get; init; }
            = string.Empty;

        public long? VisitId { get; init; }

        public string CustomerId { get; init; }
            = string.Empty;

        public int? ResponsibleDoctorId { get; init; }

        public long? AppointmentId { get; init; }

        public int? AppointmentDoctorId { get; init; }

        public string? AppointmentStatus { get; init; }
    }
}