using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Ketquaxetnghiem
{
    public string IdketQua { get; set; } = null!;

    public ulong Idctphieu { get; set; }

    public string? Idmau { get; set; }

    public string Idktv { get; set; } = null!;

    public DateTime? ThoiGianBatDau { get; set; }

    public DateTime? ThoiGianHoanThanh { get; set; }

    public DateTime ThoiGianNhap { get; set; }

    public string? KetQuaTongQuat { get; set; }

    public string TrangThai { get; set; } = null!;

    public int? IdbacSiDuyet { get; set; }

    public DateTime? ThoiGianDuyet { get; set; }

    public string? KetLuanBacSi { get; set; }

    public string? GhiChu { get; set; }

    public virtual Bacsi? IdbacSiDuyetNavigation { get; set; }

    public virtual Ctphieuxetnghiem IdctphieuNavigation { get; set; } = null!;

    public virtual Nhanvien IdktvNavigation { get; set; } = null!;

    public virtual Maubenhpham? IdmauNavigation { get; set; }

    public virtual ICollection<Ketquachiso> Ketquachisos { get; set; } = new List<Ketquachiso>();
}
