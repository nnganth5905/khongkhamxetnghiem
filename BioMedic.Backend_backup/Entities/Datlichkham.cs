using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Datlichkham
{
    public ulong IddatLichKham { get; set; }

    public string MaDatLich { get; set; } = null!;

    public int? UserId { get; set; }

    public string IdkhachHang { get; set; } = null!;

    public string IdchuyenKhoa { get; set; } = null!;

    public int IdbacSi { get; set; }

    public string CoSoId { get; set; } = null!;

    public DateOnly NgayKham { get; set; }

    public TimeOnly GioKham { get; set; }

    public string? GhiChu { get; set; }

    public string TrangThai { get; set; } = null!;

    public string StatusMail { get; set; } = null!;

    public string? MaQr { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Coso CoSo { get; set; } = null!;

    public virtual Bacsi IdbacSiNavigation { get; set; } = null!;

    public virtual Chuyenkhoa IdchuyenKhoaNavigation { get; set; } = null!;

    public virtual Khachhang IdkhachHangNavigation { get; set; } = null!;

    public virtual Luotkham? Luotkham { get; set; }

    public virtual User? User { get; set; }
}
