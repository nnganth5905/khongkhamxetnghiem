using KhamXetNghiem.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Data;

public sealed class AppDbContext
    : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Account> Accounts =>
        Set<Account>();

    public DbSet<Customer> Customers =>
        Set<Customer>();

    public DbSet<Doctor> Doctors =>
        Set<Doctor>();

    public DbSet<Specialty> Specialties =>
        Set<Specialty>();

    public DbSet<Employee> Employees =>
        Set<Employee>();

    public DbSet<Facility> Facilities =>
        Set<Facility>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(
            modelBuilder
        );

        ConfigureAccount(
            modelBuilder
        );

        ConfigureCustomer(
            modelBuilder
        );

        ConfigureDoctor(
            modelBuilder
        );

        ConfigureSpecialty(
            modelBuilder
        );

        ConfigureEmployee(
            modelBuilder
        );

        ConfigureFacility(
            modelBuilder
        );
    }

    // =====================================================
    // ACCOUNT
    // =====================================================

    private static void ConfigureAccount(
        ModelBuilder modelBuilder
    )
    {
        var entity =
            modelBuilder
                .Entity<Account>();

        entity.ToTable(
            "users"
        );

        entity.HasKey(
            x => x.UserId
        );

        entity.Property(
                x => x.UserId
            )
            .HasColumnName(
                "UserID"
            )
            .ValueGeneratedOnAdd();

        entity.Property(
                x => x.Email
            )
            .HasColumnName(
                "Email"
            )
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(
                x => x.Username
            )
            .HasColumnName(
                "Username"
            )
            .HasMaxLength(100);

        entity.Property(
                x => x.PasswordHash
            )
            .HasColumnName(
                "PasswordHash"
            )
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(
                x => x.Role
            )
            .HasColumnName(
                "Role"
            )
            .IsRequired();

        entity.Property(
                x => x.IdKhachHang
            )
            .HasColumnName(
                "IDKhachHang"
            )
            .HasMaxLength(10);

        entity.Property(
                x => x.IdBacSi
            )
            .HasColumnName(
                "IDBacSi"
            );

        entity.Property(
                x => x.IdNhanVien
            )
            .HasColumnName(
                "IDNhanVien"
            )
            .HasMaxLength(10);

        entity.Property(
                x => x.IsActive
            )
            .HasColumnName(
                "IsActive"
            )
            .IsRequired();

        entity.Property(
                x => x.CreatedAt
            )
            .HasColumnName(
                "CreatedAt"
            )
            .HasColumnType(
                "timestamp"
            )
            .ValueGeneratedOnAdd();

        entity.Property(
                x => x.UpdatedAt
            )
            .HasColumnName(
                "UpdatedAt"
            )
            .HasColumnType(
                "timestamp"
            )
            .ValueGeneratedOnAddOrUpdate();

        entity.HasIndex(
                x => x.Email
            )
            .IsUnique();

        entity.HasIndex(
                x => x.Username
            )
            .IsUnique();

        entity.HasIndex(
                x => x.IdKhachHang
            )
            .IsUnique();

        entity.HasIndex(
                x => x.IdBacSi
            )
            .IsUnique();

        entity.HasIndex(
                x => x.IdNhanVien
            )
            .IsUnique();
    }

    // =====================================================
    // CUSTOMER
    // =====================================================

    private static void ConfigureCustomer(
        ModelBuilder modelBuilder
    )
    {
        var entity =
            modelBuilder
                .Entity<Customer>();

        entity.ToTable(
            "khachhang"
        );

        entity.HasKey(
            x => x.Id
        );

        entity.Property(
                x => x.Id
            )
            .HasColumnName(
                "IDKhachHang"
            )
            .HasMaxLength(10)
            .ValueGeneratedNever();

        entity.Property(
                x => x.FullName
            )
            .HasColumnName(
                "TenKhachHang"
            )
            .HasMaxLength(120)
            .IsRequired();

        entity.Property(
                x => x.BirthDate
            )
            .HasColumnName(
                "NgaySinh"
            )
            .HasColumnType(
                "date"
            );

        entity.Property(
                x => x.Phone
            )
            .HasColumnName(
                "SoDienThoai"
            )
            .HasMaxLength(20);

        entity.Property(
                x => x.Gender
            )
            .HasColumnName(
                "GioiTinh"
            );

        entity.Property(
                x => x.CitizenId
            )
            .HasColumnName(
                "CCCD"
            )
            .HasMaxLength(20);

        entity.Property(
                x => x.Address
            )
            .HasColumnName(
                "DiaChi"
            )
            .HasMaxLength(255);

        entity.Property(
                x => x.Email
            )
            .HasColumnName(
                "Email"
            )
            .HasMaxLength(191);

        entity.Property(
                x => x.Status
            )
            .HasColumnName(
                "Status"
            )
            .IsRequired();

        entity.Property(
                x => x.CreatedAt
            )
            .HasColumnName(
                "CreatedAt"
            )
            .HasColumnType(
                "timestamp"
            )
            .ValueGeneratedOnAdd();

        entity.Property(
                x => x.UpdatedAt
            )
            .HasColumnName(
                "UpdatedAt"
            )
            .HasColumnType(
                "timestamp"
            )
            .ValueGeneratedOnAddOrUpdate();

        entity.HasIndex(
                x => x.CitizenId
            )
            .IsUnique();
    }

    // =====================================================
    // DOCTOR
    // =====================================================

    private static void ConfigureDoctor(
        ModelBuilder modelBuilder
    )
    {
        var entity =
            modelBuilder
                .Entity<Doctor>();

        entity.ToTable(
            "bacsi"
        );

        entity.HasKey(
            x => x.Id
        );

        entity.Property(
                x => x.Id
            )
            .HasColumnName(
                "IDBacSi"
            )
            .ValueGeneratedOnAdd();

        entity.Property(
                x => x.Name
            )
            .HasColumnName(
                "TenBacSi"
            )
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(
                x => x.Degree
            )
            .HasColumnName(
                "HocVi"
            )
            .HasMaxLength(100);

        entity.Property(
                x => x.Title
            )
            .HasColumnName(
                "ChucDanh"
            )
            .HasMaxLength(150);

        entity.Property(
                x => x.SpecialtyId
            )
            .HasColumnName(
                "KhoaID"
            )
            .HasMaxLength(10)
            .IsRequired();

        entity.Property(
                x => x.FacilityId
            )
            .HasColumnName(
                "CoSoID"
            )
            .HasMaxLength(20);

        entity.Property(
                x => x.Bio
            )
            .HasColumnName(
                "MoTa"
            );

        entity.Property(
                x => x.ImageUrl
            )
            .HasColumnName(
                "HinhAnh"
            )
            .HasMaxLength(500);

        entity.Property(
                x => x.Rating
            )
            .HasColumnName(
                "SoSao"
            )
            .HasPrecision(
                2,
                1
            );

        entity.Property(
                x => x.ExperienceYears
            )
            .HasColumnName(
                "NamKinhNghiem"
            );

        entity.Property(
                x => x.Status
            )
            .HasColumnName(
                "TrangThai"
            )
            .IsRequired();

        entity.Property(
                x => x.EmployeeId
            )
            .HasColumnName(
                "IDNhanVien"
            )
            .HasMaxLength(10);

        entity.HasIndex(
                x => x.EmployeeId
            )
            .IsUnique();
    }

    // =====================================================
    // SPECIALTY
    // =====================================================

    private static void ConfigureSpecialty(
        ModelBuilder modelBuilder
    )
    {
        var entity =
            modelBuilder
                .Entity<Specialty>();

        entity.ToTable(
            "chuyenkhoa"
        );

        entity.HasKey(
            x => x.Id
        );
    }

    // =====================================================
    // EMPLOYEE
    // =====================================================

    private static void ConfigureEmployee(
        ModelBuilder modelBuilder
    )
    {
        var entity =
            modelBuilder
                .Entity<Employee>();

        entity.ToTable(
            "nhanvien"
        );

        entity.HasKey(
            x => x.Id
        );
    }

    // =====================================================
    // FACILITY
    // =====================================================

    private static void ConfigureFacility(
        ModelBuilder modelBuilder
    )
    {
        var entity =
            modelBuilder
                .Entity<Facility>();

        entity.ToTable(
            "coso"
        );

        entity.HasKey(
            x => x.Id
        );
    }
}