using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Loaixetnghiem
{
    public string IdxetNghiem { get; set; } = null!;

    public string TenXetNghiem { get; set; } = null!;

    public string ChuyenKhoaId { get; set; } = null!;

    public string? Loai { get; set; }

    public string? MoTa { get; set; }

    public decimal Gia { get; set; }

    public string? LoaiMauMacDinh { get; set; }

    public int? ThoiGianDuKienPhut { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Chisoxetnghiem> Chisoxetnghiems { get; set; } = new List<Chisoxetnghiem>();

    public virtual Chuyenkhoa ChuyenKhoa { get; set; } = null!;

    public virtual ICollection<Ctdatlichxetnghiem> Ctdatlichxetnghiems { get; set; } = new List<Ctdatlichxetnghiem>();

    public virtual ICollection<Ctphieuxetnghiem> Ctphieuxetnghiems { get; set; } = new List<Ctphieuxetnghiem>();
}
