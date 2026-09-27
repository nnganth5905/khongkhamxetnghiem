using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("coso")]
public class Facility
{
    [Key]
    [Column("CoSoID")]
    [MaxLength(20)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Column("TenCoSo")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Column("DiaChi")]
    [MaxLength(255)]
    public string? Address { get; set; }

    [Column("SoDienThoai")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [Column("Email")]
    [MaxLength(191)]
    public string? Email { get; set; }

    [Required]
    [Column("TrangThai")]
    public string Status { get; set; } = "active";

    [Column("CreatedAt")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedAt { get; set; }

    [Column("UpdatedAt")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
}