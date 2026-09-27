using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Kham
{
    public ulong Idkham { get; set; }

    public ulong IdluotKham { get; set; }

    public int IdbacSi { get; set; }

    public string? TrieuChung { get; set; }

    public string? TienSuBenh { get; set; }

    public string? ChanDoan { get; set; }

    public string? KetLuan { get; set; }

    public string? HuongDieuTri { get; set; }

    public DateTime ThoiGianKham { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bacsi IdbacSiNavigation { get; set; } = null!;

    public virtual Luotkham IdluotKhamNavigation { get; set; } = null!;

    public virtual ICollection<Phieuxetnghiem> Phieuxetnghiems { get; set; } = new List<Phieuxetnghiem>();
}
