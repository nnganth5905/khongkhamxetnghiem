using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Nhanvien
{
    public string IdnhanVien { get; set; } = null!;

    public string TenNhanVien { get; set; } = null!;

    public string ViTri { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? CoSoId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Bacsi? Bacsi { get; set; }

    public virtual ICollection<Bangiaomau> Bangiaomaus { get; set; } = new List<Bangiaomau>();

    public virtual Coso? CoSo { get; set; }

    public virtual ICollection<Ketquaxetnghiem> Ketquaxetnghiems { get; set; } = new List<Ketquaxetnghiem>();

    public virtual User? User { get; set; }

    public virtual ICollection<Worklist> Worklists { get; set; } = new List<Worklist>();
}
