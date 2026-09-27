namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class TechnicianResponse
{
    public string Id { get; init; } = string.Empty;

    public string IdNhanVien => Id;

    public string MaKTV => Id;

    public string HoTen { get; init; } = string.Empty;

    public string? IdCoSo { get; init; }

    public string? TenCoSo { get; init; }

    public string? SoDienThoai { get; init; }

    public string? Email { get; init; }

    public string TrangThai { get; init; } = "ACTIVE";

    public string Status { get; init; } = "yes";

    public DateTime? CreatedAt { get; init; }
}