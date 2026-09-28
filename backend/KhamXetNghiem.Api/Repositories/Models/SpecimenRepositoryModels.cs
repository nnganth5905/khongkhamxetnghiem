namespace KhamXetNghiem.Api.Repositories.Models;

public sealed record SpecimenActorContext(
    int UserId,
    int? DoctorId,
    string? EmployeeId,
    string Role,
    string? EmployeePosition,
    string? EmployeeStatus
);

public sealed record SpecimenDbRow(
    string Id,
    long TestOrderItemId,
    string TestOrderId,
    string TestId,
    string TestName,
    string CustomerId,
    string PatientName,
    long? VisitId,
    string SpecimenType,
    string Barcode,
    DateTime? CollectedAt,
    int? CollectorDoctorId,
    string? CollectorName,
    string Status,
    string? Note,
    long? HandoverId,
    int? HandoverDoctorId,
    string? HandoverDoctorName,
    string? ReceiverId,
    string? ReceiverName,
    DateTime? HandedOverAt,
    DateTime? ReceivedAt,
    string? HandoverStatus
);