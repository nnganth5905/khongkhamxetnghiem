using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Chitiethoadon
{
    public ulong IdchiTietHoaDon { get; set; }

    public string MaHoaDon { get; set; } = null!;

    public string LoaiDichVu { get; set; } = null!;

    public string? MaDichVu { get; set; }

    public string TenDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual Hoadon MaHoaDonNavigation { get; set; } = null!;
}
