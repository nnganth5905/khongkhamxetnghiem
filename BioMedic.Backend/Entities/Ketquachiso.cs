using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Ketquachiso
{
    public ulong IdketQuaChiSo { get; set; }

    public string IdketQua { get; set; } = null!;

    public string IdchiSo { get; set; } = null!;

    public decimal? GiaTriSo { get; set; }

    public string? GiaTriText { get; set; }

    public decimal? NguongMinApDung { get; set; }

    public decimal? NguongMaxApDung { get; set; }

    public string? GiaTriThamChieu { get; set; }

    public string DanhGia { get; set; } = null!;

    public string? GhiChu { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Chisoxetnghiem IdchiSoNavigation { get; set; } = null!;

    public virtual Ketquaxetnghiem IdketQuaNavigation { get; set; } = null!;
}
