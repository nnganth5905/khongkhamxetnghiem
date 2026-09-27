using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("chuyenkhoa")]
public class Specialty
{
    [Key]
    [Column("IDChuyenKhoa")]
    [MaxLength(10)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Column("TenChuyenKhoa")]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Column("MoTa")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [Column("Status")]
    public string Status { get; set; } = "yes";
}