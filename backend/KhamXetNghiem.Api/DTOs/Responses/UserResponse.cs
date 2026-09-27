namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed record UserResponse(
    string Id,
    string? Username,
    string Email,
    string FullName,
    string Role,
    string Status
);
