using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Ctdatlichxetnghiem
{
    public ulong IdctdatLichXn { get; set; }

    public ulong IddatLichXn { get; set; }

    public string IdxetNghiem { get; set; } = null!;

    public decimal DonGia { get; set; }

    public string? GhiChu { get; set; }

    public virtual Datlichxetnghiem IddatLichXnNavigation { get; set; } = null!;

    public virtual Loaixetnghiem IdxetNghiemNavigation { get; set; } = null!;
}
