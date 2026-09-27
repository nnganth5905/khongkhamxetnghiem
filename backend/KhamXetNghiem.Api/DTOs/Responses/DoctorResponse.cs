namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class DoctorResponse
{
    public int Id { get; init; }

    public int IdBacSi => Id;

    public string HoTen { get; init; } = string.Empty;

    public string FullName => HoTen;

    public string IdChuyenKhoa { get; init; } = string.Empty;

    public string SpecialtyId => IdChuyenKhoa;

    public string? TenChuyenKhoa { get; init; }

    public string? SpecialtyName => TenChuyenKhoa;

    public string? CoSoId { get; init; }

    public string? FacilityId => CoSoId;

    public string? HocVi { get; init; }

    public string? Degree => HocVi;

    public string? ChucDanh { get; init; }

    public string? Title => ChucDanh;

    public string? SoDienThoai { get; init; }

    public string? Phone => SoDienThoai;

    public string? Email { get; init; }

    public string? HinhAnh { get; init; }

    public string? ImageUrl => HinhAnh;

    public string? GioiThieu { get; init; }

    public string? Bio => GioiThieu;

    public string TrangThai { get; init; } = "active";

    public string Status => TrangThai;

    public string? IdNhanVien { get; init; }

    public decimal SoSao { get; init; }

    public byte? NamKinhNghiem { get; init; }
}