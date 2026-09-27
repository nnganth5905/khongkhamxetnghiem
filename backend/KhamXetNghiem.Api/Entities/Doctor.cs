using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("bacsi")]
public class Doctor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("IDBacSi")]
    public int Id { get; set; }

    [Required]
    [Column("TenBacSi")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Column("HocVi")]
    [MaxLength(100)]
    public string? Degree { get; set; }

    [Column("ChucDanh")]
    [MaxLength(150)]
    public string? Title { get; set; }

    [Required]
    [Column("KhoaID")]
    [MaxLength(10)]
    public string SpecialtyId { get; set; } = string.Empty;

    /*
     * FIX:
     * Java cũ gọi field này là roomId,
     * nhưng DB thực tế là CoSoID.
     */
    [Column("CoSoID")]
    [MaxLength(20)]
    public string? FacilityId { get; set; }

    [Column("MoTa")]
    public string? Bio { get; set; }

    [Column("HinhAnh")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("SoSao", TypeName = "decimal(2,1)")]
    public decimal Rating { get; set; } = 5.0m;

    [Column("NamKinhNghiem")]
    public byte? ExperienceYears { get; set; }

    [Required]
    [Column("TrangThai")]
    public string Status { get; set; } = "active";

    [Column("IDNhanVien")]
    [MaxLength(10)]
    public string? EmployeeId { get; set; }
}