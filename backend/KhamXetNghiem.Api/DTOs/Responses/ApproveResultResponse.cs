namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class ApproveResultResponse
{
    public string Status { get; init; } = "SUCCESS";

    public string Message { get; init; } =
        "Đã phê duyệt kết quả xét nghiệm thành công.";

    public DateTime ApprovedAt { get; init; }
}