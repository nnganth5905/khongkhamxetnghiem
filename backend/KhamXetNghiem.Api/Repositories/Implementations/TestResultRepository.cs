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

public sealed class TestResultRepository
    : ITestResultRepository
{
    private readonly AppDbContext _dbContext;

    public TestResultRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // ACTOR
    // =====================================================

    public async Task<TestResultActor?> GetActorAsync(
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
                ON n.IDNhanVien = u.IDNhanVien

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

        return new TestResultActor(
            GetInt(row, "UserID")!.Value,
            GetInt(row, "IDBacSi"),
            GetString(row, "IDNhanVien"),
            GetString(row, "Role") ?? string.Empty,
            GetString(row, "ViTri"),
            GetString(row, "EmployeeStatus")
        );
    }

    // =====================================================
    // GET RESULT ENTRY
    // =====================================================

    public async Task<ResultEntryResponse> GetResultEntryAsync(
        long worklistId,
        string technicianId,
        CancellationToken cancellationToken = default
    )
    {
        var context =
            await GetWorkContextAsync(
                worklistId,
                technicianId,
                false,
                null,
                null,
                cancellationToken
            );

        var resultRows =
            await QueryAsync(
                """
                SELECT
                    IDKetQua,
                    KetQuaTongQuat,
                    TrangThai,
                    GhiChu
                FROM ketquaxetnghiem
                WHERE IDCTPhieu = @ctId
                LIMIT 1
                """,
                cancellationToken,
                (
                    "@ctId",
                    context.TestOrderItemId
                )
            );

        string? resultId = null;
        string? resultStatus = null;
        string? generalResult = null;
        string? notes = null;

        if (resultRows.Count > 0)
        {
            resultId =
                GetString(
                    resultRows[0],
                    "IDKetQua"
                );

            resultStatus =
                GetString(
                    resultRows[0],
                    "TrangThai"
                );

            generalResult =
                GetString(
                    resultRows[0],
                    "KetQuaTongQuat"
                );

            notes =
                GetString(
                    resultRows[0],
                    "GhiChu"
                );
        }

        var definitions =
            await GetIndicatorDefinitionsAsync(
                context.TestId,
                null,
                null,
                cancellationToken
            );

        var savedValues =
            new Dictionary<
                string,
                Dictionary<string, object?>
            >(
                StringComparer.OrdinalIgnoreCase
            );

        if (!string.IsNullOrWhiteSpace(resultId))
        {
            var savedRows =
                await QueryAsync(
                    """
                    SELECT
                        IDChiSo,
                        GiaTriSo,
                        GiaTriText,
                        NguongMinApDung,
                        NguongMaxApDung,
                        GiaTriThamChieu,
                        DanhGia,
                        GhiChu
                    FROM ketquachiso
                    WHERE
                        IDKetQua = @resultId
                        AND Status = 'yes'
                    """,
                    cancellationToken,
                    (
                        "@resultId",
                        resultId
                    )
                );

            foreach (var row in savedRows)
            {
                var indicatorId =
                    GetString(
                        row,
                        "IDChiSo"
                    );

                if (!string.IsNullOrWhiteSpace(indicatorId))
                {
                    savedValues[indicatorId] =
                        row;
                }
            }
        }

        var age =
            CalculateAge(
                context.BirthDate,
                VietnamTime.Now.Date
            );

        var indicators =
            new List<ResultIndicatorResponse>();

        foreach (var definition in definitions)
        {
            savedValues.TryGetValue(
                definition.Id,
                out var saved
            );

            var threshold =
                await GetThresholdAsync(
                    definition.Id,
                    context.Gender,
                    age,
                    null,
                    null,
                    cancellationToken
                );

            var numericValue =
                saved is null
                    ? null
                    : GetDecimal(
                        saved,
                        "GiaTriSo"
                    );

            var textValue =
                saved is null
                    ? null
                    : GetString(
                        saved,
                        "GiaTriText"
                    );

            var value =
                definition.DataType == "number"
                    ? (
                        numericValue?.ToString(
                            "0.####",
                            CultureInfo.InvariantCulture
                        )
                        ?? string.Empty
                    )
                    : textValue
                      ?? string.Empty;

            var min =
                saved is null
                    ? threshold?.Min
                    : GetDecimal(
                        saved,
                        "NguongMinApDung"
                    );

            var max =
                saved is null
                    ? threshold?.Max
                    : GetDecimal(
                        saved,
                        "NguongMaxApDung"
                    );

            var reference =
                saved is null
                    ? BuildReference(
                        threshold
                    )
                    : GetString(
                        saved,
                        "GiaTriThamChieu"
                    )
                      ?? BuildReference(
                          threshold
                      );

            indicators.Add(
                new ResultIndicatorResponse
                {
                    IndicatorId =
                        definition.Id,

                    Name =
                        definition.Name,

                    Unit =
                        definition.Unit,

                    DataType =
                        definition.DataType,

                    Value =
                        value,

                    NumericValue =
                        numericValue,

                    TextValue =
                        textValue,

                    Min =
                        min,

                    Max =
                        max,

                    Reference =
                        reference,

                    Evaluation =
                        saved is null
                            ? "chua_danh_gia"
                            : GetString(
                                  saved,
                                  "DanhGia"
                              )
                              ?? "chua_danh_gia",

                    Note =
                        saved is null
                            ? null
                            : GetString(
                                saved,
                                "GhiChu"
                            )
                }
            );
        }

        var editable =
            context.WorklistStatus ==
                "to_result"
            &&
            (
                resultStatus is null
                or "dang_thuc_hien"
                or "can_lam_lai"
            );

        return new ResultEntryResponse
        {
            WorklistId =
                context.WorklistId,

            ResultId =
                resultId,

            PatientName =
                context.PatientName,

            CustomerId =
                context.CustomerId,

            SpecimenId =
                context.SpecimenId,

            SpecimenCode =
                context.Barcode,

            TestId =
                context.TestId,

            TestName =
                context.TestName,

            WorklistStatus =
                MapWorklistStatusToApi(
                    context.WorklistStatus
                ),

            ResultStatus =
                resultStatus is null
                    ? null
                    : MapResultStatusToApi(
                        resultStatus
                    ),

            GeneralResult =
                generalResult,

            Notes =
                notes,

            Editable =
                editable,

            Indicators =
                indicators
        };
    }

    // =====================================================
    // SAVE / SUBMIT
    // =====================================================

    public async Task<TestResultActionResponse> SaveResultAsync(
        long worklistId,
        ResultEntryRequest request,
        bool submit,
        string technicianId,
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

        var context =
            await GetWorkContextAsync(
                worklistId,
                technicianId,
                true,
                connection,
                dbTransaction,
                cancellationToken
            );

        if (
            context.WorklistStatus !=
            "to_result"
        )
        {
            throw new BadRequestException(
                context.WorklistStatus switch
                {
                    "queue" =>
                        "Xét nghiệm chưa được bắt đầu.",

                    "running" =>
                        "Cần hoàn tất bước chạy xét nghiệm trước khi nhập kết quả.",

                    "finished" =>
                        "Kết quả đã được gửi duyệt.",

                    _ =>
                        $"Worklist đang ở trạng thái {context.WorklistStatus}."
                }
            );
        }

        var definitions =
            await GetIndicatorDefinitionsAsync(
                context.TestId,
                connection,
                dbTransaction,
                cancellationToken
            );

        if (definitions.Count == 0)
        {
            throw new BadRequestException(
                "Loại xét nghiệm chưa được cấu hình chỉ số."
            );
        }

        var requested =
            request.Indicators
                .Where(
                    x =>
                        !string.IsNullOrWhiteSpace(
                            x.IndicatorId
                        )
                )
                .GroupBy(
                    x => x.IndicatorId.Trim(),
                    StringComparer.OrdinalIgnoreCase
                )
                .ToDictionary(
                    x => x.Key,
                    x => x.Last(),
                    StringComparer.OrdinalIgnoreCase
                );

        // Không cho frontend gửi chỉ số không thuộc loại XN.
        var validIds =
            definitions
                .Select(
                    x => x.Id
                )
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase
                );

        var invalidId =
            requested.Keys
                .FirstOrDefault(
                    x => !validIds.Contains(x)
                );

        if (invalidId is not null)
        {
            throw new BadRequestException(
                $"Chỉ số {invalidId} không thuộc xét nghiệm {context.TestName}."
            );
        }

        if (submit)
        {
            foreach (var definition in definitions)
            {
                if (
                    !requested.TryGetValue(
                        definition.Id,
                        out var item
                    )
                    ||
                    string.IsNullOrWhiteSpace(
                        item.Value
                    )
                )
                {
                    throw new BadRequestException(
                        $"Chưa nhập kết quả cho chỉ số {definition.Name}."
                    );
                }
            }
        }

        var existingRows =
            await QueryAsync(
                """
                SELECT
                    IDKetQua,
                    IDKTV,
                    TrangThai
                FROM ketquaxetnghiem
                WHERE IDCTPhieu = @ctId
                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@ctId",
                    context.TestOrderItemId
                )
            );

        string resultId;
        string? previousResultStatus = null;

        if (existingRows.Count == 0)
        {
            resultId =
                await GenerateResultIdAsync(
                    connection,
                    dbTransaction,
                    cancellationToken
                );

            await ExecuteAsync(
                """
                INSERT INTO ketquaxetnghiem
                (
                    IDKetQua,
                    IDCTPhieu,
                    IDMau,
                    IDKTV,
                    ThoiGianBatDau,
                    ThoiGianNhap,
                    KetQuaTongQuat,
                    TrangThai,
                    GhiChu
                )
                VALUES
                (
                    @resultId,
                    @ctId,
                    @specimenId,
                    @technicianId,
                    @startedAt,
                    @enteredAt,
                    @generalResult,
                    'dang_thuc_hien',
                    @note
                )
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@resultId", resultId),
                (
                    "@ctId",
                    context.TestOrderItemId
                ),
                (
                    "@specimenId",
                    context.SpecimenId
                ),
                (
                    "@technicianId",
                    technicianId
                ),
                (
                    "@startedAt",
                    context.WorkStartedAt
                    ?? now
                ),
                ("@enteredAt", now),
                (
                    "@generalResult",
                    Truncate(
                        request.GeneralResult,
                        500
                    )
                ),
                (
                    "@note",
                    Truncate(
                        request.Notes,
                        500
                    )
                )
            );
        }
        else
        {
            resultId =
                GetString(
                    existingRows[0],
                    "IDKetQua"
                )!;

            previousResultStatus =
                GetString(
                    existingRows[0],
                    "TrangThai"
                );

            var owner =
                GetString(
                    existingRows[0],
                    "IDKTV"
                );

            if (
                !string.Equals(
                    owner,
                    technicianId,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new ForbiddenException(
                    "Kết quả này được nhập bởi kỹ thuật viên khác."
                );
            }

            if (
                previousResultStatus is
                    "cho_duyet"
                    or "da_duyet"
                    or "hoan_tat"
            )
            {
                throw new BadRequestException(
                    "Kết quả đã gửi duyệt hoặc đã được duyệt, không thể chỉnh sửa."
                );
            }

            await ExecuteAsync(
                """
                UPDATE ketquaxetnghiem
                SET
                    KetQuaTongQuat = @generalResult,
                    GhiChu = @note,
                    TrangThai = 'dang_thuc_hien'
                WHERE IDKetQua = @resultId
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@generalResult",
                    Truncate(
                        request.GeneralResult,
                        500
                    )
                ),
                (
                    "@note",
                    Truncate(
                        request.Notes,
                        500
                    )
                ),
                ("@resultId", resultId)
            );
        }

        var age =
            CalculateAge(
                context.BirthDate,
                now.Date
            );

        // =================================================
        // SAVE INDICATORS
        // =================================================

        foreach (var definition in definitions)
        {
            if (
                !requested.TryGetValue(
                    definition.Id,
                    out var input
                )
            )
            {
                // Lưu nháp có thể chưa nhập đủ.
                continue;
            }

            var raw =
                input.Value?.Trim();

            if (string.IsNullOrWhiteSpace(raw))
            {
                if (submit)
                {
                    throw new BadRequestException(
                        $"Chưa nhập giá trị {definition.Name}."
                    );
                }

                // Draft rỗng: không cần tạo record.
                continue;
            }

            var threshold =
                await GetThresholdAsync(
                    definition.Id,
                    context.Gender,
                    age,
                    connection,
                    dbTransaction,
                    cancellationToken
                );

            decimal? numericValue = null;
            string? textValue = null;

            string evaluation;

            if (
                definition.DataType ==
                "number"
            )
            {
                if (
                    !TryParseDecimal(
                        raw,
                        out var parsed
                    )
                )
                {
                    throw new BadRequestException(
                        $"Chỉ số {definition.Name} yêu cầu giá trị số."
                    );
                }

                numericValue =
                    parsed;

                evaluation =
                    EvaluateNumeric(
                        parsed,
                        threshold,
                        input.Abnormal
                    );
            }
            else
            {
                textValue =
                    raw;

                evaluation =
                    EvaluateText(
                        raw,
                        threshold,
                        input.Abnormal
                    );
            }

            var reference =
                BuildReference(
                    threshold
                );

            await ExecuteAsync(
                """
                INSERT INTO ketquachiso
                (
                    IDKetQua,
                    IDChiSo,
                    GiaTriSo,
                    GiaTriText,
                    NguongMinApDung,
                    NguongMaxApDung,
                    GiaTriThamChieu,
                    DanhGia,
                    GhiChu,
                    Status
                )
                VALUES
                (
                    @resultId,
                    @indicatorId,
                    @numericValue,
                    @textValue,
                    @min,
                    @max,
                    @reference,
                    @evaluation,
                    @note,
                    'yes'
                )

                ON DUPLICATE KEY UPDATE

                    GiaTriSo =
                        VALUES(GiaTriSo),

                    GiaTriText =
                        VALUES(GiaTriText),

                    NguongMinApDung =
                        VALUES(NguongMinApDung),

                    NguongMaxApDung =
                        VALUES(NguongMaxApDung),

                    GiaTriThamChieu =
                        VALUES(GiaTriThamChieu),

                    DanhGia =
                        VALUES(DanhGia),

                    GhiChu =
                        VALUES(GhiChu),

                    Status =
                        'yes'
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@resultId", resultId),
                (
                    "@indicatorId",
                    definition.Id
                ),
                (
                    "@numericValue",
                    numericValue
                ),
                (
                    "@textValue",
                    textValue
                ),
                (
                    "@min",
                    threshold?.Min
                ),
                (
                    "@max",
                    threshold?.Max
                ),
                (
                    "@reference",
                    reference
                ),
                (
                    "@evaluation",
                    evaluation
                ),
                (
                    "@note",
                    Truncate(
                        input.Note,
                        255
                    )
                )
            );
        }

        // =================================================
        // SUBMIT
        // =================================================

        if (submit)
        {
            var savedCount =
                Convert.ToInt32(
                    await ScalarAsync(
                        """
                        SELECT COUNT(*)
                        FROM ketquachiso
                        WHERE
                            IDKetQua = @resultId
                            AND Status = 'yes'
                            AND
                            (
                                GiaTriSo IS NOT NULL
                                OR
                                (
                                    GiaTriText IS NOT NULL
                                    AND TRIM(GiaTriText) <> ''
                                )
                            )
                        """,
                        cancellationToken,
                        connection,
                        dbTransaction,
                        (
                            "@resultId",
                            resultId
                        )
                    )
                    ?? 0
                );

            if (
                savedCount <
                definitions.Count
            )
            {
                throw new BadRequestException(
                    "Chưa nhập đầy đủ tất cả chỉ số xét nghiệm."
                );
            }

            await ExecuteAsync(
                """
                UPDATE ketquaxetnghiem
                SET
                    TrangThai = 'cho_duyet',
                    ThoiGianHoanThanh = @now
                WHERE IDKetQua = @resultId
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@now", now),
                ("@resultId", resultId)
            );

            await ExecuteAsync(
                """
                UPDATE worklist
                SET
                    Status = 'finished',
                    FinishedAt = @now
                WHERE id = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@now", now),
                ("@id", worklistId)
            );

            await ExecuteAsync(
                """
                UPDATE ctphieuxetnghiem
                SET TrangThai = 'cho_duyet'
                WHERE IDCTPhieu = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                (
                    "@id",
                    context.TestOrderItemId
                )
            );

            await SyncOrderAfterSubmitAsync(
                context.TestOrderId,
                context.VisitId,
                connection,
                dbTransaction,
                cancellationToken
            );

            await InsertTrackingAsync(
                connection,
                dbTransaction,
                context.CustomerId,
                "ketquaxetnghiem",
                resultId,
                "KTV gửi kết quả chờ bác sĩ duyệt",
                previousResultStatus
                ?? "dang_thuc_hien",
                "cho_duyet",
                actorUserId,
                $"Xét nghiệm: {context.TestName}",
                cancellationToken
            );
        }
        else
        {
            await InsertTrackingAsync(
                connection,
                dbTransaction,
                context.CustomerId,
                "ketquaxetnghiem",
                resultId,
                "KTV lưu nháp kết quả xét nghiệm",
                previousResultStatus,
                "dang_thuc_hien",
                actorUserId,
                $"Xét nghiệm: {context.TestName}",
                cancellationToken
            );
        }

        await transaction.CommitAsync(
            cancellationToken
        );

        return new TestResultActionResponse
        {
            Message =
                submit
                    ? "Đã gửi kết quả sang bác sĩ duyệt."
                    : "Đã lưu nháp kết quả.",

            ResultId =
                resultId,

            Status =
                submit
                    ? "PENDING_APPROVAL"
                    : "IN_PROGRESS"
        };
    }

    // =====================================================
    // PENDING RESULTS FOR DOCTOR
    // =====================================================

    public async Task<List<PendingDoctorResultResponse>>
        GetPendingDoctorResultsAsync(
            int doctorId,
            CancellationToken cancellationToken = default
        )
    {
        var rows = await QueryAsync(
            """
            SELECT
                kq.IDKetQua,
                m.MaBarcode,
                k.TenKhachHang,
                xn.TenXetNghiem,
                nv.TenNhanVien AS TechnicianName,
                kq.ThoiGianHoanThanh,

                EXISTS
                (
                    SELECT 1
                    FROM ketquachiso kqc
                    WHERE
                        kqc.IDKetQua = kq.IDKetQua
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
                ON ct.IDCTPhieu = kq.IDCTPhieu

            JOIN phieuxetnghiem p
                ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem

            LEFT JOIN luotxetnghiem lx
                ON lx.IDLuotXetNghiem = p.IDLuotXetNghiem

            LEFT JOIN datlichxetnghiem d
                ON d.IDDatLichXN = lx.IDDatLichXN

            JOIN khachhang k
                ON k.IDKhachHang = p.IDKhachHang

            JOIN loaixetnghiem xn
                ON xn.IDXetNghiem = ct.IDXetNghiem

            LEFT JOIN maubenhpham m
                ON m.IDMau = kq.IDMau

            JOIN nhanvien nv
                ON nv.IDNhanVien = kq.IDKTV

            WHERE
                kq.TrangThai = 'cho_duyet'

                AND
                (
                    p.IDBacSiPhuTrach = @doctorId

                    OR
                    (
                        p.IDBacSiPhuTrach IS NULL
                        AND d.IDBacSi = @doctorId
                    )

                    OR
                    (
                        p.IDBacSiPhuTrach IS NULL
                        AND d.IDBacSi IS NULL
                    )
                )

            ORDER BY
                HasAbnormal DESC,
                kq.ThoiGianHoanThanh ASC
            """,
            cancellationToken,
            ("@doctorId", doctorId)
        );

        return rows.Select(
            row =>
                new PendingDoctorResultResponse
                {
                    Id =
                        GetString(
                            row,
                            "IDKetQua"
                        )!,

                    SpecimenCode =
                        GetString(
                            row,
                            "MaBarcode"
                        )
                        ?? "—",

                    PatientName =
                        GetString(
                            row,
                            "TenKhachHang"
                        )
                        ?? "—",

                    TestName =
                        GetString(
                            row,
                            "TenXetNghiem"
                        )
                        ?? "—",

                    TechnicianName =
                        GetString(
                            row,
                            "TechnicianName"
                        )
                        ?? "—",

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
        ).ToList();
    }

    // =====================================================
    // DOCTOR RESULT DETAIL
    // =====================================================

    public async Task<DoctorResultDetailResponse> GetDoctorResultAsync(
        string resultId,
        int doctorId,
        CancellationToken cancellationToken = default
    )
    {
        var rows = await QueryAsync(
            """
            SELECT
                kq.IDKetQua,
                kq.IDMau,
                kq.TrangThai,
                kq.ThoiGianHoanThanh,
                kq.KetQuaTongQuat,
                kq.KetLuanBacSi,
                kq.GhiChu,

                m.MaBarcode,

                p.IDKhachHang,

                k.TenKhachHang,

                xn.TenXetNghiem,

                nv.TenNhanVien AS TechnicianName

            FROM ketquaxetnghiem kq

            JOIN ctphieuxetnghiem ct
                ON ct.IDCTPhieu = kq.IDCTPhieu

            JOIN phieuxetnghiem p
                ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem

            LEFT JOIN luotxetnghiem lx
                ON lx.IDLuotXetNghiem = p.IDLuotXetNghiem

            LEFT JOIN datlichxetnghiem d
                ON d.IDDatLichXN = lx.IDDatLichXN

            JOIN khachhang k
                ON k.IDKhachHang = p.IDKhachHang

            JOIN loaixetnghiem xn
                ON xn.IDXetNghiem = ct.IDXetNghiem

            LEFT JOIN maubenhpham m
                ON m.IDMau = kq.IDMau

            JOIN nhanvien nv
                ON nv.IDNhanVien = kq.IDKTV

            WHERE
                kq.IDKetQua = @resultId

                AND
                (
                    p.IDBacSiPhuTrach = @doctorId

                    OR
                    (
                        p.IDBacSiPhuTrach IS NULL
                        AND d.IDBacSi = @doctorId
                    )

                    OR
                    (
                        p.IDBacSiPhuTrach IS NULL
                        AND d.IDBacSi IS NULL
                    )
                )

            LIMIT 1
            """,
            cancellationToken,
            ("@resultId", resultId),
            ("@doctorId", doctorId)
        );

        if (rows.Count == 0)
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy kết quả hoặc kết quả không thuộc phạm vi phụ trách của bác sĩ."
            );
        }

        var row =
            rows[0];

        var indicatorRows =
            await QueryAsync(
                """
                SELECT
                    cs.IDChiSo,
                    cs.TenChiSo,
                    cs.DonVi,
                    cs.KieuDuLieu,

                    kqc.GiaTriSo,
                    kqc.GiaTriText,
                    kqc.NguongMinApDung,
                    kqc.NguongMaxApDung,
                    kqc.GiaTriThamChieu,
                    kqc.DanhGia,
                    kqc.GhiChu

                FROM ketquachiso kqc

                JOIN chisoxetnghiem cs
                    ON cs.IDChiSo = kqc.IDChiSo

                WHERE
                    kqc.IDKetQua = @resultId
                    AND kqc.Status = 'yes'

                ORDER BY kqc.IDKetQuaChiSo
                """,
                cancellationToken,
                ("@resultId", resultId)
            );

        var indicators =
            indicatorRows
                .Select(MapIndicator)
                .ToList();

        return new DoctorResultDetailResponse
        {
            Id =
                resultId,

            SpecimenId =
                GetString(
                    row,
                    "IDMau"
                )
                ?? string.Empty,

            SpecimenCode =
                GetString(
                    row,
                    "MaBarcode"
                )
                ?? "—",

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

            TestName =
                GetString(
                    row,
                    "TenXetNghiem"
                )!,

            TechnicianName =
                GetString(
                    row,
                    "TechnicianName"
                )!,

            Status =
                MapResultStatusToApi(
                    GetString(
                        row,
                        "TrangThai"
                    )!
                ),

            SubmittedAt =
                GetDateTime(
                    row,
                    "ThoiGianHoanThanh"
                ),

            TechnicianNotes =
                GetString(
                    row,
                    "GhiChu"
                ),

            GeneralResult =
                GetString(
                    row,
                    "KetQuaTongQuat"
                ),

            DoctorConclusion =
                GetString(
                    row,
                    "KetLuanBacSi"
                ),

            Indicators =
                indicators
        };
    }

    // =====================================================
    // APPROVE
    // =====================================================

    public async Task<TestResultActionResponse> ApproveAsync(
        string resultId,
        string conclusion,
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
                    kq.IDKetQua,
                    kq.IDCTPhieu,
                    kq.IDMau,
                    kq.TrangThai,

                    ct.IDPhieuXetNghiem,

                    p.IDKhachHang,
                    p.IDLuotXetNghiem,
                    p.IDBacSiPhuTrach,

                    d.IDBacSi AS AppointmentDoctorId

                FROM ketquaxetnghiem kq

                JOIN ctphieuxetnghiem ct
                    ON ct.IDCTPhieu = kq.IDCTPhieu

                JOIN phieuxetnghiem p
                    ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem

                LEFT JOIN luotxetnghiem lx
                    ON lx.IDLuotXetNghiem = p.IDLuotXetNghiem

                LEFT JOIN datlichxetnghiem d
                    ON d.IDDatLichXN = lx.IDDatLichXN

                WHERE kq.IDKetQua = @resultId

                LIMIT 1
                FOR UPDATE
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@resultId", resultId)
            );

        if (rows.Count == 0)
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy kết quả xét nghiệm."
            );
        }

        var row =
            rows[0];

        var responsibleDoctor =
            GetInt(
                row,
                "IDBacSiPhuTrach"
            );

        var appointmentDoctor =
            GetInt(
                row,
                "AppointmentDoctorId"
            );

        if (
            responsibleDoctor.HasValue
            &&
            responsibleDoctor.Value != doctorId
        )
        {
            throw new ForbiddenException(
                "Kết quả được phân công cho bác sĩ khác."
            );
        }

        if (
            !responsibleDoctor.HasValue
            &&
            appointmentDoctor.HasValue
            &&
            appointmentDoctor.Value != doctorId
        )
        {
            throw new ForbiddenException(
                "Lịch xét nghiệm thuộc bác sĩ khác."
            );
        }

        var currentStatus =
            GetString(
                row,
                "TrangThai"
            )!;

        if (
            currentStatus !=
            "cho_duyet"
        )
        {
            throw new BadRequestException(
                currentStatus switch
                {
                    "da_duyet" =>
                        "Kết quả đã được duyệt.",

                    "dang_thuc_hien" =>
                        "KTV chưa gửi kết quả duyệt.",

                    _ =>
                        $"Không thể duyệt kết quả ở trạng thái {currentStatus}."
                }
            );
        }

        var ctId =
            GetLong(
                row,
                "IDCTPhieu"
            )!.Value;

        var specimenId =
            GetString(
                row,
                "IDMau"
            );

        var testOrderId =
            GetString(
                row,
                "IDPhieuXetNghiem"
            )!;

        var customerId =
            GetString(
                row,
                "IDKhachHang"
            )!;

        var visitId =
            GetLong(
                row,
                "IDLuotXetNghiem"
            );

        await ExecuteAsync(
            """
            UPDATE ketquaxetnghiem
            SET
                TrangThai = 'da_duyet',
                IDBacSiDuyet = @doctorId,
                ThoiGianDuyet = @now,
                KetLuanBacSi = @conclusion
            WHERE IDKetQua = @resultId
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@doctorId", doctorId),
            ("@now", now),
            (
                "@conclusion",
                conclusion
            ),
            ("@resultId", resultId)
        );

        await ExecuteAsync(
            """
            UPDATE ctphieuxetnghiem
            SET TrangThai = 'da_duyet'
            WHERE IDCTPhieu = @id
            """,
            cancellationToken,
            connection,
            dbTransaction,
            ("@id", ctId)
        );

        if (!string.IsNullOrWhiteSpace(specimenId))
        {
            await ExecuteAsync(
                """
                UPDATE maubenhpham
                SET TrangThai = 'hoan_tat'
                WHERE IDMau = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", specimenId)
            );
        }

        /*
         * Không được chuyển cả lượt XN sang hoan_tat
         * ngay khi duyệt một kết quả.
         *
         * Một phiếu có thể có nhiều xét nghiệm.
         */
        var pendingCount =
            Convert.ToInt32(
                await ScalarAsync(
                    """
                    SELECT COUNT(*)
                    FROM ctphieuxetnghiem
                    WHERE
                        IDPhieuXetNghiem = @orderId
                        AND Status = 'yes'
                        AND TrangThai NOT IN
                        (
                            'da_duyet',
                            'hoan_tat'
                        )
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@orderId", testOrderId)
                )
                ?? 0
            );

        if (pendingCount == 0)
        {
            await ExecuteAsync(
                """
                UPDATE phieuxetnghiem
                SET TrangThai = 'da_co_kq'
                WHERE IDPhieuXetNghiem = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", testOrderId)
            );

            if (visitId.HasValue)
            {
                await ExecuteAsync(
                    """
                    UPDATE luotxetnghiem
                    SET TrangThai = 'da_co_kq'
                    WHERE IDLuotXetNghiem = @id
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", visitId.Value)
                );
            }
        }
        else
        {
            await ExecuteAsync(
                """
                UPDATE phieuxetnghiem
                SET TrangThai = 'cho_duyet'
                WHERE IDPhieuXetNghiem = @id
                """,
                cancellationToken,
                connection,
                dbTransaction,
                ("@id", testOrderId)
            );

            if (visitId.HasValue)
            {
                await ExecuteAsync(
                    """
                    UPDATE luotxetnghiem
                    SET TrangThai = 'cho_duyet_kq'
                    WHERE IDLuotXetNghiem = @id
                    """,
                    cancellationToken,
                    connection,
                    dbTransaction,
                    ("@id", visitId.Value)
                );
            }
        }

        await InsertTrackingAsync(
            connection,
            dbTransaction,
            customerId,
            "ketquaxetnghiem",
            resultId,
            "Bác sĩ duyệt kết quả xét nghiệm",
            "cho_duyet",
            "da_duyet",
            actorUserId,
            conclusion,
            cancellationToken
        );

        await InsertResultNotificationAsync(
            connection,
            dbTransaction,
            customerId,
            resultId,
            cancellationToken
        );

        await transaction.CommitAsync(
            cancellationToken
        );

        return new TestResultActionResponse
        {
            Message =
                "Duyệt kết quả xét nghiệm thành công.",

            ResultId =
                resultId,

            Status =
                "APPROVED"
        };
    }

    // =====================================================
    // WORK CONTEXT
    // =====================================================

    private async Task<ResultWorkContext> GetWorkContextAsync(
        long worklistId,
        string technicianId,
        bool forUpdate,
        DbConnection? connection,
        DbTransaction? transaction,
        CancellationToken cancellationToken
    )
    {
        var sql =
            $"""
            SELECT
                w.id AS WorklistId,
                w.IDCTPhieu,
                w.IDMau,
                w.IDKTV,
                w.Status AS WorkStatus,
                w.StartedAt,

                ct.IDPhieuXetNghiem,
                ct.IDXetNghiem,

                xn.TenXetNghiem,

                p.IDKhachHang,
                p.IDLuotXetNghiem,
                p.IDBacSiPhuTrach,

                k.TenKhachHang,
                k.NgaySinh,
                k.GioiTinh,

                m.MaBarcode,

                d.IDBacSi AS AppointmentDoctorId

            FROM worklist w

            JOIN ctphieuxetnghiem ct
                ON ct.IDCTPhieu = w.IDCTPhieu

            JOIN loaixetnghiem xn
                ON xn.IDXetNghiem = ct.IDXetNghiem

            JOIN phieuxetnghiem p
                ON p.IDPhieuXetNghiem = ct.IDPhieuXetNghiem

            JOIN khachhang k
                ON k.IDKhachHang = p.IDKhachHang

            LEFT JOIN maubenhpham m
                ON m.IDMau = w.IDMau

            LEFT JOIN luotxetnghiem lx
                ON lx.IDLuotXetNghiem = p.IDLuotXetNghiem

            LEFT JOIN datlichxetnghiem d
                ON d.IDDatLichXN = lx.IDDatLichXN

            WHERE
                w.id = @worklistId

            LIMIT 1

            {(forUpdate ? "FOR UPDATE" : string.Empty)}
            """;

        List<Dictionary<string, object?>> rows;

        if (connection is null)
        {
            rows =
                await QueryAsync(
                    sql,
                    cancellationToken,
                    (
                        "@worklistId",
                        worklistId
                    )
                );
        }
        else
        {
            rows =
                await QueryAsync(
                    sql,
                    cancellationToken,
                    connection,
                    transaction,
                    (
                        "@worklistId",
                        worklistId
                    )
                );
        }

        if (rows.Count == 0)
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy worklist."
            );
        }

        var row =
            rows[0];

        var owner =
            GetString(
                row,
                "IDKTV"
            );

        if (
            string.IsNullOrWhiteSpace(owner)
            ||
            !string.Equals(
                owner,
                technicianId,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new ForbiddenException(
                "Worklist không thuộc kỹ thuật viên đang đăng nhập."
            );
        }

        var specimenId =
            GetString(
                row,
                "IDMau"
            );

        if (string.IsNullOrWhiteSpace(specimenId))
        {
            throw new BadRequestException(
                "Worklist chưa gắn mẫu bệnh phẩm."
            );
        }

        return new ResultWorkContext(
            GetLong(
                row,
                "WorklistId"
            )!.Value,

            GetLong(
                row,
                "IDCTPhieu"
            )!.Value,

            GetString(
                row,
                "IDPhieuXetNghiem"
            )!,

            GetString(
                row,
                "IDXetNghiem"
            )!,

            GetString(
                row,
                "TenXetNghiem"
            )!,

            specimenId,

            GetString(
                row,
                "MaBarcode"
            )
            ?? specimenId,

            owner,

            GetString(
                row,
                "WorkStatus"
            )!,

            GetDateTime(
                row,
                "StartedAt"
            ),

            GetString(
                row,
                "IDKhachHang"
            )!,

            GetString(
                row,
                "TenKhachHang"
            )!,

            GetDateTime(
                row,
                "NgaySinh"
            ),

            GetString(
                row,
                "GioiTinh"
            ),

            GetLong(
                row,
                "IDLuotXetNghiem"
            ),

            GetInt(
                row,
                "AppointmentDoctorId"
            ),

            GetInt(
                row,
                "IDBacSiPhuTrach"
            )
        );
    }

    // =====================================================
    // INDICATORS
    // =====================================================

    private async Task<List<IndicatorDefinition>>
        GetIndicatorDefinitionsAsync(
            string testId,
            DbConnection? connection,
            DbTransaction? transaction,
            CancellationToken cancellationToken
        )
    {
        const string sql =
            """
            SELECT
                IDChiSo,
                TenChiSo,
                DonVi,
                KieuDuLieu
            FROM chisoxetnghiem
            WHERE
                IDXetNghiem = @testId
                AND Status = 'yes'
            ORDER BY IDChiSo
            """;

        List<Dictionary<string, object?>> rows;

        if (connection is null)
        {
            rows =
                await QueryAsync(
                    sql,
                    cancellationToken,
                    ("@testId", testId)
                );
        }
        else
        {
            rows =
                await QueryAsync(
                    sql,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@testId", testId)
                );
        }

        return rows.Select(
            row =>
                new IndicatorDefinition(
                    GetString(
                        row,
                        "IDChiSo"
                    )!,

                    GetString(
                        row,
                        "TenChiSo"
                    )!,

                    GetString(
                        row,
                        "DonVi"
                    ),

                    GetString(
                        row,
                        "KieuDuLieu"
                    )
                    ?? "number"
                )
        ).ToList();
    }

    // =====================================================
    // THRESHOLD
    // =====================================================

    private async Task<IndicatorThreshold?> GetThresholdAsync(
        string indicatorId,
        string? gender,
        int? age,
        DbConnection? connection,
        DbTransaction? transaction,
        CancellationToken cancellationToken
    )
    {
        var normalizedGender =
            NormalizeGender(
                gender
            );

        const string sql =
            """
            SELECT
                GiaTriMin,
                GiaTriMax,
                GiaTriTextBinhThuong,
                GhiChu
            FROM nguongchisoxetnghiem

            WHERE
                IDChiSo = @indicatorId

                AND Status = 'yes'

                AND
                (
                    GioiTinhApDung = 'tatca'
                    OR GioiTinhApDung = @gender
                )

                AND
                (
                    @age IS NULL
                    OR TuoiMin IS NULL
                    OR TuoiMin <= @age
                )

                AND
                (
                    @age IS NULL
                    OR TuoiMax IS NULL
                    OR TuoiMax >= @age
                )

            ORDER BY
                CASE
                    WHEN GioiTinhApDung = @gender
                    THEN 0
                    ELSE 1
                END,

                CASE
                    WHEN TuoiMin IS NOT NULL
                         OR TuoiMax IS NOT NULL
                    THEN 0
                    ELSE 1
                END,

                IDNguong

            LIMIT 1
            """;

        List<Dictionary<string, object?>> rows;

        var parameters =
            new[]
            {
                (
                    "@indicatorId",
                    (object?)indicatorId
                ),
                (
                    "@gender",
                    (object?)normalizedGender
                ),
                (
                    "@age",
                    (object?)age
                )
            };

        if (connection is null)
        {
            rows =
                await QueryAsync(
                    sql,
                    cancellationToken,
                    parameters
                );
        }
        else
        {
            rows =
                await QueryAsync(
                    sql,
                    cancellationToken,
                    connection,
                    transaction,
                    parameters
                );
        }

        if (rows.Count == 0)
        {
            return null;
        }

        return new IndicatorThreshold(
            GetDecimal(
                rows[0],
                "GiaTriMin"
            ),

            GetDecimal(
                rows[0],
                "GiaTriMax"
            ),

            GetString(
                rows[0],
                "GiaTriTextBinhThuong"
            ),

            GetString(
                rows[0],
                "GhiChu"
            )
        );
    }

    // =====================================================
    // EVALUATION
    // =====================================================

    private static string EvaluateNumeric(
        decimal value,
        IndicatorThreshold? threshold,
        bool abnormalOverride
    )
    {
        if (abnormalOverride)
        {
            return "bat_thuong";
        }

        if (threshold is null)
        {
            return "chua_danh_gia";
        }

        if (
            threshold.Min.HasValue
            &&
            value <
            threshold.Min.Value
        )
        {
            return "thap";
        }

        if (
            threshold.Max.HasValue
            &&
            value >
            threshold.Max.Value
        )
        {
            return "cao";
        }

        if (
            threshold.Min.HasValue
            ||
            threshold.Max.HasValue
        )
        {
            return "binh_thuong";
        }

        return "chua_danh_gia";
    }

    private static string EvaluateText(
        string value,
        IndicatorThreshold? threshold,
        bool abnormalOverride
    )
    {
        if (abnormalOverride)
        {
            return "bat_thuong";
        }

        if (
            threshold is null
            ||
            string.IsNullOrWhiteSpace(
                threshold.NormalText
            )
        )
        {
            return "chua_danh_gia";
        }

        return string.Equals(
            value.Trim(),
            threshold.NormalText.Trim(),
            StringComparison.OrdinalIgnoreCase
        )
            ? "binh_thuong"
            : "bat_thuong";
    }

    // =====================================================
    // SYNC AFTER SUBMIT
    // =====================================================

    private async Task SyncOrderAfterSubmitAsync(
        string orderId,
        long? visitId,
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        var unfinished =
            Convert.ToInt32(
                await ScalarAsync(
                    """
                    SELECT COUNT(*)
                    FROM ctphieuxetnghiem
                    WHERE
                        IDPhieuXetNghiem = @orderId
                        AND Status = 'yes'
                        AND TrangThai NOT IN
                        (
                            'cho_duyet',
                            'da_duyet',
                            'hoan_tat'
                        )
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@orderId", orderId)
                )
                ?? 0
            );

        if (unfinished == 0)
        {
            await ExecuteAsync(
                """
                UPDATE phieuxetnghiem
                SET TrangThai = 'cho_duyet'
                WHERE IDPhieuXetNghiem = @orderId
                """,
                cancellationToken,
                connection,
                transaction,
                ("@orderId", orderId)
            );

            if (visitId.HasValue)
            {
                await ExecuteAsync(
                    """
                    UPDATE luotxetnghiem
                    SET TrangThai = 'cho_duyet_kq'
                    WHERE IDLuotXetNghiem = @visitId
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@visitId", visitId.Value)
                );
            }
        }
        else
        {
            await ExecuteAsync(
                """
                UPDATE phieuxetnghiem
                SET TrangThai = 'dang_xet_nghiem'
                WHERE IDPhieuXetNghiem = @orderId
                """,
                cancellationToken,
                connection,
                transaction,
                ("@orderId", orderId)
            );

            if (visitId.HasValue)
            {
                await ExecuteAsync(
                    """
                    UPDATE luotxetnghiem
                    SET TrangThai = 'dang_xet_nghiem'
                    WHERE IDLuotXetNghiem = @visitId
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@visitId", visitId.Value)
                );
            }
        }
    }

    // =====================================================
    // RESULT ID
    // =====================================================

    private async Task<string> GenerateResultIdAsync(
        DbConnection connection,
        DbTransaction transaction,
        CancellationToken cancellationToken
    )
    {
        for (
            var index = 0;
            index < 10;
            index++
        )
        {
            var id =
                "KQ"
                +
                Guid.NewGuid()
                    .ToString("N")[..12]
                    .ToUpperInvariant();

            var exists =
                await ScalarAsync(
                    """
                    SELECT IDKetQua
                    FROM ketquaxetnghiem
                    WHERE IDKetQua = @id
                    LIMIT 1
                    """,
                    cancellationToken,
                    connection,
                    transaction,
                    ("@id", id)
                );

            if (exists is null)
            {
                return id;
            }
        }

        throw new BadRequestException(
            "Không thể sinh mã kết quả duy nhất."
        );
    }

    // =====================================================
    // NOTIFICATION
    // =====================================================

    private async Task InsertResultNotificationAsync(
        DbConnection connection,
        DbTransaction transaction,
        string customerId,
        string resultId,
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

        if (userId is null)
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
                'ket_qua',
                'Đã có kết quả xét nghiệm',
                'Kết quả xét nghiệm của bạn đã được bác sĩ duyệt.',
                'ketquaxetnghiem',
                @resultId,
                0,
                @now
            )
            """,
            cancellationToken,
            connection,
            transaction,
            (
                "@userId",
                Convert.ToInt32(userId)
            ),
            ("@resultId", resultId),
            ("@now", VietnamTime.Now)
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
                @now,
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
            ("@now", VietnamTime.Now),
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
    // MAPPER
    // =====================================================

    private static ResultIndicatorResponse MapIndicator(
        IReadOnlyDictionary<string, object?> row
    )
    {
        var dataType =
            GetString(
                row,
                "KieuDuLieu"
            )
            ?? "number";

        var numeric =
            GetDecimal(
                row,
                "GiaTriSo"
            );

        var text =
            GetString(
                row,
                "GiaTriText"
            );

        return new ResultIndicatorResponse
        {
            IndicatorId =
                GetString(
                    row,
                    "IDChiSo"
                )!,

            Name =
                GetString(
                    row,
                    "TenChiSo"
                )!,

            Unit =
                GetString(
                    row,
                    "DonVi"
                ),

            DataType =
                dataType,

            Value =
                dataType == "number"
                    ? numeric?.ToString(
                          "0.####",
                          CultureInfo.InvariantCulture
                      )
                      ?? string.Empty
                    : text
                      ?? string.Empty,

            NumericValue =
                numeric,

            TextValue =
                text,

            Min =
                GetDecimal(
                    row,
                    "NguongMinApDung"
                ),

            Max =
                GetDecimal(
                    row,
                    "NguongMaxApDung"
                ),

            Reference =
                GetString(
                    row,
                    "GiaTriThamChieu"
                ),

            Evaluation =
                GetString(
                    row,
                    "DanhGia"
                )
                ?? "chua_danh_gia",

            Note =
                GetString(
                    row,
                    "GhiChu"
                )
        };
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private static string BuildReference(
        IndicatorThreshold? threshold
    )
    {
        if (threshold is null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(threshold.NormalText))
        {
            return threshold.NormalText!;
        }

        if (
            threshold.Min.HasValue
            &&
            threshold.Max.HasValue
        )
        {
            return
                $"{threshold.Min.Value:0.####} - {threshold.Max.Value:0.####}";
        }

        if (threshold.Min.HasValue)
        {
            return
                $">= {threshold.Min.Value:0.####}";
        }

        if (threshold.Max.HasValue)
        {
            return
                $"<= {threshold.Max.Value:0.####}";
        }

        return threshold.Note
               ?? string.Empty;
    }

    private static bool TryParseDecimal(
        string value,
        out decimal result
    )
    {
        var text =
            value.Trim();

        if (
            decimal.TryParse(
                text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out result
            )
        )
        {
            return true;
        }

        if (
            decimal.TryParse(
                text,
                NumberStyles.Number,
                new CultureInfo("vi-VN"),
                out result
            )
        )
        {
            return true;
        }

        // 12,6 -> 12.6
        if (
            text.Contains(',')
            &&
            !text.Contains('.')
        )
        {
            return decimal.TryParse(
                text.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out result
            );
        }

        return false;
    }

    private static int? CalculateAge(
        DateTime? birthDate,
        DateTime date
    )
    {
        if (!birthDate.HasValue)
        {
            return null;
        }

        var birth =
            birthDate.Value.Date;

        var age =
            date.Year
            - birth.Year;

        if (
            birth >
            date.AddYears(-age)
        )
        {
            age--;
        }

        return Math.Max(
            age,
            0
        );
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
            "nữ" => "nu",
            _ => "tatca"
        };
    }

    private static string MapResultStatusToApi(
        string status
    )
    {
        return status switch
        {
            "dang_thuc_hien" =>
                "IN_PROGRESS",

            "cho_duyet" =>
                "PENDING_APPROVAL",

            "da_duyet" =>
                "APPROVED",

            "can_lam_lai" =>
                "NEEDS_RERUN",

            "hoan_tat" =>
                "COMPLETED",

            _ =>
                status.ToUpperInvariant()
        };
    }

    private static string MapWorklistStatusToApi(
        string status
    )
    {
        return status switch
        {
            "queue" =>
                "PENDING",

            "running" =>
                "IN_PROGRESS",

            "to_result" =>
                "COMPLETED",

            "rerun" =>
                "RERUN",

            "finished" =>
                "RESULT_ENTERED",

            "cancelled" =>
                "CANCELLED",

            _ =>
                status.ToUpperInvariant()
        };
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
    // RAW DATABASE
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

        var close =
            existingConnection is null
            &&
            connection.State !=
            ConnectionState.Open;

        if (close)
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
                        reader.IsDBNull(index)
                            ? null
                            : reader.GetValue(index);
                }

                result.Add(row);
            }

            return result;
        }
        finally
        {
            if (close)
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

        command.CommandText =
            sql;

        command.Transaction =
            transaction;

        AddParameters(
            command,
            parameters
        );

        return await command.ExecuteNonQueryAsync(
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

        command.CommandText =
            sql;

        command.Transaction =
            transaction;

        AddParameters(
            command,
            parameters
        );

        return await command.ExecuteScalarAsync(
            cancellationToken
        );
    }

    private static void AddParameters(
        DbCommand command,
        IEnumerable<
            (string Name, object? Value)
        > parameters
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
}