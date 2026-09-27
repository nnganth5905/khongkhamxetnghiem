namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class SpecimenListItemResponse
{
    public string Id { get; init; } = string.Empty;

    public string IdMauBenhPham => Id;

    public string MaMau => Id;

    public string MaVach { get; init; } = string.Empty;

    public string? TenKhachHang { get; init; }

    public string LoaiMau { get; init; } = string.Empty;

    public DateTime? ThoiGianLay { get; init; }

    public string TrangThai { get; init; } = "UNKNOWN";
}