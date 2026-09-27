using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Khachhang
{
    public string IdkhachHang { get; set; } = null!;

    public string TenKhachHang { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? GioiTinh { get; set; }

    public string? Cccd { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Datlichkham> Datlichkhams { get; set; } = new List<Datlichkham>();

    public virtual ICollection<Datlichxetnghiem> Datlichxetnghiems { get; set; } = new List<Datlichxetnghiem>();

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual ICollection<Luotkham> Luotkhams { get; set; } = new List<Luotkham>();

    public virtual ICollection<Luotxetnghiem> Luotxetnghiems { get; set; } = new List<Luotxetnghiem>();

    public virtual ICollection<Phieuxetnghiem> Phieuxetnghiems { get; set; } = new List<Phieuxetnghiem>();

    public virtual ICollection<Truyvet> Truyvets { get; set; } = new List<Truyvet>();

    public virtual User? User { get; set; }
}
