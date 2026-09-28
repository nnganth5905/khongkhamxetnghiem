using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhamXetNghiem.Api.Entities;

[Table("lichlamviec")]
public sealed class WorkSchedule
{
    [Key]
    [Column("LichID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("IDBacSi")]
    public int DoctorId { get; set; }

    [Column("IDPhong")]
    [MaxLength(20)]
    public string? RoomId { get; set; }

    [Required]
    [Column("Ngay", TypeName = "date")]
    public DateTime WorkDate { get; set; }

    [Required]
    [Column("Ca")]
    public string Shift { get; set; } = "TuyChinh";

    [Required]
    [Column("GioBatDau", TypeName = "time")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Column("GioKetThuc", TypeName = "time")]
    public TimeSpan EndTime { get; set; }

    [Required]
    [Column("NguonTao")]
    public string Source { get; set; } = "admin";

    [Column("NguoiTaoUserID")]
    public int? CreatedByUserId { get; set; }

    [Required]
    [Column("TrangThai")]
    public string Status { get; set; } = "duoc_duyet";

    [Column("GhiChu")]
    [MaxLength(255)]
    public string? Note { get; set; }

    [Column("CreatedAt")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }
}