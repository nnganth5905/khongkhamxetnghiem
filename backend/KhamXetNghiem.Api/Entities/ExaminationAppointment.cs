using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("datlichkham")]
public sealed class ExaminationAppointment
{
    [Key]
    [Column("IDDatLichKham")]
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

    [Required]
    [Column("IDChuyenKhoa")]
    [MaxLength(10)]
    public string SpecialtyId { get; set; } = string.Empty;

    [Required]
    [Column("IDBacSi")]
    public int DoctorId { get; set; }

    // CoSoID = cơ sở, KHÔNG PHẢI phòng.
    [Required]
    [Column("CoSoID")]
    [MaxLength(20)]
    public string FacilityId { get; set; } = string.Empty;

    [Required]
    [Column("NgayKham", TypeName = "date")]
    public DateTime ExaminationDate { get; set; }

    [Required]
    [Column("GioKham", TypeName = "time")]
    public TimeSpan ExaminationTime { get; set; }

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