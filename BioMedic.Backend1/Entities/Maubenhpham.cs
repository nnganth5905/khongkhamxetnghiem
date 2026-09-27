using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Maubenhpham
{
    public string Idmau { get; set; } = null!;

    public ulong Idctphieu { get; set; }

    public string MaBarcode { get; set; } = null!;

    public string LoaiMau { get; set; } = null!;

    public int? IdbacSiLayMau { get; set; }

    public DateTime? ThoiGianLayMau { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<Bangiaomau> Bangiaomaus { get; set; } = new List<Bangiaomau>();

    public virtual Bacsi? IdbacSiLayMauNavigation { get; set; }

    public virtual Ctphieuxetnghiem IdctphieuNavigation { get; set; } = null!;

    public virtual ICollection<Ketquaxetnghiem> Ketquaxetnghiems { get; set; } = new List<Ketquaxetnghiem>();

    public virtual ICollection<Worklist> Worklists { get; set; } = new List<Worklist>();
}
