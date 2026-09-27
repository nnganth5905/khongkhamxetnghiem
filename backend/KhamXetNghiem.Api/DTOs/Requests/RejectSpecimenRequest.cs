using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class RejectSpecimenRequest
{
    [Required(
        ErrorMessage = "Vui lòng nhập lý do từ chối mẫu."
    )]
    public string Reason { get; set; } = string.Empty;
}