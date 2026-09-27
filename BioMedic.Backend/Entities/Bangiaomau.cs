using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Bangiaomau
{
    public ulong IdbanGiao { get; set; }

    public string Idmau { get; set; } = null!;

    public int IdbacSiBanGiao { get; set; }

    public string IdktvtiepNhan { get; set; } = null!;

    public DateTime ThoiGianBanGiao { get; set; }

    public DateTime? ThoiGianTiepNhan { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? LyDoTuChoi { get; set; }

    public string? GhiChu { get; set; }

    public virtual Bacsi IdbacSiBanGiaoNavigation { get; set; } = null!;

    public virtual Nhanvien IdktvtiepNhanNavigation { get; set; } = null!;

    public virtual Maubenhpham IdmauNavigation { get; set; } = null!;
}
