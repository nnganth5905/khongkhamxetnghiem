using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class RoomRequest
{
    [Required(
        ErrorMessage = "Tên phòng không được để trống."
    )]
    [StringLength(
        120,
        ErrorMessage = "Tên phòng tối đa 120 ký tự."
    )]
    public string TenPhong { get; set; } = string.Empty;

    [Required(
        ErrorMessage = "Loại phòng không được để trống."
    )]
    public string LoaiPhong { get; set; } = string.Empty;

    [Required(
        ErrorMessage = "Cơ sở không được để trống."
    )]
    public string IdCoSo { get; set; } = string.Empty;

    /*
     * DB có IDChuyenKhoa,
     * nhưng cho phép NULL.
     */
    public string? IdChuyenKhoa { get; set; }

    public string? Tang { get; set; }

    public string? TrangThai { get; set; }
}