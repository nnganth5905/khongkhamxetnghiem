namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class EmployeeResponse
{
    public string Id { get; init; } = string.Empty;

    public string IdNhanVien => Id;

    public string HoTen { get; init; } = string.Empty;

    public string FullName => HoTen;

    /// <summary>
    /// Giá trị thân thiện cho frontend:
    /// RECEPTIONIST, TECHNICIAN, DOCTOR...
    /// </summary>
    public string VaiTro { get; init; } = string.Empty;

    /// <summary>
    /// Giá trị thật trong DB:
    /// letan, ktv, bacsi, admin...
    /// </summary>
    public string Position { get; init; } = string.Empty;

    public string? SoDienThoai { get; init; }

    public string? Phone => SoDienThoai;

    public string? Email { get; init; }

    public string? IdCoSo { get; init; }

    public string? FacilityId => IdCoSo;

    /// <summary>
    /// ACTIVE / INACTIVE
    /// </summary>
    public string TrangThai { get; init; } = "ACTIVE";

    /// <summary>
    /// yes / no
    /// </summary>
    public string Status { get; init; } = "yes";

    public DateTime? CreatedAt { get; init; }
}