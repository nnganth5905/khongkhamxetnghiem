using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Chuyenkhoa
{
    public string IdchuyenKhoa { get; set; } = null!;

    public string TenChuyenKhoa { get; set; } = null!;

    public string? MoTa { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Bacsi> Bacsis { get; set; } = new List<Bacsi>();

    public virtual ICollection<Datlichkham> Datlichkhams { get; set; } = new List<Datlichkham>();

    public virtual ICollection<Loaixetnghiem> Loaixetnghiems { get; set; } = new List<Loaixetnghiem>();

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
