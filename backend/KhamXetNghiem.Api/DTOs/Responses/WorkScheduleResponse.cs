namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class WorkScheduleResponse
{
    public long Id { get; init; }

    public long IdLichLamViec => Id;

    public long MaLich => Id;

    public int IdBacSi { get; init; }

    public string? TenBacSi { get; init; }

    // compatibility
    public string LoaiNhanSu => "DOCTOR";

    public string IdNhanSu =>
        IdBacSi.ToString();

    public string? TenNhanSu =>
        TenBacSi;

    public string NgayLamViec { get; init; }
        = string.Empty;

    public string CaLamViec { get; init; }
        = "CUSTOM";

    public string GioBatDau { get; init; }
        = string.Empty;

    public string GioKetThuc { get; init; }
        = string.Empty;

    public string? IdPhong { get; init; }

    public string? TenPhong { get; init; }

    public string TrangThai { get; init; }
        = "ACTIVE";

    public string DbTrangThai { get; init; }
        = "duoc_duyet";

    public string? GhiChu { get; init; }

    public string NguonTao { get; init; }
        = "admin";

    public int? NguoiTaoUserId { get; init; }

    public DateTime? CreatedAt { get; init; }
}