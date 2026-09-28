using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("ctdatlichxetnghiem")]
public sealed class TestAppointmentItem
{
    [Key]
    [Column("IDCTDatLichXN")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("IDDatLichXN")]
    public long TestAppointmentId { get; set; }

    [Required]
    [Column("IDXetNghiem")]
    [MaxLength(10)]
    public string TestId { get; set; } = string.Empty;

    [Required]
    [Column("DonGia")]
    public decimal Price { get; set; }

    [Column("GhiChu")]
    [MaxLength(255)]
    public string? Note { get; set; }
}