using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Thongbao
{
    public ulong IdthongBao { get; set; }

    public int UserIdnhan { get; set; }

    public string LoaiThongBao { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string? LoaiDoiTuong { get; set; }

    public string? IddoiTuong { get; set; }

    public bool DaDoc { get; set; }

    public DateTime ThoiGianTao { get; set; }

    public DateTime? ThoiGianDoc { get; set; }

    public virtual User UserIdnhanNavigation { get; set; } = null!;
}
