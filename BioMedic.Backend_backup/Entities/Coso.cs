using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Coso
{
    public string CoSoId { get; set; } = null!;

    public string TenCoSo { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Bacsi> Bacsis { get; set; } = new List<Bacsi>();

    public virtual ICollection<Datlichkham> Datlichkhams { get; set; } = new List<Datlichkham>();

    public virtual ICollection<Datlichxetnghiem> Datlichxetnghiems { get; set; } = new List<Datlichxetnghiem>();

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
