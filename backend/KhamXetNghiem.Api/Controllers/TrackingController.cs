using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using KhamXetNghiem.Api.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api/tracking")]
[Authorize]
public sealed class TrackingController : ControllerBase
{
    private readonly AppDbContext _context;

    public TrackingController(
        AppDbContext context
    )
    {
        _context = context;
    }

    // =========================================================
    // 1. THEO DÕI LƯỢT KHÁM
    //
    // GET /api/tracking/visits/{id}
    //
    // id có thể là:
    // - IDLuotKham
    // - MaDatLich
    // =========================================================

    [HttpGet("visits/{id}")]
    public async Task<IActionResult> GetVisitTracking(
        string id,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(
                new
                {
                    success = false,
                    message = "Thiếu mã lượt khám."
                }
            );
        }

        await using var connection =
            _context.Database.GetDbConnection();

        await EnsureOpenAsync(
            connection,
            cancellationToken
        );

        const string sql =
            """
            SELECT
                lk.IDLuotKham,
                lk.IDDatLichKham,
                lk.IDKhachHang,
                lk.IDBacSi,
                lk.IDPhong,
                lk.SoThuTu,
                lk.ThoiGianTiepNhan,
                lk.ThoiGianBatDau,
                lk.ThoiGianKetThuc,
                lk.TrangThai AS TrangThaiLuotKham,

                dl.MaDatLich,
                dl.NgayKham,
                dl.GioKham,
                dl.TrangThai AS TrangThaiDatLich,
                dl.GhiChu,
                dl.CreatedAt,

                kh.TenKhachHang,
                kh.NgaySinh,
                kh.GioiTinh,
                kh.SoDienThoai,

                bs.TenBacSi,

                p.TenPhong

            FROM luotkham lk

            INNER JOIN datlichkham dl
                ON dl.IDDatLichKham = lk.IDDatLichKham

            INNER JOIN khachhang kh
                ON kh.IDKhachHang = lk.IDKhachHang

            LEFT JOIN bacsi bs
                ON bs.IDBacSi = lk.IDBacSi

            LEFT JOIN phong p
                ON p.IDPhong = lk.IDPhong

            WHERE
                CAST(lk.IDLuotKham AS CHAR) = @id
                OR dl.MaDatLich = @id

            LIMIT 1;
            """;

        var visit =
            await QuerySingleAsync(
                connection,
                sql,
                cancellationToken,
                ("@id", id.Trim())
            );

        if (visit is null)
        {
            return NotFound(
                new
                {
                    success = false,
                    message = "Không tìm thấy lượt khám."
                }
            );
        }

        var customerId =
            GetString(
                visit,
                "IDKhachHang"
            );

        if (
            !await CanAccessCustomerAsync(
                connection,
                customerId,
                cancellationToken
            )
        )
        {
            return Forbid();
        }

        var visitId =
            GetString(
                visit,
                "IDLuotKham"
            );

        var appointmentDbId =
            GetString(
                visit,
                "IDDatLichKham"
            );

        var timeline =
            new List<TrackingTimelineItem>();

        // =====================================================
        // ĐẶT LỊCH
        // =====================================================

        AddTimeline(
            timeline,
            visit["CreatedAt"],
            "BOOKED",
            "Đặt lịch khám thành công",
            null,
            "system"
        );

        // =====================================================
        // TRUY VẾT LỊCH KHÁM
        // =====================================================

        timeline.AddRange(
            await GetTrackingEventsAsync(
                connection,
                customerId,
                "datlichkham",
                appointmentDbId,
                cancellationToken
            )
        );

        // =====================================================
        // TRUY VẾT LƯỢT KHÁM
        // =====================================================

        timeline.AddRange(
            await GetTrackingEventsAsync(
                connection,
                customerId,
                "luotkham",
                visitId,
                cancellationToken
            )
        );

        // =====================================================
        // CHECK-IN
        // =====================================================

        AddTimeline(
            timeline,
            visit["ThoiGianTiepNhan"],
            "CHECKED_IN",
            "Check-in tại quầy",
            null,
            "system"
        );

        // =====================================================
        // BẮT ĐẦU KHÁM
        // =====================================================

        AddTimeline(
            timeline,
            visit["ThoiGianBatDau"],
            "EXAM_STARTED",
            "Bác sĩ bắt đầu khám",
            null,
            "system"
        );

        // =====================================================
        // HOÀN TẤT
        // =====================================================

        AddTimeline(
            timeline,
            visit["ThoiGianKetThuc"],
            "EXAM_COMPLETED",
            "Hoàn tất khám",
            null,
            "system"
        );

        timeline =
            NormalizeTimeline(
                timeline
            );

        return Ok(
            new
            {
                success = true,

                id =
                    visitId,

                appointmentId =
                    GetString(
                        visit,
                        "MaDatLich"
                    ),

                type =
                    "EXAMINATION",

                patientCode =
                    customerId,

                customerName =
                    GetString(
                        visit,
                        "TenKhachHang"
                    ),

                date =
                    visit["NgayKham"],

                time =
                    visit["GioKham"],

                doctorId =
                    visit["IDBacSi"],

                doctorName =
                    GetString(
                        visit,
                        "TenBacSi"
                    ),

                roomId =
                    GetString(
                        visit,
                        "IDPhong"
                    ),

                roomName =
                    GetString(
                        visit,
                        "TenPhong"
                    ),

                queueNumber =
                    visit["SoThuTu"],

                status =
                    GetString(
                        visit,
                        "TrangThaiLuotKham"
                    ),

                appointmentStatus =
                    GetString(
                        visit,
                        "TrangThaiDatLich"
                    ),

                note =
                    GetString(
                        visit,
                        "GhiChu"
                    ),

                receivedAt =
                    visit["ThoiGianTiepNhan"],

                startedAt =
                    visit["ThoiGianBatDau"],

                completedAt =
                    visit["ThoiGianKetThuc"],

                timeline
            }
        );
    }

    // =========================================================
    // 2. THEO DÕI LƯỢT XÉT NGHIỆM
    //
    // GET /api/tracking/tests/{id}
    //
    // id có thể là:
    // - IDLuotXetNghiem
    // - MaDatLich
    // =========================================================

    [HttpGet("tests/{id}")]
    public async Task<IActionResult> GetTestTracking(
        string id,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(
                new
                {
                    success = false,
                    message = "Thiếu mã lượt xét nghiệm."
                }
            );
        }

        await using var connection =
            _context.Database.GetDbConnection();

        await EnsureOpenAsync(
            connection,
            cancellationToken
        );

        const string sql =
            """
            SELECT
                lx.IDLuotXetNghiem,
                lx.IDDatLichXN,
                lx.IDKhachHang,
                lx.IDPhong,
                lx.SoThuTu,
                lx.ThoiGianTiepNhan,
                lx.ThoiGianBatDau,
                lx.ThoiGianKetThuc,
                lx.TrangThai AS TrangThaiLuotXN,

                dl.MaDatLich,
                dl.IDBacSi,
                dl.NgayXetNghiem,
                dl.GioXetNghiem,
                dl.TrangThai AS TrangThaiDatLich,
                dl.GhiChu,
                dl.CreatedAt,

                kh.TenKhachHang,
                kh.NgaySinh,
                kh.GioiTinh,
                kh.SoDienThoai,

                bs.TenBacSi,

                p.TenPhong

            FROM luotxetnghiem lx

            INNER JOIN datlichxetnghiem dl
                ON dl.IDDatLichXN = lx.IDDatLichXN

            INNER JOIN khachhang kh
                ON kh.IDKhachHang = lx.IDKhachHang

            LEFT JOIN bacsi bs
                ON bs.IDBacSi = dl.IDBacSi

            LEFT JOIN phong p
                ON p.IDPhong = lx.IDPhong

            WHERE
                CAST(lx.IDLuotXetNghiem AS CHAR) = @id
                OR dl.MaDatLich = @id

            LIMIT 1;
            """;

        var visit =
            await QuerySingleAsync(
                connection,
                sql,
                cancellationToken,
                ("@id", id.Trim())
            );

        if (visit is null)
        {
            return NotFound(
                new
                {
                    success = false,
                    message = "Không tìm thấy lượt xét nghiệm."
                }
            );
        }

        var customerId =
            GetString(
                visit,
                "IDKhachHang"
            );

        if (
            !await CanAccessCustomerAsync(
                connection,
                customerId,
                cancellationToken
            )
        )
        {
            return Forbid();
        }

        var visitId =
            GetString(
                visit,
                "IDLuotXetNghiem"
            );

        var appointmentDbId =
            GetString(
                visit,
                "IDDatLichXN"
            );

        var timeline =
            new List<TrackingTimelineItem>();

        // =====================================================
        // ĐẶT LỊCH
        // =====================================================

        AddTimeline(
            timeline,
            visit["CreatedAt"],
            "BOOKED",
            "Đặt lịch xét nghiệm thành công",
            null,
            "system"
        );

        timeline.AddRange(
            await GetTrackingEventsAsync(
                connection,
                customerId,
                "datlichxetnghiem",
                appointmentDbId,
                cancellationToken
            )
        );

        timeline.AddRange(
            await GetTrackingEventsAsync(
                connection,
                customerId,
                "luotxetnghiem",
                visitId,
                cancellationToken
            )
        );

        // =====================================================
        // CHECK-IN
        // =====================================================

        AddTimeline(
            timeline,
            visit["ThoiGianTiepNhan"],
            "CHECKED_IN",
            "Check-in tại quầy",
            null,
            "system"
        );

        // =====================================================
        // BẮT ĐẦU QUY TRÌNH
        // =====================================================

        AddTimeline(
            timeline,
            visit["ThoiGianBatDau"],
            "STARTED",
            "Bắt đầu quy trình xét nghiệm",
            null,
            "system"
        );

        // =====================================================
        // PHIẾU XÉT NGHIỆM
        // =====================================================

        const string orderSql =
            """
            SELECT
                px.IDPhieuXetNghiem,
                px.TrangThai,
                px.NgayTao,
                px.IDBacSiChiDinh,
                px.IDBacSiPhuTrach

            FROM phieuxetnghiem px

            WHERE px.IDLuotXetNghiem = @visitId

            ORDER BY
                px.NgayTao DESC;
            """;

        var orders =
            await QueryListAsync(
                connection,
                orderSql,
                cancellationToken,
                ("@visitId", visitId)
            );

        // =====================================================
        // MẪU + BÀN GIAO
        // =====================================================

        const string specimenSql =
            """
            SELECT
                m.IDMau,
                m.MaBarcode,
                m.LoaiMau,
                m.TrangThai,
                m.ThoiGianLayMau,
                m.GhiChu,

                bg.IDBanGiao,
                bg.IDKTVTiepNhan,
                bg.ThoiGianBanGiao,
                bg.ThoiGianTiepNhan
                    AS ThoiGianKTVTiepNhan,
                bg.TrangThai
                    AS TrangThaiBanGiao

            FROM phieuxetnghiem px

            INNER JOIN ctphieuxetnghiem ct
                ON ct.IDPhieuXetNghiem =
                   px.IDPhieuXetNghiem

            INNER JOIN maubenhpham m
                ON m.IDCTPhieu =
                   ct.IDCTPhieu

            LEFT JOIN bangiaomau bg
                ON bg.IDMau =
                   m.IDMau

            WHERE
                px.IDLuotXetNghiem =
                @visitId

            ORDER BY
                m.ThoiGianLayMau,
                bg.ThoiGianBanGiao;
            """;

        var specimens =
            await QueryListAsync(
                connection,
                specimenSql,
                cancellationToken,
                ("@visitId", visitId)
            );

        foreach (var specimen in specimens)
        {
            AddTimeline(
                timeline,
                specimen["ThoiGianLayMau"],
                "SPECIMEN_COLLECTED",
                "Đã lấy mẫu bệnh phẩm",
                BuildDescription(
                    "Mã mẫu",
                    GetString(
                        specimen,
                        "IDMau"
                    ),
                    "Barcode",
                    GetString(
                        specimen,
                        "MaBarcode"
                    )
                ),
                "user"
            );

            AddTimeline(
                timeline,
                specimen["ThoiGianBanGiao"],
                "SPECIMEN_HANDED_OVER",
                "Đã bàn giao mẫu cho kỹ thuật viên",
                !string.IsNullOrWhiteSpace(
                    GetString(
                        specimen,
                        "IDKTVTiepNhan"
                    )
                )
                    ? $"KTV dự kiến tiếp nhận: {GetString(specimen, "IDKTVTiepNhan")}"
                    : null,
                "user"
            );

            AddTimeline(
                timeline,
                specimen["ThoiGianKTVTiepNhan"],
                "SPECIMEN_RECEIVED",
                "Kỹ thuật viên đã tiếp nhận mẫu",
                !string.IsNullOrWhiteSpace(
                    GetString(
                        specimen,
                        "IDKTVTiepNhan"
                    )
                )
                    ? $"KTV: {GetString(specimen, "IDKTVTiepNhan")}"
                    : null,
                "user"
            );

            // Truy vết riêng mẫu.
            timeline.AddRange(
                await GetTrackingEventsAsync(
                    connection,
                    customerId,
                    "maubenhpham",
                    GetString(
                        specimen,
                        "IDMau"
                    ),
                    cancellationToken
                )
            );
        }

        // =====================================================
        // WORKLIST
        // =====================================================

        const string workSql =
            """
            SELECT
                w.id,
                w.IDCTPhieu,
                w.IDMau,
                w.IDKTV,
                w.Instrument,
                w.ReagentLot,
                w.Status,
                w.ReceivedAt,
                w.StartedAt,
                w.FinishedAt

            FROM worklist w

            INNER JOIN ctphieuxetnghiem ct
                ON ct.IDCTPhieu =
                   w.IDCTPhieu

            INNER JOIN phieuxetnghiem px
                ON px.IDPhieuXetNghiem =
                   ct.IDPhieuXetNghiem

            WHERE
                px.IDLuotXetNghiem =
                @visitId

            ORDER BY w.id;
            """;

        var works =
            await QueryListAsync(
                connection,
                workSql,
                cancellationToken,
                ("@visitId", visitId)
            );

        foreach (var work in works)
        {
            AddTimeline(
                timeline,
                work["ReceivedAt"],
                "WORKLIST_RECEIVED",
                "Mẫu đã vào danh sách thực hiện",
                !string.IsNullOrWhiteSpace(
                    GetString(
                        work,
                        "IDKTV"
                    )
                )
                    ? $"KTV: {GetString(work, "IDKTV")}"
                    : null,
                "system"
            );

            AddTimeline(
                timeline,
                work["StartedAt"],
                "TEST_IN_PROGRESS",
                "Kỹ thuật viên bắt đầu xét nghiệm",
                BuildDescription(
                    "Thiết bị",
                    GetString(
                        work,
                        "Instrument"
                    ),
                    "Lô hóa chất",
                    GetString(
                        work,
                        "ReagentLot"
                    )
                ),
                "user"
            );

            AddTimeline(
                timeline,
                work["FinishedAt"],
                "TEST_FINISHED",
                "Hoàn tất bước thực hiện xét nghiệm",
                null,
                "user"
            );
        }

        // =====================================================
        // KẾT QUẢ
        // =====================================================

        const string resultSql =
            """
            SELECT
                kq.IDKetQua,
                kq.TrangThai,
                kq.IDKTV,
                kq.ThoiGianBatDau,
                kq.ThoiGianHoanThanh,
                kq.ThoiGianNhap,
                kq.IDBacSiDuyet,
                kq.ThoiGianDuyet,
                kq.KetQuaTongQuat,
                kq.KetLuanBacSi,
                kq.GhiChu

            FROM ketquaxetnghiem kq

            INNER JOIN ctphieuxetnghiem ct
                ON ct.IDCTPhieu =
                   kq.IDCTPhieu

            INNER JOIN phieuxetnghiem px
                ON px.IDPhieuXetNghiem =
                   ct.IDPhieuXetNghiem

            WHERE
                px.IDLuotXetNghiem =
                @visitId

            ORDER BY
                kq.ThoiGianNhap,
                kq.IDKetQua;
            """;

        var results =
            await QueryListAsync(
                connection,
                resultSql,
                cancellationToken,
                ("@visitId", visitId)
            );

        foreach (var result in results)
        {
            AddTimeline(
                timeline,
                result["ThoiGianNhap"],
                "RESULT_ENTERED",
                "Kỹ thuật viên đã nhập kết quả",
                $"Mã kết quả: {GetString(result, "IDKetQua")}",
                "user"
            );

            AddTimeline(
                timeline,
                result["ThoiGianHoanThanh"],
                "RESULT_SUBMITTED",
                "Kết quả đã gửi chờ bác sĩ duyệt",
                null,
                "user"
            );

            AddTimeline(
                timeline,
                result["ThoiGianDuyet"],
                "RESULT_APPROVED",
                "Bác sĩ đã duyệt kết quả xét nghiệm",
                GetString(
                    result,
                    "KetLuanBacSi"
                ),
                "user"
            );

            timeline.AddRange(
                await GetTrackingEventsAsync(
                    connection,
                    customerId,
                    "ketquaxetnghiem",
                    GetString(
                        result,
                        "IDKetQua"
                    ),
                    cancellationToken
                )
            );
        }

        // =====================================================
        // HOÀN TẤT LƯỢT
        // =====================================================

        AddTimeline(
            timeline,
            visit["ThoiGianKetThuc"],
            "COMPLETED",
            "Hoàn tất quy trình xét nghiệm",
            null,
            "system"
        );

        timeline =
            NormalizeTimeline(
                timeline
            );

        return Ok(
            new
            {
                success = true,

                id =
                    visitId,

                appointmentId =
                    GetString(
                        visit,
                        "MaDatLich"
                    ),

                type =
                    "TEST",

                patientCode =
                    customerId,

                customerName =
                    GetString(
                        visit,
                        "TenKhachHang"
                    ),

                date =
                    visit["NgayXetNghiem"],

                time =
                    visit["GioXetNghiem"],

                doctorId =
                    visit["IDBacSi"],

                doctorName =
                    GetString(
                        visit,
                        "TenBacSi"
                    ),

                roomId =
                    GetString(
                        visit,
                        "IDPhong"
                    ),

                roomName =
                    GetString(
                        visit,
                        "TenPhong"
                    ),

                queueNumber =
                    visit["SoThuTu"],

                status =
                    GetString(
                        visit,
                        "TrangThaiLuotXN"
                    ),

                appointmentStatus =
                    GetString(
                        visit,
                        "TrangThaiDatLich"
                    ),

                note =
                    GetString(
                        visit,
                        "GhiChu"
                    ),

                receivedAt =
                    visit["ThoiGianTiepNhan"],

                startedAt =
                    visit["ThoiGianBatDau"],

                completedAt =
                    visit["ThoiGianKetThuc"],

                orders,

                specimens,

                results,

                timeline
            }
        );
    }

    // =========================================================
    // 3. LỊCH SỬ TRUY VẾT
    //
    // GET /api/tracking/history
    //
    // CUSTOMER:
    // - backend tự lấy IDKhachHang từ JWT
    //
    // STAFF:
    // - có thể dùng ?customerId=KHxxxx
    // =========================================================

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
        [FromQuery] string? customerId = null,
        [FromQuery] string? type = null,
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default
    )
    {
        await using var connection =
            _context.Database.GetDbConnection();

        await EnsureOpenAsync(
            connection,
            cancellationToken
        );

        var currentCustomerId =
            await GetCurrentCustomerIdAsync(
                connection,
                cancellationToken
            );

        // =====================================================
        // CUSTOMER chỉ được xem của chính mình
        // =====================================================

        if (User.IsInRole("CUSTOMER"))
        {
            if (
                string.IsNullOrWhiteSpace(
                    currentCustomerId
                )
            )
            {
                return Unauthorized(
                    new
                    {
                        success = false,
                        message =
                            "Tài khoản chưa liên kết với hồ sơ khách hàng."
                    }
                );
            }

            customerId =
                currentCustomerId;
        }

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return BadRequest(
                new
                {
                    success = false,
                    message = "Thiếu mã khách hàng."
                }
            );
        }

        if (
            !await CanAccessCustomerAsync(
                connection,
                customerId,
                cancellationToken
            )
        )
        {
            return Forbid();
        }

        limit =
            Math.Clamp(
                limit,
                1,
                500
            );

        var sql =
            """
            SELECT
                tv.IDTruyVet
                    AS id,

                tv.IDKhachHang
                    AS patientCode,

                tv.LoaiDoiTuong
                    AS objectType,

                tv.IDDoiTuong
                    AS objectId,

                tv.HanhDong
                    AS action,

                tv.TrangThaiCu
                    AS oldStatus,

                tv.TrangThaiMoi
                    AS newStatus,

                tv.UserIDThucHien
                    AS performedByUserId,

                tv.NguonThucHien
                    AS source,

                tv.ThoiGian
                    AS time,

                tv.MoTa
                    AS description,

                tv.IPAddress
                    AS ipAddress

            FROM truyvet tv

            WHERE
                tv.IDKhachHang =
                @customerId
            """;

        var parameters =
            new List<(string Name, object? Value)>
            {
                (
                    "@customerId",
                    customerId.Trim()
                )
            };

        if (!string.IsNullOrWhiteSpace(type))
        {
            sql +=
                """
                AND tv.LoaiDoiTuong = @type
                """;

            parameters.Add(
                (
                    "@type",
                    type.Trim()
                )
            );
        }

        sql +=
            """
            ORDER BY
                tv.ThoiGian DESC,
                tv.IDTruyVet DESC

            LIMIT @limit;
            """;

        parameters.Add(
            (
                "@limit",
                limit
            )
        );

        var rows =
            await QueryListAsync(
                connection,
                sql,
                cancellationToken,
                parameters.ToArray()
            );

        return Ok(
            new
            {
                success = true,

                patientCode =
                    customerId,

                count =
                    rows.Count,

                items =
                    rows
            }
        );
    }

    // =========================================================
    // 4. LỊCH SỬ CỦA MỘT ĐỐI TƯỢNG
    //
    // GET:
    // /api/tracking/object/luotkham/12
    // /api/tracking/object/maubenhpham/M001
    // =========================================================

    [HttpGet("object/{objectType}/{objectId}")]
    public async Task<IActionResult> GetObjectHistory(
        string objectType,
        string objectId,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(objectType)
            ||
            string.IsNullOrWhiteSpace(objectId)
        )
        {
            return BadRequest(
                new
                {
                    success = false,
                    message =
                        "Thiếu thông tin đối tượng truy vết."
                }
            );
        }

        await using var connection =
            _context.Database.GetDbConnection();

        await EnsureOpenAsync(
            connection,
            cancellationToken
        );

        const string ownerSql =
            """
            SELECT
                IDKhachHang

            FROM truyvet

            WHERE
                LoaiDoiTuong = @type
                AND IDDoiTuong = @id

            ORDER BY
                IDTruyVet

            LIMIT 1;
            """;

        var owner =
            await QuerySingleAsync(
                connection,
                ownerSql,
                cancellationToken,
                (
                    "@type",
                    objectType.Trim()
                ),
                (
                    "@id",
                    objectId.Trim()
                )
            );

        if (owner is null)
        {
            return NotFound(
                new
                {
                    success = false,
                    message =
                        "Không tìm thấy lịch sử truy vết."
                }
            );
        }

        var customerId =
            GetString(
                owner,
                "IDKhachHang"
            );

        if (
            !await CanAccessCustomerAsync(
                connection,
                customerId,
                cancellationToken
            )
        )
        {
            return Forbid();
        }

        const string sql =
            """
            SELECT
                IDTruyVet
                    AS id,

                IDKhachHang
                    AS patientCode,

                LoaiDoiTuong
                    AS objectType,

                IDDoiTuong
                    AS objectId,

                HanhDong
                    AS action,

                TrangThaiCu
                    AS oldStatus,

                TrangThaiMoi
                    AS newStatus,

                UserIDThucHien
                    AS performedByUserId,

                NguonThucHien
                    AS source,

                ThoiGian
                    AS time,

                MoTa
                    AS description,

                IPAddress
                    AS ipAddress

            FROM truyvet

            WHERE
                LoaiDoiTuong = @type
                AND IDDoiTuong = @id

            ORDER BY
                ThoiGian,
                IDTruyVet;
            """;

        var rows =
            await QueryListAsync(
                connection,
                sql,
                cancellationToken,
                (
                    "@type",
                    objectType.Trim()
                ),
                (
                    "@id",
                    objectId.Trim()
                )
            );

        return Ok(
            new
            {
                success = true,

                objectType =
                    objectType.Trim(),

                objectId =
                    objectId.Trim(),

                patientCode =
                    customerId,

                count =
                    rows.Count,

                items =
                    rows
            }
        );
    }

    // =========================================================
    // 5. DANH SÁCH LƯỢT KHÁM CỦA KHÁCH HÀNG
    //
    // GET /api/tracking/my-visits
    // =========================================================

    [Authorize(Roles = "CUSTOMER")]
    [HttpGet("my-visits")]
    public async Task<IActionResult> GetMyVisits(
        CancellationToken cancellationToken
    )
    {
        await using var connection =
            _context.Database.GetDbConnection();

        await EnsureOpenAsync(
            connection,
            cancellationToken
        );

        var customerId =
            await GetCurrentCustomerIdAsync(
                connection,
                cancellationToken
            );

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Unauthorized(
                new
                {
                    success = false,
                    message =
                        "Tài khoản chưa liên kết với hồ sơ khách hàng."
                }
            );
        }

        const string sql =
            """
            SELECT
                lk.IDLuotKham
                    AS id,

                dl.MaDatLich
                    AS appointmentId,

                dl.NgayKham
                    AS date,

                dl.GioKham
                    AS time,

                lk.SoThuTu
                    AS queueNumber,

                lk.TrangThai
                    AS status,

                bs.TenBacSi
                    AS doctorName,

                p.TenPhong
                    AS roomName,

                lk.ThoiGianTiepNhan
                    AS receivedAt,

                lk.ThoiGianBatDau
                    AS startedAt,

                lk.ThoiGianKetThuc
                    AS completedAt

            FROM luotkham lk

            INNER JOIN datlichkham dl
                ON dl.IDDatLichKham =
                   lk.IDDatLichKham

            LEFT JOIN bacsi bs
                ON bs.IDBacSi =
                   lk.IDBacSi

            LEFT JOIN phong p
                ON p.IDPhong =
                   lk.IDPhong

            WHERE
                lk.IDKhachHang =
                @customerId

            ORDER BY
                dl.NgayKham DESC,
                dl.GioKham DESC;
            """;

        var rows =
            await QueryListAsync(
                connection,
                sql,
                cancellationToken,
                (
                    "@customerId",
                    customerId
                )
            );

        return Ok(rows);
    }

    // =========================================================
    // 6. DANH SÁCH LƯỢT XÉT NGHIỆM CỦA KHÁCH HÀNG
    //
    // GET /api/tracking/my-tests
    // =========================================================

    [Authorize(Roles = "CUSTOMER")]
    [HttpGet("my-tests")]
    public async Task<IActionResult> GetMyTests(
        CancellationToken cancellationToken
    )
    {
        await using var connection =
            _context.Database.GetDbConnection();

        await EnsureOpenAsync(
            connection,
            cancellationToken
        );

        var customerId =
            await GetCurrentCustomerIdAsync(
                connection,
                cancellationToken
            );

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Unauthorized(
                new
                {
                    success = false,
                    message =
                        "Tài khoản chưa liên kết với hồ sơ khách hàng."
                }
            );
        }

        const string sql =
            """
            SELECT
                lx.IDLuotXetNghiem
                    AS id,

                dl.MaDatLich
                    AS appointmentId,

                dl.NgayXetNghiem
                    AS date,

                dl.GioXetNghiem
                    AS time,

                lx.SoThuTu
                    AS queueNumber,

                lx.TrangThai
                    AS status,

                bs.TenBacSi
                    AS doctorName,

                p.TenPhong
                    AS roomName,

                lx.ThoiGianTiepNhan
                    AS receivedAt,

                lx.ThoiGianBatDau
                    AS startedAt,

                lx.ThoiGianKetThuc
                    AS completedAt

            FROM luotxetnghiem lx

            INNER JOIN datlichxetnghiem dl
                ON dl.IDDatLichXN =
                   lx.IDDatLichXN

            LEFT JOIN bacsi bs
                ON bs.IDBacSi =
                   dl.IDBacSi

            LEFT JOIN phong p
                ON p.IDPhong =
                   lx.IDPhong

            WHERE
                lx.IDKhachHang =
                @customerId

            ORDER BY
                dl.NgayXetNghiem DESC,
                dl.GioXetNghiem DESC;
            """;

        var rows =
            await QueryListAsync(
                connection,
                sql,
                cancellationToken,
                (
                    "@customerId",
                    customerId
                )
            );

        return Ok(rows);
    }

    // =========================================================
    // SECURITY
    // =========================================================

    private async Task<bool> CanAccessCustomerAsync(
        DbConnection connection,
        string customerId,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return false;
        }

        // =====================================================
        // NHÂN VIÊN NỘI BỘ
        // =====================================================

        if (
            User.IsInRole("ADMIN")
            ||
            User.IsInRole("DOCTOR")
            ||
            User.IsInRole("RECEPTIONIST")
            ||
            User.IsInRole("TECHNICIAN")
        )
        {
            return true;
        }

        // =====================================================
        // CUSTOMER
        // =====================================================

        if (!User.IsInRole("CUSTOMER"))
        {
            return false;
        }

        var currentCustomerId =
            await GetCurrentCustomerIdAsync(
                connection,
                cancellationToken
            );

        return
            !string.IsNullOrWhiteSpace(
                currentCustomerId
            )
            &&
            string.Equals(
                currentCustomerId,
                customerId,
                StringComparison.OrdinalIgnoreCase
            );
    }

    // =========================================================
    // CUSTOMER ID TỪ JWT
    // =========================================================

    private async Task<string?> GetCurrentCustomerIdAsync(
        DbConnection connection,
        CancellationToken cancellationToken
    )
    {
        // =====================================================
        // ƯU TIÊN claim customerId nếu JWT có sẵn
        // =====================================================

        var customerClaim =
            User.FindFirstValue(
                "customerId"
            )
            ??
            User.FindFirstValue(
                "IDKhachHang"
            );

        if (!string.IsNullOrWhiteSpace(customerClaim))
        {
            return customerClaim.Trim();
        }

        // =====================================================
        // Nếu có userId claim thì dùng trực tiếp
        // =====================================================

        var userIdClaim =
            User.FindFirstValue(
                "userId"
            )
            ??
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        if (
            int.TryParse(
                userIdClaim,
                out var userId
            )
        )
        {
            const string userIdSql =
                """
                SELECT IDKhachHang
                FROM users
                WHERE
                    UserID = @userId
                    AND IsActive = 1
                LIMIT 1;
                """;

            var userRow =
                await QuerySingleAsync(
                    connection,
                    userIdSql,
                    cancellationToken,
                    ("@userId", userId)
                );

            if (userRow is not null)
            {
                var customerId =
                    GetString(
                        userRow,
                        "IDKhachHang"
                    );

                if (!string.IsNullOrWhiteSpace(customerId))
                {
                    return customerId;
                }
            }
        }

        // =====================================================
        // JWT của project hiện tại dùng subject/email.
        // =====================================================

        var login =
            User.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            )
            ??
            User.FindFirstValue(
                ClaimTypes.Email
            )
            ??
            User.FindFirstValue(
                ClaimTypes.Name
            )
            ??
            User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(login))
        {
            return null;
        }

        const string sql =
            """
            SELECT
                IDKhachHang

            FROM users

            WHERE
                IsActive = 1
                AND
                (
                    Email = @login
                    OR Username = @login
                )

            LIMIT 1;
            """;

        var row =
            await QuerySingleAsync(
                connection,
                sql,
                cancellationToken,
                ("@login", login.Trim())
            );

        if (row is null)
        {
            return null;
        }

        var result =
            GetString(
                row,
                "IDKhachHang"
            );

        return string.IsNullOrWhiteSpace(result)
            ? null
            : result;
    }

    // =========================================================
    // TRACKING EVENTS
    // =========================================================

    private async Task<List<TrackingTimelineItem>>
        GetTrackingEventsAsync(
            DbConnection connection,
            string customerId,
            string objectType,
            string objectId,
            CancellationToken cancellationToken
        )
    {
        if (
            string.IsNullOrWhiteSpace(customerId)
            ||
            string.IsNullOrWhiteSpace(objectType)
            ||
            string.IsNullOrWhiteSpace(objectId)
        )
        {
            return [];
        }

        const string sql =
            """
            SELECT
                IDTruyVet,
                HanhDong,
                TrangThaiCu,
                TrangThaiMoi,
                UserIDThucHien,
                NguonThucHien,
                ThoiGian,
                MoTa

            FROM truyvet

            WHERE
                IDKhachHang = @customerId
                AND LoaiDoiTuong = @objectType
                AND IDDoiTuong = @objectId

            ORDER BY
                ThoiGian,
                IDTruyVet;
            """;

        var rows =
            await QueryListAsync(
                connection,
                sql,
                cancellationToken,
                (
                    "@customerId",
                    customerId
                ),
                (
                    "@objectType",
                    objectType
                ),
                (
                    "@objectId",
                    objectId
                )
            );

        return rows
            .Select(
                row =>
                    new TrackingTimelineItem
                    {
                        Time =
                            ToDateTime(
                                row["ThoiGian"]
                            ),

                        Code =
                            BuildEventCode(
                                GetString(
                                    row,
                                    "HanhDong"
                                )
                            ),

                        Event =
                            GetString(
                                row,
                                "HanhDong"
                            ),

                        Description =
                            GetString(
                                row,
                                "MoTa"
                            ),

                        OldStatus =
                            GetString(
                                row,
                                "TrangThaiCu"
                            ),

                        NewStatus =
                            GetString(
                                row,
                                "TrangThaiMoi"
                            ),

                        Source =
                            GetString(
                                row,
                                "NguonThucHien"
                            ),

                        UserId =
                            ToNullableInt(
                                row["UserIDThucHien"]
                            )
                    }
            )
            .Where(
                item =>
                    item.Time.HasValue
            )
            .ToList();
    }

    // =========================================================
    // EVENT CODE
    // =========================================================

    private static string BuildEventCode(
        string? action
    )
    {
        var value =
            (
                action
                ?? string.Empty
            )
            .Trim()
            .ToLowerInvariant();

        if (
            value.Contains(
                "thay đổi lịch"
            )
            ||
            value.Contains(
                "đổi lịch"
            )
        )
        {
            return "APPOINTMENT_RESCHEDULED";
        }

        if (
            value.Contains(
                "hủy lịch"
            )
        )
        {
            return "APPOINTMENT_CANCELLED";
        }

        if (
            value.Contains(
                "check-in"
            )
            ||
            value.Contains(
                "check in"
            )
            ||
            value.Contains(
                "tiếp nhận"
            )
            &&
            !value.Contains(
                "mẫu"
            )
        )
        {
            return "CHECKED_IN";
        }

        if (
            value.Contains("gọi")
            &&
            (
                value.Contains("phòng")
                ||
                value.Contains("bệnh nhân")
            )
        )
        {
            return "CALLED";
        }

        if (
            value.Contains(
                "vắng mặt"
            )
            ||
            value.Contains(
                "bỏ lượt"
            )
            ||
            value.Contains(
                "gọi lại"
            )
        )
        {
            return "SKIPPED";
        }

        if (
            value.Contains(
                "bắt đầu khám"
            )
        )
        {
            return "EXAM_STARTED";
        }

        if (
            value.Contains(
                "hoàn tất khám"
            )
        )
        {
            return "EXAM_COMPLETED";
        }

        if (
            value.Contains(
                "lấy mẫu"
            )
            &&
            !value.Contains(
                "bàn giao"
            )
        )
        {
            return "SPECIMEN_COLLECTED";
        }

        if (
            value.Contains(
                "bàn giao mẫu"
            )
        )
        {
            return "SPECIMEN_HANDED_OVER";
        }

        if (
            value.Contains(
                "tiếp nhận mẫu"
            )
        )
        {
            return "SPECIMEN_RECEIVED";
        }

        if (
            value.Contains(
                "bắt đầu xét nghiệm"
            )
            ||
            value.Contains(
                "bắt đầu thực hiện xét nghiệm"
            )
        )
        {
            return "TEST_IN_PROGRESS";
        }

        if (
            value.Contains(
                "lưu nháp kết quả"
            )
        )
        {
            return "RESULT_ENTERED";
        }

        if (
            value.Contains(
                "gửi kết quả"
            )
            ||
            value.Contains(
                "chờ bác sĩ duyệt"
            )
        )
        {
            return "RESULT_SUBMITTED";
        }

        if (
            value.Contains(
                "duyệt kết quả"
            )
        )
        {
            return "RESULT_APPROVED";
        }

        if (
            value.Contains(
                "đã có kết quả"
            )
        )
        {
            return "RESULT_AVAILABLE";
        }

        if (
            value.Contains(
                "hoàn tất"
            )
        )
        {
            return "COMPLETED";
        }

        return "TRACKING_EVENT";
    }

    // =========================================================
    // ADD TIMELINE
    // =========================================================

    private static void AddTimeline(
        ICollection<TrackingTimelineItem> list,
        object? time,
        string code,
        string eventName,
        string? description,
        string? source
    )
    {
        var date =
            ToDateTime(
                time
            );

        if (!date.HasValue)
        {
            return;
        }

        list.Add(
            new TrackingTimelineItem
            {
                Time =
                    date,

                Code =
                    code,

                Event =
                    eventName,

                Description =
                    description,

                Source =
                    source
            }
        );
    }

    // =========================================================
    // NORMALIZE TIMELINE
    // =========================================================

    private static List<TrackingTimelineItem>
        NormalizeTimeline(
            IEnumerable<TrackingTimelineItem> source
        )
    {
        return source
            .Where(
                x =>
                    x.Time.HasValue
            )
            .GroupBy(
                x => new
                {
                    Time =
                        x.Time,

                    Event =
                        (
                            x.Event
                            ?? string.Empty
                        )
                        .Trim()
                        .ToLowerInvariant(),

                    OldStatus =
                        (
                            x.OldStatus
                            ?? string.Empty
                        )
                        .Trim()
                        .ToLowerInvariant(),

                    NewStatus =
                        (
                            x.NewStatus
                            ?? string.Empty
                        )
                        .Trim()
                        .ToLowerInvariant()
                }
            )
            .Select(
                group =>
                    group.First()
            )
            .OrderBy(
                x => x.Time
            )
            .ThenBy(
                x => x.Event
            )
            .ToList();
    }

    // =========================================================
    // DB CONNECTION
    // =========================================================

    private static async Task EnsureOpenAsync(
        DbConnection connection,
        CancellationToken cancellationToken
    )
    {
        if (
            connection.State
            != ConnectionState.Open
        )
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }
    }

    // =========================================================
    // QUERY SINGLE
    // =========================================================

    private static async Task<Dictionary<string, object?>?>
        QuerySingleAsync(
            DbConnection connection,
            string sql,
            CancellationToken cancellationToken,
            params (
                string Name,
                object? Value
            )[] parameters
        )
    {
        var rows =
            await QueryListAsync(
                connection,
                sql,
                cancellationToken,
                parameters
            );

        return rows.FirstOrDefault();
    }

    // =========================================================
    // QUERY LIST
    // =========================================================

    private static async Task<List<Dictionary<string, object?>>>
        QueryListAsync(
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

        var result =
            new List<
                Dictionary<
                    string,
                    object?
                >
            >();

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
                    reader.GetName(
                        index
                    )
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

            result.Add(
                row
            );
        }

        return result;
    }

    // =========================================================
    // STRING
    // =========================================================

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

    // =========================================================
    // DATETIME
    // =========================================================

    private static DateTime? ToDateTime(
        object? value
    )
    {
        if (
            value is null
            ||
            value == DBNull.Value
        )
        {
            return null;
        }

        if (value is DateTime date)
        {
            return DateTime.SpecifyKind(
                date,
                DateTimeKind.Unspecified
            );
        }

        if (
            DateTime.TryParse(
                Convert.ToString(value),
                out var parsed
            )
        )
        {
            return DateTime.SpecifyKind(
                parsed,
                DateTimeKind.Unspecified
            );
        }

        return null;
    }

    // =========================================================
    // INT
    // =========================================================

    private static int? ToNullableInt(
        object? value
    )
    {
        if (
            value is null
            ||
            value == DBNull.Value
        )
        {
            return null;
        }

        return int.TryParse(
            Convert.ToString(value),
            out var result
        )
            ? result
            : null;
    }

    // =========================================================
    // DESCRIPTION
    // =========================================================

    private static string? BuildDescription(
        string key1,
        string? value1,
        string key2,
        string? value2
    )
    {
        var values =
            new List<string>();

        if (!string.IsNullOrWhiteSpace(value1))
        {
            values.Add(
                $"{key1}: {value1}"
            );
        }

        if (!string.IsNullOrWhiteSpace(value2))
        {
            values.Add(
                $"{key2}: {value2}"
            );
        }

        return values.Count == 0
            ? null
            : string.Join(
                " - ",
                values
            );
    }

    // =========================================================
    // INTERNAL DTO
    // =========================================================

    public sealed class TrackingTimelineItem
    {
        public DateTime? Time { get; set; }

        public string Code { get; set; } =
            string.Empty;

        public string Event { get; set; } =
            string.Empty;

        public string? Description { get; set; }

        public string? OldStatus { get; set; }

        public string? NewStatus { get; set; }

        public string? Source { get; set; }

        public int? UserId { get; set; }
    }
}