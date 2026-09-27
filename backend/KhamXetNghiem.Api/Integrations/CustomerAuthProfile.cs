namespace KhamXetNghiem.Api.Integrations;

/// <summary>
/// Chỉ chứa các trường mà AuthService.java thực sự sử dụng.
/// Đây KHÔNG phải Entity EF Core và không đoán tên bảng/cột Customer.
/// </summary>
public sealed record CustomerAuthProfile(
    string Id,
    string FullName,
    string Email,
    string Phone
);
