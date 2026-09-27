using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Luotkham
{
    public ulong IdluotKham { get; set; }

    public ulong IddatLichKham { get; set; }

    public string IdkhachHang { get; set; } = null!;

    public int IdbacSi { get; set; }

    public string? Idphong { get; set; }

    public int? SoThuTu { get; set; }

    public DateTime? ThoiGianTiepNhan { get; set; }

    public DateTime? ThoiGianBatDau { get; set; }

    public DateTime? ThoiGianKetThuc { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual Bacsi IdbacSiNavigation { get; set; } = null!;

    public virtual Datlichkham IddatLichKhamNavigation { get; set; } = null!;

    public virtual Khachhang IdkhachHangNavigation { get; set; } = null!;

    public virtual Phong? IdphongNavigation { get; set; }

    public virtual Kham? Kham { get; set; }
}
