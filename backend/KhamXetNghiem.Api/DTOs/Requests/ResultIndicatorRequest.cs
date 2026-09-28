namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class ResultIndicatorRequest
{
    public string IndicatorId { get; set; } = string.Empty;

    /// <summary>
    /// Frontend gửi string để hỗ trợ cả number và text.
    /// Backend quyết định GiaTriSo/GiaTriText dựa trên KieuDuLieu.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// true = KTV chủ động đánh dấu bất thường.
    /// false không có nghĩa chắc chắn bình thường;
    /// backend vẫn tự đánh giá theo ngưỡng.
    /// </summary>
    public bool Abnormal { get; set; }

    public string? Note { get; set; }
}