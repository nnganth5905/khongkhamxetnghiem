using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Chisoxetnghiem
{
    public string IdchiSo { get; set; } = null!;

    public string IdxetNghiem { get; set; } = null!;

    public string TenChiSo { get; set; } = null!;

    public string? DonVi { get; set; }

    public string KieuDuLieu { get; set; } = null!;

    public string? GhiChu { get; set; }

    public string Status { get; set; } = null!;

    public virtual Loaixetnghiem IdxetNghiemNavigation { get; set; } = null!;

    public virtual ICollection<Ketquachiso> Ketquachisos { get; set; } = new List<Ketquachiso>();

    public virtual ICollection<Nguongchisoxetnghiem> Nguongchisoxetnghiems { get; set; } = new List<Nguongchisoxetnghiem>();
}
