namespace KhamXetNghiem.Api.Entities;

/// <summary>
/// Domain model kết quả xét nghiệm.
/// Database thật:
/// - ketquaxetnghiem
/// - ketquachiso
///
/// Module 11 thao tác bằng repository/raw SQL để kiểm soát
/// transaction và state transition.
/// </summary>
public sealed class TestResult
{
    public string Id { get; set; } = string.Empty;

    public long TestOrderItemId { get; set; }

    public string? SpecimenId { get; set; }

    public string TechnicianId { get; set; } = string.Empty;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime EnteredAt { get; set; }

    public string? GeneralResult { get; set; }

    public string Status { get; set; } = string.Empty;

    public int? ApprovedByDoctorId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? DoctorConclusion { get; set; }

    public string? Note { get; set; }
}