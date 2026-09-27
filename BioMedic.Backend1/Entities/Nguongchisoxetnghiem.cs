using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Nguongchisoxetnghiem
{
    public ulong Idnguong { get; set; }

    public string IdchiSo { get; set; } = null!;

    public string GioiTinhApDung { get; set; } = null!;

    public ushort? TuoiMin { get; set; }

    public ushort? TuoiMax { get; set; }

    public decimal? GiaTriMin { get; set; }

    public decimal? GiaTriMax { get; set; }

    public string? GiaTriTextBinhThuong { get; set; }

    public string? GhiChu { get; set; }

    public string Status { get; set; } = null!;

    public virtual Chisoxetnghiem IdchiSoNavigation { get; set; } = null!;
}
