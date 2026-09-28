using System.Text.Json.Serialization;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class UpdateAppointmentRequest
{
    [JsonPropertyName("new_date")]
    public string NewDate { get; set; } = string.Empty;

    [JsonPropertyName("new_time")]
    public string NewTime { get; set; } = string.Empty;

    [JsonPropertyName("new_bs")]
    public string? NewDoctorId { get; set; }

    [JsonPropertyName("ghichu")]
    public string? Note { get; set; }
}