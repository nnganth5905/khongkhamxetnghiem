using System.Security.Claims;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/doctor/test-orders")]
[Authorize(Roles = "DOCTOR")]
public class DoctorTestOrderController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorTestOrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDoctorTestOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.VisitId))
        {
            return BadRequest(new
            {
                success = false,
                message = "Thiếu mã lượt khám."
            });
        }

        if (request.TestIds is null || request.TestIds.Count == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Vui lòng chọn ít nhất một xét nghiệm."
            });
        }

        var doctorId = await GetDoctorId();
        if (doctorId is null)
        {
            return Forbid();
        }

        var visit = await ResolveVisit(request.VisitId, doctorId.Value);

        if (visit is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy lượt khám thuộc bác sĩ hiện tại."
            });
        }

        Kham? examination = null;

        if (request.ExaminationId.HasValue)
        {
            examination = await _context.Khams
                .FirstOrDefaultAsync(x =>
                    x.Idkham == request.ExaminationId.Value &&
                    x.IdluotKham == visit.IdluotKham);
        }

        examination ??= await _context.Khams
            .FirstOrDefaultAsync(x => x.IdluotKham == visit.IdluotKham);

        if (examination is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Chưa có hồ sơ khám cho lượt khám này."
            });
        }

        // Tránh dùng Contains trong LINQ do project hiện chạy .NET 10 + EF Core 8.
        var activeTests = await _context.Loaixetnghiems
            .AsNoTracking()
            .Where(x => x.Status == "yes")
            .ToListAsync();

        var selectedIds = request.TestIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var selectedTests = activeTests
            .Where(x => selectedIds.Contains(x.IdxetNghiem))
            .ToList();

        if (selectedTests.Count != selectedIds.Count)
        {
            var found = selectedTests
                .Select(x => x.IdxetNghiem)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missing = selectedIds
                .Where(x => !found.Contains(x))
                .ToArray();

            return BadRequest(new
            {
                success = false,
                message = "Có xét nghiệm không tồn tại hoặc đã ngừng hoạt động.",
                missing
            });
        }

        await using var tx = await _context.Database.BeginTransactionAsync();

        var orderId = await GenerateOrderId();

        var noteParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.Diagnosis))
        {
            noteParts.Add($"Chẩn đoán sơ bộ: {request.Diagnosis.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            noteParts.Add(request.Notes.Trim());
        }

        var note = string.Join(" | ", noteParts);

        if (note.Length > 500)
        {
            note = note[..500];
        }

        var order = new Phieuxetnghiem
        {
            IdphieuXetNghiem = orderId,
            IdkhachHang = visit.IdkhachHang,
            Idkham = examination.Idkham,
            IdluotXetNghiem = null,
            IdbacSiChiDinh = doctorId.Value,
            IdbacSiPhuTrach = doctorId.Value,
            NgayTao = DateTime.Now,
            TongTien = selectedTests.Sum(x => x.Gia),
            TrangThaiThanhToan = "chua_thanh_toan",
            TrangThai = "cho_lay_mau",
            GhiChu = string.IsNullOrWhiteSpace(note) ? null : note
        };

        _context.Phieuxetnghiems.Add(order);

        foreach (var test in selectedTests)
        {
            _context.Ctphieuxetnghiems.Add(new Ctphieuxetnghiem
            {
                IdphieuXetNghiem = orderId,
                IdxetNghiem = test.IdxetNghiem,
                SoLuong = 1,
                DonGia = test.Gia,
                GhiChu = string.IsNullOrWhiteSpace(request.Notes)
                    ? null
                    : request.Notes.Trim(),
                TrangThai = "cho_lay_mau",
                Status = "yes"
            });
        }

        AddTracking(
            visit.IdkhachHang,
            "phieuxetnghiem",
            orderId,
            "Bác sĩ chỉ định xét nghiệm",
            $"Lượt khám: {visit.IdluotKham}; số xét nghiệm: {selectedTests.Count}"
        );

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return StatusCode(StatusCodes.Status201Created, new
        {
            success = true,
            orderId,
            idPhieuXetNghiem = orderId,
            visitId = visit.IdluotKham,
            examinationId = examination.Idkham,
            patientCode = visit.IdkhachHang,
            patientName = visit.IdkhachHangNavigation.TenKhachHang,
            total = order.TongTien,
            status = order.TrangThai,
            tests = selectedTests.Select(x => new
            {
                id = x.IdxetNghiem,
                name = x.TenXetNghiem,
                price = x.Gia,
                sampleType = x.LoaiMauMacDinh
            })
        });
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> Get(string orderId)
    {
        var doctorId = await GetDoctorId();

        if (doctorId is null)
        {
            return Forbid();
        }

        var order = await _context.Phieuxetnghiems
            .AsNoTracking()
            .Include(x => x.IdkhachHangNavigation)
            .Include(x => x.Ctphieuxetnghiems)
                .ThenInclude(x => x.IdxetNghiemNavigation)
            .FirstOrDefaultAsync(x =>
                x.IdphieuXetNghiem == orderId &&
                (x.IdbacSiChiDinh == doctorId.Value ||
                 x.IdbacSiPhuTrach == doctorId.Value));

        if (order is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy phiếu xét nghiệm."
            });
        }

        return Ok(new
        {
            success = true,
            orderId = order.IdphieuXetNghiem,
            patientCode = order.IdkhachHang,
            patientName = order.IdkhachHangNavigation.TenKhachHang,
            status = order.TrangThai,
            notes = order.GhiChu,
            defaultSampleType = order.Ctphieuxetnghiems
                .Select(x => x.IdxetNghiemNavigation.LoaiMauMacDinh)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
            tests = order.Ctphieuxetnghiems.Select(x => new
            {
                detailId = x.Idctphieu,
                id = x.IdxetNghiem,
                name = x.IdxetNghiemNavigation.TenXetNghiem,
                price = x.DonGia,
                sampleType = x.IdxetNghiemNavigation.LoaiMauMacDinh,
                status = x.TrangThai
            })
        });
    }

    [HttpPost("{orderId}/specimens")]
    public async Task<IActionResult> CollectSpecimen(
        string orderId,
        [FromBody] CollectDoctorOrderSpecimenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Barcode))
        {
            return BadRequest(new
            {
                success = false,
                message = "Thiếu barcode mẫu."
            });
        }

        var doctorId = await GetDoctorId();

        if (doctorId is null)
        {
            return Forbid();
        }

        var order = await _context.Phieuxetnghiems
            .Include(x => x.Ctphieuxetnghiems)
                .ThenInclude(x => x.IdxetNghiemNavigation)
            .FirstOrDefaultAsync(x =>
                x.IdphieuXetNghiem == orderId &&
                (x.IdbacSiChiDinh == doctorId.Value ||
                 x.IdbacSiPhuTrach == doctorId.Value));

        if (order is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy phiếu xét nghiệm."
            });
        }

        var detail = order.Ctphieuxetnghiems
            .OrderBy(x => x.Idctphieu)
            .FirstOrDefault();

        if (detail is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Phiếu xét nghiệm chưa có chi tiết xét nghiệm."
            });
        }

        var barcode = request.Barcode.Trim();
        var specimenId = "M" + barcode;

        var exists = await _context.Maubenhphams
            .AnyAsync(x =>
                x.Idmau == specimenId ||
                x.MaBarcode == barcode);

        if (exists)
        {
            return Conflict(new
            {
                success = false,
                message = "Barcode mẫu đã tồn tại."
            });
        }

        await using var tx = await _context.Database.BeginTransactionAsync();

        order.TrangThai = "da_lay_mau";

        foreach (var item in order.Ctphieuxetnghiems)
        {
            if (item.TrangThai == "cho_lay_mau")
            {
                item.TrangThai = "da_lay_mau";
            }
        }

        var sampleType = !string.IsNullOrWhiteSpace(request.SampleType)
            ? request.SampleType.Trim()
            : detail.IdxetNghiemNavigation.LoaiMauMacDinh ?? "Khác";

        var specimen = new Maubenhpham
        {
            Idmau = specimenId,
            Idctphieu = detail.Idctphieu,
            MaBarcode = barcode,
            LoaiMau = sampleType,
            IdbacSiLayMau = doctorId.Value,
            ThoiGianLayMau = DateTime.Now,
            TrangThai = "da_lay_mau",
            GhiChu = string.IsNullOrWhiteSpace(request.Notes)
                ? null
                : request.Notes.Trim()
        };

        _context.Maubenhphams.Add(specimen);

        AddTracking(
            order.IdkhachHang,
            "maubenhpham",
            specimenId,
            "Bác sĩ xác nhận đã lấy mẫu",
            $"Phiếu xét nghiệm: {orderId}; Barcode: {barcode}"
        );

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return StatusCode(StatusCodes.Status201Created, new
        {
            success = true,
            specimenId,
            id = specimenId,
            barcode,
            orderId,
            status = "COLLECTED",
            message = "Đã lấy mẫu thành công."
        });
    }

    private async Task<Luotkham?> ResolveVisit(string value, int doctorId)
    {
        var query = _context.Luotkhams
            .Include(x => x.IdkhachHangNavigation)
            .Include(x => x.IddatLichKhamNavigation)
            .Where(x => x.IdbacSi == doctorId);

        if (ulong.TryParse(value, out var numeric))
        {
            var byId = await query
                .FirstOrDefaultAsync(x => x.IdluotKham == numeric);

            if (byId is not null)
            {
                return byId;
            }
        }

        return await query
            .FirstOrDefaultAsync(x =>
                x.IddatLichKhamNavigation.MaDatLich == value);
    }

    private async Task<int?> GetDoctorId()
    {
        if (!int.TryParse(
            User.FindFirstValue("userId"),
            out var userId))
        {
            return null;
        }

        var account = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.IsActive == true);

        if (account is null)
        {
            return null;
        }

        if (account.IdbacSi.HasValue)
        {
            return account.IdbacSi.Value;
        }

        var employeeId = account.IdnhanVien;

        if (string.IsNullOrWhiteSpace(employeeId) &&
            !string.IsNullOrWhiteSpace(account.Email))
        {
            employeeId = await _context.Nhanviens
                .AsNoTracking()
                .Where(x => x.Email == account.Email)
                .Select(x => x.IdnhanVien)
                .FirstOrDefaultAsync();
        }

        if (string.IsNullOrWhiteSpace(employeeId))
        {
            return null;
        }

        return await _context.Bacsis
            .AsNoTracking()
            .Where(x => x.IdnhanVien == employeeId)
            .Select(x => (int?)x.IdbacSi)
            .FirstOrDefaultAsync();
    }

    private async Task<string> GenerateOrderId()
    {
        for (var i = 0; i < 10; i++)
        {
            var value = $"PXN{DateTime.Now:yyMMddHHmmssfff}";

            var exists = await _context.Phieuxetnghiems
                .AnyAsync(x => x.IdphieuXetNghiem == value);

            if (!exists)
            {
                return value;
            }

            await Task.Delay(2);
        }

        return $"PXN{Guid.NewGuid():N}"[..20].ToUpperInvariant();
    }

    private void AddTracking(
        string customerId,
        string objectType,
        string objectId,
        string action,
        string? description)
    {
        int? userId = int.TryParse(
            User.FindFirstValue("userId"),
            out var parsed)
                ? parsed
                : null;

        _context.Truyvets.Add(new Truyvet
        {
            IdkhachHang = customerId,
            LoaiDoiTuong = objectType,
            IddoiTuong = objectId,
            HanhDong = action,
            UserIdthucHien = userId,
            NguonThucHien = "user",
            ThoiGian = DateTime.Now,
            MoTa = description
        });
    }
}

public class CreateDoctorTestOrderRequest
{
    public string VisitId { get; set; } = "";
    public ulong? ExaminationId { get; set; }
    public string? Diagnosis { get; set; }
    public string? Notes { get; set; }
    public List<string> TestIds { get; set; } = new();
}

public class CollectDoctorOrderSpecimenRequest
{
    public string Barcode { get; set; } = "";
    public string? SampleType { get; set; }
    public string? Notes { get; set; }
}
