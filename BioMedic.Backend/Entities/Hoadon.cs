using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Hoadon
{
    public string MaHoaDon { get; set; } = null!;

    public string IdkhachHang { get; set; } = null!;

    public string? IdphieuXetNghiem { get; set; }

    public ulong? IdluotKham { get; set; }

    public DateTime NgayTaoHoaDon { get; set; }

    public decimal TongTien { get; set; }

    public string TrangThaiThanhToan { get; set; } = null!;

    public string? PhuongThuc { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Chitiethoadon> Chitiethoadons { get; set; } = new List<Chitiethoadon>();

    public virtual Khachhang IdkhachHangNavigation { get; set; } = null!;

    public virtual Luotkham? IdluotKhamNavigation { get; set; }

    public virtual Phieuxetnghiem? IdphieuXetNghiemNavigation { get; set; }
}
