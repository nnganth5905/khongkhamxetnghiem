namespace KhamXetNghiem.Api.Enums;

/// <summary>
/// Trạng thái canonical dùng ở tầng API.
///
/// Repository vẫn lưu đúng enum tiếng Việt của MySQL.
/// </summary>
public enum VisitStatus
{
    WAITING,

    CALLED,

    ON_HOLD,

    IN_PROGRESS,

    WAITING_TEST,

    WAITING_RESULT,

    RESULT_READY,

    COMPLETED,

    CANCELLED
}