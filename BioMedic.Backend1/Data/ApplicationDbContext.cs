using System;
using System.Collections.Generic;
using BioMedic.Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace BioMedic.Backend.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bacsi> Bacsis { get; set; }

    public virtual DbSet<Bangiaomau> Bangiaomaus { get; set; }

    public virtual DbSet<Chisoxetnghiem> Chisoxetnghiems { get; set; }

    public virtual DbSet<Chitiethoadon> Chitiethoadons { get; set; }

    public virtual DbSet<Chuyenkhoa> Chuyenkhoas { get; set; }

    public virtual DbSet<Coso> Cosos { get; set; }

    public virtual DbSet<Ctdatlichxetnghiem> Ctdatlichxetnghiems { get; set; }

    public virtual DbSet<Ctphieuxetnghiem> Ctphieuxetnghiems { get; set; }

    public virtual DbSet<Datlichkham> Datlichkhams { get; set; }

    public virtual DbSet<Datlichxetnghiem> Datlichxetnghiems { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Ketquachiso> Ketquachisos { get; set; }

    public virtual DbSet<Ketquaxetnghiem> Ketquaxetnghiems { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Kham> Khams { get; set; }

    public virtual DbSet<Lichlamviec> Lichlamviecs { get; set; }

    public virtual DbSet<Loaixetnghiem> Loaixetnghiems { get; set; }

    public virtual DbSet<Luotkham> Luotkhams { get; set; }

    public virtual DbSet<Luotxetnghiem> Luotxetnghiems { get; set; }

    public virtual DbSet<Maubenhpham> Maubenhphams { get; set; }

    public virtual DbSet<Nguongchisoxetnghiem> Nguongchisoxetnghiems { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<Phieuxetnghiem> Phieuxetnghiems { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<Thongbao> Thongbaos { get; set; }

    public virtual DbSet<Truyvet> Truyvets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Worklist> Worklists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Bacsi>(entity =>
        {
            entity.HasKey(e => e.IdbacSi).HasName("PRIMARY");

            entity
                .ToTable("bacsi")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdnhanVien, "IDNhanVien").IsUnique();

            entity.HasIndex(e => e.KhoaId, "fk_bacsi_chuyenkhoa");

            entity.HasIndex(e => e.CoSoId, "fk_bacsi_coso");

            entity.Property(e => e.IdbacSi).HasColumnName("IDBacSi");
            entity.Property(e => e.ChucDanh).HasMaxLength(150);
            entity.Property(e => e.CoSoId)
                .HasMaxLength(20)
                .HasColumnName("CoSoID");
            entity.Property(e => e.HinhAnh).HasMaxLength(500);
            entity.Property(e => e.HocVi).HasMaxLength(100);
            entity.Property(e => e.IdnhanVien)
                .HasMaxLength(10)
                .HasColumnName("IDNhanVien");
            entity.Property(e => e.KhoaId)
                .HasMaxLength(10)
                .HasColumnName("KhoaID");
            entity.Property(e => e.MoTa).HasColumnType("text");
            entity.Property(e => e.SoSao)
                .HasPrecision(2, 1)
                .HasDefaultValueSql("'5.0'");
            entity.Property(e => e.TenBacSi).HasMaxLength(255);
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','inactive')");

            entity.HasOne(d => d.CoSo).WithMany(p => p.Bacsis)
                .HasForeignKey(d => d.CoSoId)
                .HasConstraintName("fk_bacsi_coso");

            entity.HasOne(d => d.IdnhanVienNavigation).WithOne(p => p.Bacsi)
                .HasForeignKey<Bacsi>(d => d.IdnhanVien)
                .HasConstraintName("fk_bacsi_nhanvien");

            entity.HasOne(d => d.Khoa).WithMany(p => p.Bacsis)
                .HasForeignKey(d => d.KhoaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bacsi_chuyenkhoa");
        });

        modelBuilder.Entity<Bangiaomau>(entity =>
        {
            entity.HasKey(e => e.IdbanGiao).HasName("PRIMARY");

            entity
                .ToTable("bangiaomau")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdbacSiBanGiao, "fk_bangiao_bacsi");

            entity.HasIndex(e => e.IdktvtiepNhan, "fk_bangiao_ktv");

            entity.HasIndex(e => new { e.Idmau, e.ThoiGianBanGiao }, "idx_bangiao_mau");

            entity.Property(e => e.IdbanGiao).HasColumnName("IDBanGiao");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.IdbacSiBanGiao).HasColumnName("IDBacSiBanGiao");
            entity.Property(e => e.IdktvtiepNhan)
                .HasMaxLength(10)
                .HasColumnName("IDKTVTiepNhan");
            entity.Property(e => e.Idmau)
                .HasMaxLength(30)
                .HasColumnName("IDMau");
            entity.Property(e => e.LyDoTuChoi).HasMaxLength(255);
            entity.Property(e => e.ThoiGianBanGiao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTiepNhan).HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'da_ban_giao'")
                .HasColumnType("enum('da_ban_giao','da_tiep_nhan','tu_choi')");

            entity.HasOne(d => d.IdbacSiBanGiaoNavigation).WithMany(p => p.Bangiaomaus)
                .HasForeignKey(d => d.IdbacSiBanGiao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bangiao_bacsi");

            entity.HasOne(d => d.IdktvtiepNhanNavigation).WithMany(p => p.Bangiaomaus)
                .HasForeignKey(d => d.IdktvtiepNhan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bangiao_ktv");

            entity.HasOne(d => d.IdmauNavigation).WithMany(p => p.Bangiaomaus)
                .HasForeignKey(d => d.Idmau)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bangiao_mau");
        });

        modelBuilder.Entity<Chisoxetnghiem>(entity =>
        {
            entity.HasKey(e => e.IdchiSo).HasName("PRIMARY");

            entity
                .ToTable("chisoxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdxetNghiem, "idx_chiso_xn");

            entity.Property(e => e.IdchiSo)
                .HasMaxLength(10)
                .HasColumnName("IDChiSo");
            entity.Property(e => e.DonVi).HasMaxLength(30);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.IdxetNghiem)
                .HasMaxLength(10)
                .HasColumnName("IDXetNghiem");
            entity.Property(e => e.KieuDuLieu)
                .HasDefaultValueSql("'number'")
                .HasColumnType("enum('number','text')");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.TenChiSo).HasMaxLength(120);

            entity.HasOne(d => d.IdxetNghiemNavigation).WithMany(p => p.Chisoxetnghiems)
                .HasForeignKey(d => d.IdxetNghiem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_chiso_loaixn");
        });

        modelBuilder.Entity<Chitiethoadon>(entity =>
        {
            entity.HasKey(e => e.IdchiTietHoaDon).HasName("PRIMARY");

            entity
                .ToTable("chitiethoadon")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MaHoaDon, "fk_cthd_hd");

            entity.Property(e => e.IdchiTietHoaDon).HasColumnName("IDChiTietHoaDon");
            entity.Property(e => e.DonGia).HasPrecision(12, 2);
            entity.Property(e => e.LoaiDichVu).HasColumnType("enum('kham','xet_nghiem','khac')");
            entity.Property(e => e.MaDichVu).HasMaxLength(30);
            entity.Property(e => e.MaHoaDon).HasMaxLength(30);
            entity.Property(e => e.SoLuong).HasDefaultValueSql("'1'");
            entity.Property(e => e.TenDichVu).HasMaxLength(200);
            entity.Property(e => e.ThanhTien)
                .HasPrecision(12, 2)
                .HasComputedColumnSql("`SoLuong` * `DonGia`", true);

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.Chitiethoadons)
                .HasForeignKey(d => d.MaHoaDon)
                .HasConstraintName("fk_cthd_hd");
        });

        modelBuilder.Entity<Chuyenkhoa>(entity =>
        {
            entity.HasKey(e => e.IdchuyenKhoa).HasName("PRIMARY");

            entity
                .ToTable("chuyenkhoa")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdchuyenKhoa)
                .HasMaxLength(10)
                .HasColumnName("IDChuyenKhoa");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.TenChuyenKhoa).HasMaxLength(128);
        });

        modelBuilder.Entity<Coso>(entity =>
        {
            entity.HasKey(e => e.CoSoId).HasName("PRIMARY");

            entity
                .ToTable("coso")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.CoSoId)
                .HasMaxLength(20)
                .HasColumnName("CoSoID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(191);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenCoSo).HasMaxLength(150);
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','inactive')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<Ctdatlichxetnghiem>(entity =>
        {
            entity.HasKey(e => e.IdctdatLichXn).HasName("PRIMARY");

            entity
                .ToTable("ctdatlichxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdxetNghiem, "fk_ctdlxn_xn");

            entity.HasIndex(e => new { e.IddatLichXn, e.IdxetNghiem }, "uq_ctdlxn").IsUnique();

            entity.Property(e => e.IdctdatLichXn).HasColumnName("IDCTDatLichXN");
            entity.Property(e => e.DonGia).HasPrecision(12, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.IddatLichXn).HasColumnName("IDDatLichXN");
            entity.Property(e => e.IdxetNghiem)
                .HasMaxLength(10)
                .HasColumnName("IDXetNghiem");

            entity.HasOne(d => d.IddatLichXnNavigation).WithMany(p => p.Ctdatlichxetnghiems)
                .HasForeignKey(d => d.IddatLichXn)
                .HasConstraintName("fk_ctdlxn_datlich");

            entity.HasOne(d => d.IdxetNghiemNavigation).WithMany(p => p.Ctdatlichxetnghiems)
                .HasForeignKey(d => d.IdxetNghiem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ctdlxn_xn");
        });

        modelBuilder.Entity<Ctphieuxetnghiem>(entity =>
        {
            entity.HasKey(e => e.Idctphieu).HasName("PRIMARY");

            entity
                .ToTable("ctphieuxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdxetNghiem, "fk_ctphieu_xn");

            entity.HasIndex(e => new { e.IdphieuXetNghiem, e.IdxetNghiem }, "uq_ctphieu_xn").IsUnique();

            entity.Property(e => e.Idctphieu).HasColumnName("IDCTPhieu");
            entity.Property(e => e.DonGia).HasPrecision(12, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.IdphieuXetNghiem)
                .HasMaxLength(20)
                .HasColumnName("IDPhieuXetNghiem");
            entity.Property(e => e.IdxetNghiem)
                .HasMaxLength(10)
                .HasColumnName("IDXetNghiem");
            entity.Property(e => e.SoLuong).HasDefaultValueSql("'1'");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'cho_lay_mau'")
                .HasColumnType("enum('cho_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet','da_duyet','can_lam_lai','hoan_tat','huy')");

            entity.HasOne(d => d.IdphieuXetNghiemNavigation).WithMany(p => p.Ctphieuxetnghiems)
                .HasForeignKey(d => d.IdphieuXetNghiem)
                .HasConstraintName("fk_ctphieu_phieu");

            entity.HasOne(d => d.IdxetNghiemNavigation).WithMany(p => p.Ctphieuxetnghiems)
                .HasForeignKey(d => d.IdxetNghiem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ctphieu_xn");
        });

        modelBuilder.Entity<Datlichkham>(entity =>
        {
            entity.HasKey(e => e.IddatLichKham).HasName("PRIMARY");

            entity
                .ToTable("datlichkham")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MaDatLich, "MaDatLich").IsUnique();

            entity.HasIndex(e => e.MaQr, "MaQR").IsUnique();

            entity.HasIndex(e => e.IdchuyenKhoa, "fk_dlk_chuyenkhoa");

            entity.HasIndex(e => e.CoSoId, "fk_dlk_coso");

            entity.HasIndex(e => e.UserId, "fk_dlk_user");

            entity.HasIndex(e => new { e.IdbacSi, e.NgayKham, e.GioKham, e.TrangThai }, "idx_dlk_bacsi_time");

            entity.HasIndex(e => new { e.IdkhachHang, e.NgayKham }, "idx_dlk_khachhang");

            entity.Property(e => e.IddatLichKham).HasColumnName("IDDatLichKham");
            entity.Property(e => e.CoSoId)
                .HasMaxLength(20)
                .HasColumnName("CoSoID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.GioKham).HasColumnType("time");
            entity.Property(e => e.IdbacSi).HasColumnName("IDBacSi");
            entity.Property(e => e.IdchuyenKhoa)
                .HasMaxLength(10)
                .HasColumnName("IDChuyenKhoa");
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.MaDatLich).HasMaxLength(30);
            entity.Property(e => e.MaQr).HasColumnName("MaQR");
            entity.Property(e => e.StatusMail)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','sent','failed')");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','checked_in','cancelled','no_show','completed')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.CoSo).WithMany(p => p.Datlichkhams)
                .HasForeignKey(d => d.CoSoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dlk_coso");

            entity.HasOne(d => d.IdbacSiNavigation).WithMany(p => p.Datlichkhams)
                .HasForeignKey(d => d.IdbacSi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dlk_bacsi");

            entity.HasOne(d => d.IdchuyenKhoaNavigation).WithMany(p => p.Datlichkhams)
                .HasForeignKey(d => d.IdchuyenKhoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dlk_chuyenkhoa");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.Datlichkhams)
                .HasForeignKey(d => d.IdkhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dlk_khachhang");

            entity.HasOne(d => d.User).WithMany(p => p.Datlichkhams)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_dlk_user");
        });

        modelBuilder.Entity<Datlichxetnghiem>(entity =>
        {
            entity.HasKey(e => e.IddatLichXn).HasName("PRIMARY");

            entity
                .ToTable("datlichxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MaDatLich, "MaDatLich").IsUnique();

            entity.HasIndex(e => e.MaQr, "MaQR").IsUnique();

            entity.HasIndex(e => e.IdbacSi, "fk_dlxn_bacsi");

            entity.HasIndex(e => e.UserId, "fk_dlxn_user");

            entity.HasIndex(e => new { e.IdkhachHang, e.NgayXetNghiem }, "idx_dlxn_khachhang");

            entity.HasIndex(e => new { e.CoSoId, e.NgayXetNghiem, e.GioXetNghiem, e.TrangThai }, "idx_dlxn_time");

            entity.Property(e => e.IddatLichXn).HasColumnName("IDDatLichXN");
            entity.Property(e => e.CoSoId)
                .HasMaxLength(20)
                .HasColumnName("CoSoID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.GioXetNghiem).HasColumnType("time");
            entity.Property(e => e.IdbacSi).HasColumnName("IDBacSi");
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.MaDatLich).HasMaxLength(30);
            entity.Property(e => e.MaQr).HasColumnName("MaQR");
            entity.Property(e => e.StatusMail)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','sent','failed')");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','checked_in','cancelled','no_show','completed')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.CoSo).WithMany(p => p.Datlichxetnghiems)
                .HasForeignKey(d => d.CoSoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dlxn_coso");

            entity.HasOne(d => d.IdbacSiNavigation).WithMany(p => p.Datlichxetnghiems)
                .HasForeignKey(d => d.IdbacSi)
                .HasConstraintName("fk_dlxn_bacsi");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.Datlichxetnghiems)
                .HasForeignKey(d => d.IdkhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dlxn_khachhang");

            entity.HasOne(d => d.User).WithMany(p => p.Datlichxetnghiems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_dlxn_user");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PRIMARY");

            entity
                .ToTable("hoadon")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdkhachHang, "fk_hd_khachhang");

            entity.HasIndex(e => e.IdluotKham, "fk_hd_luotkham");

            entity.HasIndex(e => e.IdphieuXetNghiem, "fk_hd_phieu");

            entity.Property(e => e.MaHoaDon).HasMaxLength(30);
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.IdluotKham).HasColumnName("IDLuotKham");
            entity.Property(e => e.IdphieuXetNghiem)
                .HasMaxLength(20)
                .HasColumnName("IDPhieuXetNghiem");
            entity.Property(e => e.NgayTaoHoaDon)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThuc).HasColumnType("enum('tien_mat','chuyen_khoan','the','khac')");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.TongTien).HasPrecision(12, 2);
            entity.Property(e => e.TrangThaiThanhToan)
                .HasDefaultValueSql("'chua_thanh_toan'")
                .HasColumnType("enum('chua_thanh_toan','da_thanh_toan','hoan_tien','huy')");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.IdkhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_hd_khachhang");

            entity.HasOne(d => d.IdluotKhamNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.IdluotKham)
                .HasConstraintName("fk_hd_luotkham");

            entity.HasOne(d => d.IdphieuXetNghiemNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.IdphieuXetNghiem)
                .HasConstraintName("fk_hd_phieu");
        });

        modelBuilder.Entity<Ketquachiso>(entity =>
        {
            entity.HasKey(e => e.IdketQuaChiSo).HasName("PRIMARY");

            entity
                .ToTable("ketquachiso")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdchiSo, "fk_kqchiso_chiso");

            entity.HasIndex(e => new { e.IdketQua, e.IdchiSo }, "uq_kq_chiso").IsUnique();

            entity.Property(e => e.IdketQuaChiSo).HasColumnName("IDKetQuaChiSo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.DanhGia)
                .HasDefaultValueSql("'chua_danh_gia'")
                .HasColumnType("enum('binh_thuong','thap','cao','bat_thuong','chua_danh_gia')");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GiaTriSo).HasPrecision(15, 4);
            entity.Property(e => e.GiaTriText).HasMaxLength(255);
            entity.Property(e => e.GiaTriThamChieu).HasMaxLength(255);
            entity.Property(e => e.IdchiSo)
                .HasMaxLength(10)
                .HasColumnName("IDChiSo");
            entity.Property(e => e.IdketQua)
                .HasMaxLength(20)
                .HasColumnName("IDKetQua");
            entity.Property(e => e.NguongMaxApDung).HasPrecision(15, 4);
            entity.Property(e => e.NguongMinApDung).HasPrecision(15, 4);
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.IdchiSoNavigation).WithMany(p => p.Ketquachisos)
                .HasForeignKey(d => d.IdchiSo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_kqchiso_chiso");

            entity.HasOne(d => d.IdketQuaNavigation).WithMany(p => p.Ketquachisos)
                .HasForeignKey(d => d.IdketQua)
                .HasConstraintName("fk_kqchiso_kq");
        });

        modelBuilder.Entity<Ketquaxetnghiem>(entity =>
        {
            entity.HasKey(e => e.IdketQua).HasName("PRIMARY");

            entity
                .ToTable("ketquaxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Idctphieu, "IDCTPhieu").IsUnique();

            entity.HasIndex(e => e.IdbacSiDuyet, "fk_kq_bacsi_duyet");

            entity.HasIndex(e => e.Idktv, "fk_kq_ktv");

            entity.HasIndex(e => e.Idmau, "fk_kq_mau");

            entity.HasIndex(e => new { e.TrangThai, e.ThoiGianNhap }, "idx_kq_trangthai");

            entity.Property(e => e.IdketQua)
                .HasMaxLength(20)
                .HasColumnName("IDKetQua");
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.IdbacSiDuyet).HasColumnName("IDBacSiDuyet");
            entity.Property(e => e.Idctphieu).HasColumnName("IDCTPhieu");
            entity.Property(e => e.Idktv)
                .HasMaxLength(10)
                .HasColumnName("IDKTV");
            entity.Property(e => e.Idmau)
                .HasMaxLength(30)
                .HasColumnName("IDMau");
            entity.Property(e => e.KetLuanBacSi).HasColumnType("text");
            entity.Property(e => e.KetQuaTongQuat).HasMaxLength(500);
            entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianDuyet).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianHoanThanh).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianNhap)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'cho_duyet'")
                .HasColumnType("enum('dang_thuc_hien','cho_duyet','da_duyet','can_lam_lai','hoan_tat')");

            entity.HasOne(d => d.IdbacSiDuyetNavigation).WithMany(p => p.Ketquaxetnghiems)
                .HasForeignKey(d => d.IdbacSiDuyet)
                .HasConstraintName("fk_kq_bacsi_duyet");

            entity.HasOne(d => d.IdctphieuNavigation).WithOne(p => p.Ketquaxetnghiem)
                .HasForeignKey<Ketquaxetnghiem>(d => d.Idctphieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_kq_ctphieu");

            entity.HasOne(d => d.IdktvNavigation).WithMany(p => p.Ketquaxetnghiems)
                .HasForeignKey(d => d.Idktv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_kq_ktv");

            entity.HasOne(d => d.IdmauNavigation).WithMany(p => p.Ketquaxetnghiems)
                .HasForeignKey(d => d.Idmau)
                .HasConstraintName("fk_kq_mau");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.IdkhachHang).HasName("PRIMARY");

            entity
                .ToTable("khachhang")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Cccd, "uq_khachhang_cccd").IsUnique();

            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .HasColumnName("CCCD");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(191);
            entity.Property(e => e.GioiTinh).HasColumnType("enum('nam','nu','khac')");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.TenKhachHang).HasMaxLength(120);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<Kham>(entity =>
        {
            entity.HasKey(e => e.Idkham).HasName("PRIMARY");

            entity
                .ToTable("kham")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdluotKham, "IDLuotKham").IsUnique();

            entity.HasIndex(e => e.IdbacSi, "fk_kham_bacsi");

            entity.Property(e => e.Idkham).HasColumnName("IDKham");
            entity.Property(e => e.ChanDoan).HasColumnType("text");
            entity.Property(e => e.HuongDieuTri).HasColumnType("text");
            entity.Property(e => e.IdbacSi).HasColumnName("IDBacSi");
            entity.Property(e => e.IdluotKham).HasColumnName("IDLuotKham");
            entity.Property(e => e.KetLuan).HasColumnType("text");
            entity.Property(e => e.ThoiGianKham)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TienSuBenh).HasColumnType("text");
            entity.Property(e => e.TrieuChung).HasColumnType("text");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.IdbacSiNavigation).WithMany(p => p.Khams)
                .HasForeignKey(d => d.IdbacSi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_kham_bacsi");

            entity.HasOne(d => d.IdluotKhamNavigation).WithOne(p => p.Kham)
                .HasForeignKey<Kham>(d => d.IdluotKham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_kham_luot");
        });

        modelBuilder.Entity<Lichlamviec>(entity =>
        {
            entity.HasKey(e => e.LichId).HasName("PRIMARY");

            entity
                .ToTable("lichlamviec")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.NguoiTaoUserId, "fk_lich_nguoitao");

            entity.HasIndex(e => e.Idphong, "fk_lich_phong");

            entity.HasIndex(e => new { e.IdbacSi, e.Ngay, e.TrangThai }, "idx_lich_bacsi_ngay");

            entity.HasIndex(e => new { e.IdbacSi, e.Ngay, e.GioBatDau, e.GioKetThuc }, "uq_lich_exact").IsUnique();

            entity.Property(e => e.LichId).HasColumnName("LichID");
            entity.Property(e => e.Ca)
                .HasDefaultValueSql("'TuyChinh'")
                .HasColumnType("enum('Sang','Chieu','Toi','TuyChinh')");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GioBatDau).HasColumnType("time");
            entity.Property(e => e.GioKetThuc).HasColumnType("time");
            entity.Property(e => e.IdbacSi).HasColumnName("IDBacSi");
            entity.Property(e => e.Idphong)
                .HasMaxLength(20)
                .HasColumnName("IDPhong");
            entity.Property(e => e.NguoiTaoUserId).HasColumnName("NguoiTaoUserID");
            entity.Property(e => e.NguonTao)
                .HasDefaultValueSql("'bacsi'")
                .HasColumnType("enum('bacsi','admin')");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'duoc_duyet'")
                .HasColumnType("enum('duoc_duyet','huy')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");

            entity.HasOne(d => d.IdbacSiNavigation).WithMany(p => p.Lichlamviecs)
                .HasForeignKey(d => d.IdbacSi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_lich_bacsi");

            entity.HasOne(d => d.IdphongNavigation).WithMany(p => p.Lichlamviecs)
                .HasForeignKey(d => d.Idphong)
                .HasConstraintName("fk_lich_phong");

            entity.HasOne(d => d.NguoiTaoUser).WithMany(p => p.Lichlamviecs)
                .HasForeignKey(d => d.NguoiTaoUserId)
                .HasConstraintName("fk_lich_nguoitao");
        });

        modelBuilder.Entity<Loaixetnghiem>(entity =>
        {
            entity.HasKey(e => e.IdxetNghiem).HasName("PRIMARY");

            entity
                .ToTable("loaixetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ChuyenKhoaId, "fk_loaixn_chuyenkhoa");

            entity.Property(e => e.IdxetNghiem)
                .HasMaxLength(10)
                .HasColumnName("IDXetNghiem");
            entity.Property(e => e.ChuyenKhoaId)
                .HasMaxLength(10)
                .HasColumnName("ChuyenKhoaID");
            entity.Property(e => e.Gia).HasPrecision(12, 2);
            entity.Property(e => e.Loai).HasMaxLength(80);
            entity.Property(e => e.LoaiMauMacDinh).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.TenXetNghiem).HasMaxLength(150);

            entity.HasOne(d => d.ChuyenKhoa).WithMany(p => p.Loaixetnghiems)
                .HasForeignKey(d => d.ChuyenKhoaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_loaixn_chuyenkhoa");
        });

        modelBuilder.Entity<Luotkham>(entity =>
        {
            entity.HasKey(e => e.IdluotKham).HasName("PRIMARY");

            entity
                .ToTable("luotkham")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IddatLichKham, "IDDatLichKham").IsUnique();

            entity.HasIndex(e => e.IdkhachHang, "fk_luotkham_khachhang");

            entity.HasIndex(e => e.Idphong, "fk_luotkham_phong");

            entity.HasIndex(e => new { e.IdbacSi, e.TrangThai, e.SoThuTu }, "idx_luotkham_queue");

            entity.Property(e => e.IdluotKham).HasColumnName("IDLuotKham");
            entity.Property(e => e.IdbacSi).HasColumnName("IDBacSi");
            entity.Property(e => e.IddatLichKham).HasColumnName("IDDatLichKham");
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.Idphong)
                .HasMaxLength(20)
                .HasColumnName("IDPhong");
            entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTiepNhan).HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'da_tiep_nhan'")
                .HasColumnType("enum('da_tiep_nhan','cho_kham','da_den_luot','cho_goi_lai','dang_kham','da_chi_dinh_xn','moi_doc_kq','dang_tu_van_kq','bo_luot','hoan_tat')");

            entity.HasOne(d => d.IdbacSiNavigation).WithMany(p => p.Luotkhams)
                .HasForeignKey(d => d.IdbacSi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_luotkham_bacsi");

            entity.HasOne(d => d.IddatLichKhamNavigation).WithOne(p => p.Luotkham)
                .HasForeignKey<Luotkham>(d => d.IddatLichKham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_luotkham_datlich");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.Luotkhams)
                .HasForeignKey(d => d.IdkhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_luotkham_khachhang");

            entity.HasOne(d => d.IdphongNavigation).WithMany(p => p.Luotkhams)
                .HasForeignKey(d => d.Idphong)
                .HasConstraintName("fk_luotkham_phong");
        });

        modelBuilder.Entity<Luotxetnghiem>(entity =>
        {
            entity.HasKey(e => e.IdluotXetNghiem).HasName("PRIMARY");

            entity
                .ToTable("luotxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IddatLichXn, "IDDatLichXN").IsUnique();

            entity.HasIndex(e => e.IdkhachHang, "fk_luotxn_khachhang");

            entity.HasIndex(e => e.Idphong, "fk_luotxn_phong");

            entity.HasIndex(e => new { e.TrangThai, e.SoThuTu }, "idx_luotxn_queue");

            entity.Property(e => e.IdluotXetNghiem).HasColumnName("IDLuotXetNghiem");
            entity.Property(e => e.IddatLichXn).HasColumnName("IDDatLichXN");
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.Idphong)
                .HasMaxLength(20)
                .HasColumnName("IDPhong");
            entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTiepNhan).HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'da_tiep_nhan'")
                .HasColumnType("enum('da_tiep_nhan','cho_xet_nghiem','da_den_luot','dang_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet_kq','da_co_kq','moi_doc_kq','dang_tu_van_kq','hoan_tat','huy')");

            entity.HasOne(d => d.IddatLichXnNavigation).WithOne(p => p.Luotxetnghiem)
                .HasForeignKey<Luotxetnghiem>(d => d.IddatLichXn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_luotxn_datlich");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.Luotxetnghiems)
                .HasForeignKey(d => d.IdkhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_luotxn_khachhang");

            entity.HasOne(d => d.IdphongNavigation).WithMany(p => p.Luotxetnghiems)
                .HasForeignKey(d => d.Idphong)
                .HasConstraintName("fk_luotxn_phong");
        });

        modelBuilder.Entity<Maubenhpham>(entity =>
        {
            entity.HasKey(e => e.Idmau).HasName("PRIMARY");

            entity
                .ToTable("maubenhpham")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MaBarcode, "MaBarcode").IsUnique();

            entity.HasIndex(e => e.IdbacSiLayMau, "fk_mau_bacsi");

            entity.HasIndex(e => e.Idctphieu, "idx_mau_ctphieu");

            entity.Property(e => e.Idmau)
                .HasMaxLength(30)
                .HasColumnName("IDMau");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.IdbacSiLayMau).HasColumnName("IDBacSiLayMau");
            entity.Property(e => e.Idctphieu).HasColumnName("IDCTPhieu");
            entity.Property(e => e.LoaiMau).HasMaxLength(80);
            entity.Property(e => e.MaBarcode).HasMaxLength(64);
            entity.Property(e => e.ThoiGianLayMau).HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'moi_tao'")
                .HasColumnType("enum('moi_tao','da_lay_mau','da_ban_giao','ktv_tiep_nhan','dang_xu_ly','hoan_tat','tu_choi_mau','huy')");

            entity.HasOne(d => d.IdbacSiLayMauNavigation).WithMany(p => p.Maubenhphams)
                .HasForeignKey(d => d.IdbacSiLayMau)
                .HasConstraintName("fk_mau_bacsi");

            entity.HasOne(d => d.IdctphieuNavigation).WithMany(p => p.Maubenhphams)
                .HasForeignKey(d => d.Idctphieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mau_ctphieu");
        });

        modelBuilder.Entity<Nguongchisoxetnghiem>(entity =>
        {
            entity.HasKey(e => e.Idnguong).HasName("PRIMARY");

            entity
                .ToTable("nguongchisoxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.IdchiSo, e.GioiTinhApDung, e.TuoiMin, e.TuoiMax, e.Status }, "idx_nguong_lookup");

            entity.Property(e => e.Idnguong).HasColumnName("IDNguong");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GiaTriMax).HasPrecision(15, 4);
            entity.Property(e => e.GiaTriMin).HasPrecision(15, 4);
            entity.Property(e => e.GiaTriTextBinhThuong).HasMaxLength(255);
            entity.Property(e => e.GioiTinhApDung)
                .HasDefaultValueSql("'tatca'")
                .HasColumnType("enum('tatca','nam','nu')");
            entity.Property(e => e.IdchiSo)
                .HasMaxLength(10)
                .HasColumnName("IDChiSo");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");

            entity.HasOne(d => d.IdchiSoNavigation).WithMany(p => p.Nguongchisoxetnghiems)
                .HasForeignKey(d => d.IdchiSo)
                .HasConstraintName("fk_nguong_chiso");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.IdnhanVien).HasName("PRIMARY");

            entity
                .ToTable("nhanvien")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CoSoId, "fk_nhanvien_coso");

            entity.Property(e => e.IdnhanVien)
                .HasMaxLength(10)
                .HasColumnName("IDNhanVien");
            entity.Property(e => e.CoSoId)
                .HasMaxLength(20)
                .HasColumnName("CoSoID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Email).HasMaxLength(191);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'yes'")
                .HasColumnType("enum('yes','no')");
            entity.Property(e => e.TenNhanVien).HasMaxLength(120);
            entity.Property(e => e.ViTri).HasColumnType("enum('bacsi','letan','ktv','admin','dieu_duong','khac')");

            entity.HasOne(d => d.CoSo).WithMany(p => p.Nhanviens)
                .HasForeignKey(d => d.CoSoId)
                .HasConstraintName("fk_nhanvien_coso");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("password_resets")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.TokenHash, "TokenHash").IsUnique();

            entity.HasIndex(e => e.UserId, "fk_passwordreset_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_passwordreset_user");
        });

        modelBuilder.Entity<Phieuxetnghiem>(entity =>
        {
            entity.HasKey(e => e.IdphieuXetNghiem).HasName("PRIMARY");

            entity
                .ToTable("phieuxetnghiem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdbacSiChiDinh, "fk_phieu_bacsi_chidinh");

            entity.HasIndex(e => e.IdbacSiPhuTrach, "fk_phieu_bacsi_phutrach");

            entity.HasIndex(e => e.Idkham, "fk_phieu_kham");

            entity.HasIndex(e => e.IdluotXetNghiem, "fk_phieu_luotxn");

            entity.HasIndex(e => new { e.IdkhachHang, e.NgayTao }, "idx_phieu_khachhang");

            entity.Property(e => e.IdphieuXetNghiem)
                .HasMaxLength(20)
                .HasColumnName("IDPhieuXetNghiem");
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.IdbacSiChiDinh).HasColumnName("IDBacSiChiDinh");
            entity.Property(e => e.IdbacSiPhuTrach).HasColumnName("IDBacSiPhuTrach");
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.Idkham).HasColumnName("IDKham");
            entity.Property(e => e.IdluotXetNghiem).HasColumnName("IDLuotXetNghiem");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasPrecision(12, 2);
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'moi_tao'")
                .HasColumnType("enum('moi_tao','cho_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet','da_co_kq','hoan_tat','huy')");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasDefaultValueSql("'chua_thanh_toan'")
                .HasColumnType("enum('chua_thanh_toan','da_thanh_toan','mien_phi')");

            entity.HasOne(d => d.IdbacSiChiDinhNavigation).WithMany(p => p.PhieuxetnghiemIdbacSiChiDinhNavigations)
                .HasForeignKey(d => d.IdbacSiChiDinh)
                .HasConstraintName("fk_phieu_bacsi_chidinh");

            entity.HasOne(d => d.IdbacSiPhuTrachNavigation).WithMany(p => p.PhieuxetnghiemIdbacSiPhuTrachNavigations)
                .HasForeignKey(d => d.IdbacSiPhuTrach)
                .HasConstraintName("fk_phieu_bacsi_phutrach");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.Phieuxetnghiems)
                .HasForeignKey(d => d.IdkhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_phieu_khachhang");

            entity.HasOne(d => d.IdkhamNavigation).WithMany(p => p.Phieuxetnghiems)
                .HasForeignKey(d => d.Idkham)
                .HasConstraintName("fk_phieu_kham");

            entity.HasOne(d => d.IdluotXetNghiemNavigation).WithMany(p => p.Phieuxetnghiems)
                .HasForeignKey(d => d.IdluotXetNghiem)
                .HasConstraintName("fk_phieu_luotxn");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.Idphong).HasName("PRIMARY");

            entity
                .ToTable("phong")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdchuyenKhoa, "fk_phong_chuyenkhoa");

            entity.HasIndex(e => e.CoSoId, "fk_phong_coso");

            entity.Property(e => e.Idphong)
                .HasMaxLength(20)
                .HasColumnName("IDPhong");
            entity.Property(e => e.CoSoId)
                .HasMaxLength(20)
                .HasColumnName("CoSoID");
            entity.Property(e => e.IdchuyenKhoa)
                .HasMaxLength(10)
                .HasColumnName("IDChuyenKhoa");
            entity.Property(e => e.LoaiPhong).HasColumnType("enum('kham','lay_mau','xet_nghiem','tu_van','khac')");
            entity.Property(e => e.Tang).HasMaxLength(20);
            entity.Property(e => e.TenPhong).HasMaxLength(120);
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','inactive','maintenance')");

            entity.HasOne(d => d.CoSo).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.CoSoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_phong_coso");

            entity.HasOne(d => d.IdchuyenKhoaNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.IdchuyenKhoa)
                .HasConstraintName("fk_phong_chuyenkhoa");
        });

        modelBuilder.Entity<Thongbao>(entity =>
        {
            entity.HasKey(e => e.IdthongBao).HasName("PRIMARY");

            entity
                .ToTable("thongbao")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.UserIdnhan, e.DaDoc, e.ThoiGianTao }, "idx_thongbao_user");

            entity.Property(e => e.IdthongBao).HasColumnName("IDThongBao");
            entity.Property(e => e.IddoiTuong)
                .HasMaxLength(50)
                .HasColumnName("IDDoiTuong");
            entity.Property(e => e.LoaiDoiTuong).HasMaxLength(50);
            entity.Property(e => e.LoaiThongBao).HasMaxLength(50);
            entity.Property(e => e.NoiDung).HasColumnType("text");
            entity.Property(e => e.ThoiGianDoc).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TieuDe).HasMaxLength(200);
            entity.Property(e => e.UserIdnhan).HasColumnName("UserIDNhan");

            entity.HasOne(d => d.UserIdnhanNavigation).WithMany(p => p.Thongbaos)
                .HasForeignKey(d => d.UserIdnhan)
                .HasConstraintName("fk_thongbao_user");
        });

        modelBuilder.Entity<Truyvet>(entity =>
        {
            entity.HasKey(e => e.IdtruyVet).HasName("PRIMARY");

            entity
                .ToTable("truyvet")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UserIdthucHien, "fk_truyvet_user");

            entity.HasIndex(e => new { e.LoaiDoiTuong, e.IddoiTuong, e.ThoiGian }, "idx_truyvet_doituong");

            entity.HasIndex(e => new { e.IdkhachHang, e.ThoiGian }, "idx_truyvet_khachhang");

            entity.Property(e => e.IdtruyVet).HasColumnName("IDTruyVet");
            entity.Property(e => e.HanhDong).HasMaxLength(150);
            entity.Property(e => e.IddoiTuong)
                .HasMaxLength(50)
                .HasColumnName("IDDoiTuong");
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LoaiDoiTuong).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NguonThucHien)
                .HasDefaultValueSql("'user'")
                .HasColumnType("enum('user','system')");
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiCu).HasMaxLength(50);
            entity.Property(e => e.TrangThaiMoi).HasMaxLength(50);
            entity.Property(e => e.UserIdthucHien).HasColumnName("UserIDThucHien");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.Truyvets)
                .HasForeignKey(d => d.IdkhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_truyvet_khachhang");

            entity.HasOne(d => d.UserIdthucHienNavigation).WithMany(p => p.Truyvets)
                .HasForeignKey(d => d.UserIdthucHien)
                .HasConstraintName("fk_truyvet_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.IdbacSi, "IDBacSi").IsUnique();

            entity.HasIndex(e => e.IdkhachHang, "IDKhachHang").IsUnique();

            entity.HasIndex(e => e.IdnhanVien, "IDNhanVien").IsUnique();

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.IdbacSi).HasColumnName("IDBacSi");
            entity.Property(e => e.IdkhachHang)
                .HasMaxLength(10)
                .HasColumnName("IDKhachHang");
            entity.Property(e => e.IdnhanVien)
                .HasMaxLength(10)
                .HasColumnName("IDNhanVien");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasColumnType("enum('khachhang','bacsi','letan','ktv','admin')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.IdbacSiNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.IdbacSi)
                .HasConstraintName("fk_users_bacsi");

            entity.HasOne(d => d.IdkhachHangNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.IdkhachHang)
                .HasConstraintName("fk_users_khachhang");

            entity.HasOne(d => d.IdnhanVienNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.IdnhanVien)
                .HasConstraintName("fk_users_nhanvien");
        });

        modelBuilder.Entity<Worklist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("worklist")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Idctphieu, "fk_worklist_ctphieu");

            entity.HasIndex(e => e.Idktv, "fk_worklist_ktv");

            entity.HasIndex(e => e.Idmau, "fk_worklist_mau");

            entity.HasIndex(e => new { e.Status, e.ReceivedAt }, "idx_worklist_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FinishedAt).HasColumnType("datetime");
            entity.Property(e => e.Idctphieu).HasColumnName("IDCTPhieu");
            entity.Property(e => e.Idktv)
                .HasMaxLength(10)
                .HasColumnName("IDKTV");
            entity.Property(e => e.Idmau)
                .HasMaxLength(30)
                .HasColumnName("IDMau");
            entity.Property(e => e.Instrument).HasMaxLength(120);
            entity.Property(e => e.ReagentLot).HasMaxLength(64);
            entity.Property(e => e.ReceivedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.StartedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'queue'")
                .HasColumnType("enum('queue','running','to_result','rerun','finished','cancelled')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdctphieuNavigation).WithMany(p => p.Worklists)
                .HasForeignKey(d => d.Idctphieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_worklist_ctphieu");

            entity.HasOne(d => d.IdktvNavigation).WithMany(p => p.Worklists)
                .HasForeignKey(d => d.Idktv)
                .HasConstraintName("fk_worklist_ktv");

            entity.HasOne(d => d.IdmauNavigation).WithMany(p => p.Worklists)
                .HasForeignKey(d => d.Idmau)
                .HasConstraintName("fk_worklist_mau");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
