using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/doctor/examinations")]
[Authorize(Roles = "DOCTOR")]
public class ExaminationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ExaminationController(ApplicationDbContext context) => _context = context;

    [HttpGet("visit/{visitId}")]
    public async Task<IActionResult> GetByVisit(string visitId)
    {
        var visit = await ResolveVisit(visitId);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám." });
        if (visit.TrangThai is "da_tiep_nhan" or "da_den_luot" or "cho_goi_lai")
        {
            visit.TrangThai = "dang_kham";
            visit.ThoiGianBatDau ??= DateTime.Now;
            await _context.SaveChangesAsync();
        }
        var exam = await _context.Khams.AsNoTracking().FirstOrDefaultAsync(x => x.IdluotKham == visit.IdluotKham);
        var customer = await _context.Khachhangs.AsNoTracking().FirstAsync(x => x.IdkhachHang == visit.IdkhachHang);
        return Ok(new
        {
            visitId = visit.IdluotKham,
            IDKham = exam?.Idkham,
            TrieuChung = exam?.TrieuChung,
            TienSuBenh = exam?.TienSuBenh,
            ChanDoan = exam?.ChanDoan,
            KetLuan = exam?.KetLuan,
            HuongDieuTri = exam?.HuongDieuTri,
            TenKhachHang = customer.TenKhachHang,
            GioiTinh = customer.GioiTinh,
            NgaySinh = customer.NgaySinh,
            SoDienThoai = customer.SoDienThoai,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExaminationRequest request)
    {
        var visit = await ResolveVisit(request.VisitId);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám." });
        if (await _context.Khams.AnyAsync(x => x.IdluotKham == visit.IdluotKham))
            return Conflict(new { message = "Bệnh án đã tồn tại; hãy dùng chức năng cập nhật." });
        var exam = NewExam(visit, request);
        _context.Khams.Add(exam);
        await _context.SaveChangesAsync();
        return Ok(ToResponse(exam));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ExaminationRequest request)
    {
        var visit = await ResolveVisit(request.VisitId);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám." });
        Kham? exam = null;
        if (ulong.TryParse(id, out var examId)) exam = await _context.Khams.FirstOrDefaultAsync(x => x.Idkham == examId);
        exam ??= await _context.Khams.FirstOrDefaultAsync(x => x.IdluotKham == visit.IdluotKham);
        if (exam is null) { exam = NewExam(visit, request); _context.Khams.Add(exam); }
        else Apply(exam, request);
        await _context.SaveChangesAsync();
        return Ok(ToResponse(exam));
    }

    [HttpPost("{visitId}/complete")]
    public async Task<IActionResult> Complete(string visitId)
    {
        var visit = await ResolveVisit(visitId);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám." });
        visit.TrangThai = "hoan_tat";
        visit.ThoiGianKetThuc = DateTime.Now;
        _context.Truyvets.Add(new Truyvet { IdkhachHang = visit.IdkhachHang, LoaiDoiTuong = "luotkham", IddoiTuong = visit.IdluotKham.ToString(), HanhDong = "Bác sĩ hoàn tất khám", NguonThucHien = "user", ThoiGian = DateTime.Now });
        await _context.SaveChangesAsync();
        return Ok(new { id = visitId, status = "COMPLETED", message = "Đã hoàn tất phiên khám." });
    }

    private async Task<Luotkham?> ResolveVisit(string value)
    {
        if (ulong.TryParse(value, out var numeric))
        {
            var byId = await _context.Luotkhams.FirstOrDefaultAsync(x => x.IdluotKham == numeric);
            if (byId is not null) return byId;
        }
        return await _context.Luotkhams.Include(x => x.IddatLichKhamNavigation).FirstOrDefaultAsync(x => x.IddatLichKhamNavigation.MaDatLich == value);
    }
    private static Kham NewExam(Luotkham v, ExaminationRequest r) { var e = new Kham { IdluotKham = v.IdluotKham, IdbacSi = v.IdbacSi, ThoiGianKham = DateTime.Now }; Apply(e, r); return e; }
    private static void Apply(Kham e, ExaminationRequest r) { e.TrieuChung = r.Symptoms; e.TienSuBenh = r.History; e.ChanDoan = r.Diagnosis; e.KetLuan = r.Conclusion; e.HuongDieuTri = r.Advice; e.UpdatedAt = DateTime.Now; }
    private static object ToResponse(Kham e) => new { id = e.Idkham, visitId = e.IdluotKham, symptoms = e.TrieuChung, history = e.TienSuBenh, diagnosis = e.ChanDoan, conclusion = e.KetLuan, advice = e.HuongDieuTri };
}

public class ExaminationRequest
{
    public string VisitId { get; set; } = "";
    public string? Symptoms { get; set; }
    public string? History { get; set; }
    public string? Diagnosis { get; set; }
    public string? Conclusion { get; set; }
    public string? Advice { get; set; }
}
