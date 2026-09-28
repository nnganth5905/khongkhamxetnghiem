using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("datlichxetnghiem")]
public sealed class TestAppointment
{
    [Key]
    [Column("IDDatLichXN")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("MaDatLich")]
    [MaxLength(30)]
    public string AppointmentCode { get; set; } = string.Empty;

    [Column("UserID")]
    public int? UserId { get; set; }

    [Required]
    [Column("IDKhachHang")]
    [MaxLength(10)]
    public string CustomerId { get; set; } = string.Empty;

    [Column("IDBacSi")]
    public int? DoctorId { get; set; }

    [Required]
    [Column("CoSoID")]
    [MaxLength(20)]
    public string FacilityId { get; set; } = string.Empty;

    [Required]
    [Column("NgayXetNghiem", TypeName = "date")]
    public DateTime TestDate { get; set; }

    [Required]
    [Column("GioXetNghiem", TypeName = "time")]
    public TimeSpan TestTime { get; set; }

    [Column("GhiChu")]
    [MaxLength(500)]
    public string? Note { get; set; }

    [Required]
    [Column("TrangThai")]
    public string Status { get; set; } = "pending";

    [Required]
    [Column("StatusMail")]
    public string MailStatus { get; set; } = "pending";

    [Column("MaQR")]
    [MaxLength(255)]
    public string? QrCode { get; set; }

    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }
}