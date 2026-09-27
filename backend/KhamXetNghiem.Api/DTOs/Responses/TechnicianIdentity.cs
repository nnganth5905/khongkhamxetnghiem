namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed record TechnicianIdentity(
    int UserId,
    string EmployeeId,
    string FullName,
    string? Email,
    string? Phone,
    string? FacilityId
);