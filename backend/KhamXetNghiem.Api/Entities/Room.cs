using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("phong")]
public class Room
{
    [Key]
    [Column("IDPhong")]
    [MaxLength(20)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Column("CoSoID")]
    [MaxLength(20)]
    public string FacilityId { get; set; } = string.Empty;

    [Column("IDChuyenKhoa")]
    [MaxLength(10)]
    public string? SpecialtyId { get; set; }

    [Required]
    [Column("TenPhong")]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("LoaiPhong")]
    public string Type { get; set; } = "khac";

    [Column("Tang")]
    [MaxLength(20)]
    public string? Floor { get; set; }

    [Required]
    [Column("TrangThai")]
    public string Status { get; set; } = "active";
}