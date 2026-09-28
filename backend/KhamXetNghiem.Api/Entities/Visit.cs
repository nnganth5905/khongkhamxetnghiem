namespace KhamXetNghiem.Api.Entities;

/// <summary>
/// Domain model chung cho lượt thực tế.
///
/// Database KHÔNG có bảng "Visit".
/// Dữ liệu thật nằm ở:
/// - luotkham
/// - luotxetnghiem
///
/// Vì vậy class này tuyệt đối không map EF vào một table.
/// </summary>
public sealed class Visit
{
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// MaDatLich: DLK-... hoặc DLXN-...
    /// </summary>
    public string AppointmentId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string? DoctorId { get; set; }

    public string? RoomId { get; set; }

    public string Type { get; set; } = string.Empty;

    public int? QueueNumber { get; set; }

    /// <summary>
    /// MySQL DATETIME theo giờ Việt Nam.
    /// </summary>
    public DateTime? ReceivedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Giữ raw database status:
    /// da_tiep_nhan, cho_kham, ...
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// luotkham/luotxetnghiem không có cột note.
    /// Giá trị này lấy từ GhiChu của appointment.
    /// Các note khi đổi trạng thái được ghi vào truyvet.MoTa.
    /// </summary>
    public string? Note { get; set; }
}