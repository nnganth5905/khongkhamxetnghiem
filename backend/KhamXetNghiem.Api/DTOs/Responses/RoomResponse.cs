namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class RoomResponse
{
    public string Id { get; init; } = string.Empty;

    public string IdPhong => Id;

    public string MaPhong => Id;

    public string TenPhong { get; init; } = string.Empty;

    public string Name => TenPhong;

    /// <summary>
    /// EXAMINATION / SAMPLING / LAB / CONSULTATION / OTHER
    /// </summary>
    public string LoaiPhong { get; init; } = "OTHER";

    public string DbLoaiPhong { get; init; } = "khac";

    public string IdCoSo { get; init; } = string.Empty;

    public string? TenCoSo { get; init; }

    public string? IdChuyenKhoa { get; init; }

    public string? TenChuyenKhoa { get; init; }

    public string? Tang { get; init; }

    /// <summary>
    /// AVAILABLE / MAINTENANCE / INACTIVE
    /// </summary>
    public string TrangThai { get; init; } = "AVAILABLE";

    public string Status { get; init; } = "active";
}