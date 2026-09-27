using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? Username { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? IdkhachHang { get; set; }

    public int? IdbacSi { get; set; }

    public string? IdnhanVien { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Datlichkham> Datlichkhams { get; set; } = new List<Datlichkham>();

    public virtual ICollection<Datlichxetnghiem> Datlichxetnghiems { get; set; } = new List<Datlichxetnghiem>();

    public virtual Bacsi? IdbacSiNavigation { get; set; }

    public virtual Khachhang? IdkhachHangNavigation { get; set; }

    public virtual Nhanvien? IdnhanVienNavigation { get; set; }

    public virtual ICollection<Lichlamviec> Lichlamviecs { get; set; } = new List<Lichlamviec>();

    public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

    public virtual ICollection<Thongbao> Thongbaos { get; set; } = new List<Thongbao>();

    public virtual ICollection<Truyvet> Truyvets { get; set; } = new List<Truyvet>();
}
