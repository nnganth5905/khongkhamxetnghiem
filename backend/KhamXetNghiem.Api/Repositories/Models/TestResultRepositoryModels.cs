namespace KhamXetNghiem.Api.Repositories.Models;

public sealed record TestResultActor(
    int UserId,
    int? DoctorId,
    string? EmployeeId,
    string Role,
    string? EmployeePosition,
    string? EmployeeStatus
);

public sealed record ResultWorkContext(
    long WorklistId,
    long TestOrderItemId,
    string TestOrderId,
    string TestId,
    string TestName,
    string SpecimenId,
    string Barcode,
    string TechnicianId,
    string WorklistStatus,
    DateTime? WorkStartedAt,
    string CustomerId,
    string PatientName,
    DateTime? BirthDate,
    string? Gender,
    long? VisitId,
    int? AppointmentDoctorId,
    int? ResponsibleDoctorId
);

public sealed record IndicatorDefinition(
    string Id,
    string Name,
    string? Unit,
    string DataType
);

public sealed record IndicatorThreshold(
    decimal? Min,
    decimal? Max,
    string? NormalText,
    string? Note
);