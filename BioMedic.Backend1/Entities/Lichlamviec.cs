using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Lichlamviec
{
    public ulong LichId { get; set; }

    public int IdbacSi { get; set; }

    public string? Idphong { get; set; }

    public DateOnly Ngay { get; set; }

    public string Ca { get; set; } = null!;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public string NguonTao { get; set; } = null!;

    public int? NguoiTaoUserId { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bacsi IdbacSiNavigation { get; set; } = null!;

    public virtual Phong? IdphongNavigation { get; set; }

    public virtual User? NguoiTaoUser { get; set; }
}
