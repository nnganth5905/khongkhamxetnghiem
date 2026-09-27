using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Luotxetnghiem
{
    public ulong IdluotXetNghiem { get; set; }

    public ulong IddatLichXn { get; set; }

    public string IdkhachHang { get; set; } = null!;

    public string? Idphong { get; set; }

    public int? SoThuTu { get; set; }

    public DateTime? ThoiGianTiepNhan { get; set; }

    public DateTime? ThoiGianBatDau { get; set; }

    public DateTime? ThoiGianKetThuc { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual Datlichxetnghiem IddatLichXnNavigation { get; set; } = null!;

    public virtual Khachhang IdkhachHangNavigation { get; set; } = null!;

    public virtual Phong? IdphongNavigation { get; set; }

    public virtual ICollection<Phieuxetnghiem> Phieuxetnghiems { get; set; } = new List<Phieuxetnghiem>();
}
