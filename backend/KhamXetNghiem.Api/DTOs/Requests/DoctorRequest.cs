using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class DoctorRequest
{
    [Required(
        ErrorMessage = "Họ tên bác sĩ không được để trống."
    )]
    public string HoTen { get; set; } = string.Empty;

    [Required(
        ErrorMessage = "Chuyên khoa không được để trống."
    )]
    public string IdChuyenKhoa { get; set; } = string.Empty;

    public string? HocVi { get; set; }

    public string? ChucDanh { get; set; }

    /*
     * FIX:
     * bacsi.CoSoID là cơ sở,
     * không phải phòng.
     */
    public string? CoSoId { get; set; }

    public string? HinhAnh { get; set; }

    public string? GioiThieu { get; set; }

    public string? TrangThai { get; set; }

    public byte? NamKinhNghiem { get; set; }
}