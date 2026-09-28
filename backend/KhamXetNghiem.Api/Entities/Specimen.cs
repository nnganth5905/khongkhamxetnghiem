namespace KhamXetNghiem.Api.Entities;

/// <summary>
/// Domain model chung cho mẫu bệnh phẩm.
///
/// Database thật sử dụng bảng maubenhpham.
/// Module này chủ yếu thao tác bằng repository/raw SQL
/// để quản lý transaction và state transition.
/// </summary>
public sealed class Specimen
{
    public string Id { get; set; } = string.Empty;

    public long TestOrderItemId { get; set; }

    public string TestOrderId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public long? VisitId { get; set; }

    public string SpecimenType { get; set; } = string.Empty;

    /// <summary>
    /// Database không có cột specimenCode riêng.
    /// SpecimenCode tương ứng IDMau.
    /// </summary>
    public string SpecimenCode { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public DateTime? CollectedAt { get; set; }

    public int? CollectorDoctorId { get; set; }

    public DateTime? HandedOverAt { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public string? TechnicianId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Note { get; set; }
}