using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Phong
{
    public string Idphong { get; set; } = null!;

    public string CoSoId { get; set; } = null!;

    public string? IdchuyenKhoa { get; set; }

    public string TenPhong { get; set; } = null!;

    public string LoaiPhong { get; set; } = null!;

    public string? Tang { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual Coso CoSo { get; set; } = null!;

    public virtual Chuyenkhoa? IdchuyenKhoaNavigation { get; set; }

    public virtual ICollection<Lichlamviec> Lichlamviecs { get; set; } = new List<Lichlamviec>();

    public virtual ICollection<Luotkham> Luotkhams { get; set; } = new List<Luotkham>();

    public virtual ICollection<Luotxetnghiem> Luotxetnghiems { get; set; } = new List<Luotxetnghiem>();
}
