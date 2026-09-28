namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class QueueItemResponse
{
    public string Id { get; init; } = string.Empty;

    public string AppointmentId { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public int? QueueNumber { get; init; }

    public string CustomerId { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string ServiceName { get; init; } = string.Empty;

    public string? RoomId { get; init; }

    public string? RoomName { get; init; }

    public DateTime? ReceivedAt { get; init; }

    public string Status { get; init; } = string.Empty;

    // =====================================================
    // COMPATIBILITY VỚI FRONTEND JAVA CŨ
    // =====================================================

    public string MaLuot => AppointmentId;

    public int? Stt => QueueNumber;

    public string TenNguoiBenh => CustomerName;

    public string DichVu => ServiceName;

    public string Phong =>
        string.IsNullOrWhiteSpace(RoomName)
            ? "Chưa xếp phòng"
            : RoomName;

    public string CheckInTime =>
        ReceivedAt?.ToString("HH:mm") ?? string.Empty;

    public string TrangThai => Status;
}

public sealed class DoctorQueueItemResponse
{
    public long Id { get; init; }

    public int? Stt { get; init; }

    public string MaKhachHang { get; init; } = string.Empty;

    public string TenKhachHang { get; init; } = string.Empty;

    public string GioKham { get; init; } = string.Empty;

    public string TrangThai { get; init; } = string.Empty;

    public string? Phong { get; init; }

    public DateTime? ThoiGianTiepNhan { get; init; }
}