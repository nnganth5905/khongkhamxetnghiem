using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Ctphieuxetnghiem
{
    public ulong Idctphieu { get; set; }

    public string IdphieuXetNghiem { get; set; } = null!;

    public string IdxetNghiem { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public string? GhiChu { get; set; }

    public string TrangThai { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Phieuxetnghiem IdphieuXetNghiemNavigation { get; set; } = null!;

    public virtual Loaixetnghiem IdxetNghiemNavigation { get; set; } = null!;

    public virtual Ketquaxetnghiem? Ketquaxetnghiem { get; set; }

    public virtual ICollection<Maubenhpham> Maubenhphams { get; set; } = new List<Maubenhpham>();

    public virtual ICollection<Worklist> Worklists { get; set; } = new List<Worklist>();
}
