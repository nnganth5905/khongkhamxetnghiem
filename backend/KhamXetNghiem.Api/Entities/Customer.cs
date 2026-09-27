using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("khachhang")]
public class Customer
{
    [Key]
    [Column("IDKhachHang")]
    [MaxLength(10)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Column("TenKhachHang")]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Column("NgaySinh", TypeName = "date")]
    public DateOnly? BirthDate { get; set; }

    [Column("SoDienThoai")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [Column("GioiTinh")]
    public string? Gender { get; set; }

    [Column("CCCD")]
    [MaxLength(20)]
    public string? CitizenId { get; set; }

    [Column("DiaChi")]
    [MaxLength(255)]
    public string? Address { get; set; }

    [Column("Email")]
    [MaxLength(191)]
    public string? Email { get; set; }

    [Required]
    [Column("Status")]
    public string Status { get; set; } = "yes";

    [Column("CreatedAt")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedAt { get; set; }

    [Column("UpdatedAt")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
}