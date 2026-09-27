using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Entities;

public partial class Worklist
{
    public ulong Id { get; set; }

    public ulong Idctphieu { get; set; }

    public string? Idmau { get; set; }

    public string? Idktv { get; set; }

    public string? Instrument { get; set; }

    public string? ReagentLot { get; set; }

    public string Status { get; set; } = null!;

    public DateTime ReceivedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Ctphieuxetnghiem IdctphieuNavigation { get; set; } = null!;

    public virtual Nhanvien? IdktvNavigation { get; set; }

    public virtual Maubenhpham? IdmauNavigation { get; set; }
}
