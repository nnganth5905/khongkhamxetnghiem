using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("users")]
public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("Username")]
    public string? Username { get; set; }

    [Required]
    [Column("Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("Role")]
    public string Role { get; set; } = "khachhang";

    [Column("IDKhachHang")]
    public string? IdKhachHang { get; set; }

    [Column("IDBacSi")]
    public int? IdBacSi { get; set; }

    [Column("IDNhanVien")]
    public string? IdNhanVien { get; set; }

    [Required]
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }
}
