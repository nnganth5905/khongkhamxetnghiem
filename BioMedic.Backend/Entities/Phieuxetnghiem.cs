using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Phieuxetnghiem
{
    public string IdphieuXetNghiem { get; set; } = null!;

    public string IdkhachHang { get; set; } = null!;

    public ulong? Idkham { get; set; }

    public ulong? IdluotXetNghiem { get; set; }

    public int? IdbacSiChiDinh { get; set; }

    public int? IdbacSiPhuTrach { get; set; }

    public DateTime NgayTao { get; set; }

    public decimal TongTien { get; set; }

    public string TrangThaiThanhToan { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<Ctphieuxetnghiem> Ctphieuxetnghiems { get; set; } = new List<Ctphieuxetnghiem>();

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual Bacsi? IdbacSiChiDinhNavigation { get; set; }

    public virtual Bacsi? IdbacSiPhuTrachNavigation { get; set; }

    public virtual Khachhang IdkhachHangNavigation { get; set; } = null!;

    public virtual Kham? IdkhamNavigation { get; set; }

    public virtual Luotxetnghiem? IdluotXetNghiemNavigation { get; set; }
}
