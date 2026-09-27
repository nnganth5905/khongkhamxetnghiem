using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Truyvet
{
    public ulong IdtruyVet { get; set; }

    public string IdkhachHang { get; set; } = null!;

    public string LoaiDoiTuong { get; set; } = null!;

    public string IddoiTuong { get; set; } = null!;

    public string HanhDong { get; set; } = null!;

    public string? TrangThaiCu { get; set; }

    public string? TrangThaiMoi { get; set; }

    public int? UserIdthucHien { get; set; }

    public string NguonThucHien { get; set; } = null!;

    public DateTime ThoiGian { get; set; }

    public string? MoTa { get; set; }

    public string? Ipaddress { get; set; }

    public virtual Khachhang IdkhachHangNavigation { get; set; } = null!;

    public virtual User? UserIdthucHienNavigation { get; set; }
}
