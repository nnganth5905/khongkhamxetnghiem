using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("nhanvien")]
public class Employee
{
    [Key]
    [Column("IDNhanVien")]
    [MaxLength(10)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Column("TenNhanVien")]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Column("ViTri")]
    public string Position { get; set; } = "khac";

    [Column("SoDienThoai")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [Column("Email")]
    [MaxLength(191)]
    public string? Email { get; set; }

    [Column("CoSoID")]
    [MaxLength(20)]
    public string? FacilityId { get; set; }

    [Required]
    [Column("Status")]
    public string Status { get; set; } = "yes";

    [Column("CreatedAt")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedAt { get; set; }
}