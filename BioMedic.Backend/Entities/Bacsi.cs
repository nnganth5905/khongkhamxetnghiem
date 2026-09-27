using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Bacsi
{
    public int IdbacSi { get; set; }

    public string TenBacSi { get; set; } = null!;

    public string? HocVi { get; set; }

    public string? ChucDanh { get; set; }

    public string KhoaId { get; set; } = null!;

    public string? CoSoId { get; set; }

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }

    public decimal SoSao { get; set; }

    public byte? NamKinhNghiem { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? IdnhanVien { get; set; }

    public virtual ICollection<Bangiaomau> Bangiaomaus { get; set; } = new List<Bangiaomau>();

    public virtual Coso? CoSo { get; set; }

    public virtual ICollection<Datlichkham> Datlichkhams { get; set; } = new List<Datlichkham>();

    public virtual ICollection<Datlichxetnghiem> Datlichxetnghiems { get; set; } = new List<Datlichxetnghiem>();

    public virtual Nhanvien? IdnhanVienNavigation { get; set; }

    public virtual ICollection<Ketquaxetnghiem> Ketquaxetnghiems { get; set; } = new List<Ketquaxetnghiem>();

    public virtual ICollection<Kham> Khams { get; set; } = new List<Kham>();

    public virtual Chuyenkhoa Khoa { get; set; } = null!;

    public virtual ICollection<Lichlamviec> Lichlamviecs { get; set; } = new List<Lichlamviec>();

    public virtual ICollection<Luotkham> Luotkhams { get; set; } = new List<Luotkham>();

    public virtual ICollection<Maubenhpham> Maubenhphams { get; set; } = new List<Maubenhpham>();

    public virtual ICollection<Phieuxetnghiem> PhieuxetnghiemIdbacSiChiDinhNavigations { get; set; } = new List<Phieuxetnghiem>();

    public virtual ICollection<Phieuxetnghiem> PhieuxetnghiemIdbacSiPhuTrachNavigations { get; set; } = new List<Phieuxetnghiem>();

    public virtual User? User { get; set; }
}
