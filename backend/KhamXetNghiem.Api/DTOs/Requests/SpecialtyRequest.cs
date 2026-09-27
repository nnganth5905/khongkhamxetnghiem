using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class SpecialtyRequest
{
    [Required(
        ErrorMessage = "Tên chuyên khoa không được để trống."
    )]
    [StringLength(
        128,
        ErrorMessage = "Tên chuyên khoa tối đa 128 ký tự."
    )]
    public string TenChuyenKhoa { get; set; } = string.Empty;

    [StringLength(
        500,
        ErrorMessage = "Mô tả tối đa 500 ký tự."
    )]
    public string? MoTa { get; set; }

    public string? TrangThai { get; set; }
}