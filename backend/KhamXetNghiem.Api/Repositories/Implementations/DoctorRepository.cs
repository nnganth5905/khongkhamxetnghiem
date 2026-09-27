using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class DoctorRepository
    : IDoctorRepository
{
    private readonly AppDbContext _dbContext;

    public DoctorRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // LIST
    // =====================================================

    public async Task<List<DoctorResponse>> GetAllAsync(
        string? specialtyId,
        string? query,
        decimal? minimumStars,
        bool activeOnly,
        CancellationToken cancellationToken = default
    )
    {
        var doctors =
            _dbContext.Doctors
                .AsNoTracking()
                .AsQueryable();

        if (activeOnly)
        {
            doctors =
                doctors.Where(
                    x =>
                        x.Status == "active"
                );
        }

        if (
            !string.IsNullOrWhiteSpace(
                specialtyId
            )
        )
        {
            var specialty =
                specialtyId.Trim();

            doctors =
                doctors.Where(
                    x =>
                        x.SpecialtyId ==
                        specialty
                );
        }

        if (
            minimumStars.HasValue &&
            minimumStars.Value > 0
        )
        {
            doctors =
                doctors.Where(
                    x =>
                        x.Rating >=
                        minimumStars.Value
                );
        }

        var resultQuery =
            from doctor in doctors

            join specialtyBase
                in _dbContext.Specialties.AsNoTracking()
                on doctor.SpecialtyId
                equals specialtyBase.Id
                into specialtyGroup

            from specialty
                in specialtyGroup.DefaultIfEmpty()

            join employeeBase
                in _dbContext.Employees.AsNoTracking()
                on doctor.EmployeeId
                equals employeeBase.Id
                into employeeGroup

            from employee
                in employeeGroup.DefaultIfEmpty()

            join accountBase
                in _dbContext.Accounts.AsNoTracking()
                on (int?)doctor.Id
                equals accountBase.IdBacSi
                into accountGroup

            from account
                in accountGroup.DefaultIfEmpty()

            select new
            {
                Doctor = doctor,

                SpecialtyName =
                    specialty != null
                        ? specialty.Name
                        : null,

                EmployeePhone =
                    employee != null
                        ? employee.Phone
                        : null,

                EmployeeEmail =
                    employee != null
                        ? employee.Email
                        : null,

                AccountEmail =
                    account != null
                        ? account.Email
                        : null
            };

        if (
            !string.IsNullOrWhiteSpace(
                query
            )
        )
        {
            var keyword =
                query.Trim();

            resultQuery =
                resultQuery.Where(
                    x =>
                        x.Doctor.Name.Contains(
                            keyword
                        )
                        ||
                        (
                            x.Doctor.Degree != null
                            &&
                            x.Doctor.Degree.Contains(
                                keyword
                            )
                        )
                        ||
                        (
                            x.Doctor.Title != null
                            &&
                            x.Doctor.Title.Contains(
                                keyword
                            )
                        )
                        ||
                        (
                            x.SpecialtyName != null
                            &&
                            x.SpecialtyName.Contains(
                                keyword
                            )
                        )
                );
        }

        return await resultQuery
            .OrderBy(
                x =>
                    x.Doctor.Id
            )
            .Select(
                x =>
                    new DoctorResponse
                    {
                        Id =
                            x.Doctor.Id,

                        HoTen =
                            x.Doctor.Name,

                        IdChuyenKhoa =
                            x.Doctor.SpecialtyId,

                        TenChuyenKhoa =
                            x.SpecialtyName,

                        CoSoId =
                            x.Doctor.FacilityId,

                        HocVi =
                            x.Doctor.Degree,

                        ChucDanh =
                            x.Doctor.Title,

                        SoDienThoai =
                            x.EmployeePhone,

                        Email =
                            x.EmployeeEmail
                            ??
                            x.AccountEmail,

                        HinhAnh =
                            x.Doctor.ImageUrl,

                        GioiThieu =
                            x.Doctor.Bio,

                        TrangThai =
                            x.Doctor.Status,

                        IdNhanVien =
                            x.Doctor.EmployeeId,

                        SoSao =
                            x.Doctor.Rating,

                        NamKinhNghiem =
                            x.Doctor.ExperienceYears
                    }
            )
            .ToListAsync(
                cancellationToken
            );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<DoctorResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return (
            await GetAllAsync(
                null,
                null,
                null,
                false,
                cancellationToken
            )
        )
        .FirstOrDefault(
            x =>
                x.Id == id
        );
    }

    // =====================================================
    // CREATE
    // =====================================================

    public async Task<DoctorResponse> CreateAsync(
        DoctorRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await ValidateReferencesAsync(
            request,
            cancellationToken
        );

        var doctor =
            new Doctor
            {
                Name =
                    request.HoTen.Trim(),

                SpecialtyId =
                    request.IdChuyenKhoa.Trim(),

                Degree =
                    NormalizeNullable(
                        request.HocVi
                    ),

                Title =
                    NormalizeNullable(
                        request.ChucDanh
                    ),

                FacilityId =
                    NormalizeNullable(
                        request.CoSoId
                    ),

                ImageUrl =
                    NormalizeNullable(
                        request.HinhAnh
                    ),

                Bio =
                    NormalizeNullable(
                        request.GioiThieu
                    ),

                ExperienceYears =
                    request.NamKinhNghiem,

                Status =
                    NormalizeStatus(
                        request.TrangThai
                    )
            };

        await _dbContext.Doctors
            .AddAsync(
                doctor,
                cancellationToken
            );

        await _dbContext
            .SaveChangesAsync(
                cancellationToken
            );

        return (
            await GetByIdAsync(
                doctor.Id,
                cancellationToken
            )
        )
        ?? throw new InvalidOperationException(
            "Không đọc được bác sĩ vừa tạo."
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task<DoctorResponse> UpdateAsync(
        int id,
        DoctorRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await ValidateReferencesAsync(
            request,
            cancellationToken
        );

        var doctor =
            await _dbContext.Doctors
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id,
                    cancellationToken
                );

        if (doctor is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy bác sĩ với mã: {id}"
            );
        }

        doctor.Name =
            request.HoTen.Trim();

        doctor.SpecialtyId =
            request.IdChuyenKhoa.Trim();

        doctor.Degree =
            NormalizeNullable(
                request.HocVi
            );

        doctor.Title =
            NormalizeNullable(
                request.ChucDanh
            );

        doctor.FacilityId =
            NormalizeNullable(
                request.CoSoId
            );

        doctor.ImageUrl =
            NormalizeNullable(
                request.HinhAnh
            );

        doctor.Bio =
            NormalizeNullable(
                request.GioiThieu
            );

        doctor.ExperienceYears =
            request.NamKinhNghiem;

        doctor.Status =
            NormalizeStatus(
                request.TrangThai
            );

        await _dbContext
            .SaveChangesAsync(
                cancellationToken
            );

        return (
            await GetByIdAsync(
                id,
                cancellationToken
            )
        )
        ?? throw new ResourceNotFoundException(
            $"Không tìm thấy bác sĩ với mã: {id}"
        );
    }

    // =====================================================
    // DELETE = SOFT DELETE
    // =====================================================

    public async Task SoftDeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var doctor =
            await _dbContext.Doctors
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id,
                    cancellationToken
                );

        if (doctor is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy bác sĩ với mã: {id}"
            );
        }

        doctor.Status =
            "inactive";

        await _dbContext
            .SaveChangesAsync(
                cancellationToken
            );
    }

    // =====================================================
    // VALIDATE FOREIGN KEYS
    // =====================================================

    private async Task ValidateReferencesAsync(
        DoctorRequest request,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                request.HoTen
            )
        )
        {
            throw new BadRequestException(
                "Họ tên bác sĩ không được để trống."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                request.IdChuyenKhoa
            )
        )
        {
            throw new BadRequestException(
                "Chuyên khoa không được để trống."
            );
        }

        var specialtyId =
            request.IdChuyenKhoa.Trim();

        var specialtyExists =
            await _dbContext.Specialties
                .AnyAsync(
                    x =>
                        x.Id == specialtyId
                        &&
                        x.Status == "yes",
                    cancellationToken
                );

        if (!specialtyExists)
        {
            throw new BadRequestException(
                $"Chuyên khoa {specialtyId} không tồn tại hoặc đã ngừng hoạt động."
            );
        }

        var facilityId =
            NormalizeNullable(
                request.CoSoId
            );

        if (
            facilityId is not null
        )
        {
            var facilityExists =
                await _dbContext.Facilities
                    .AnyAsync(
                        x =>
                            x.Id == facilityId,
                        cancellationToken
                    );

            if (!facilityExists)
            {
                throw new BadRequestException(
                    $"Cơ sở {facilityId} không tồn tại."
                );
            }
        }
    }

    private static string NormalizeStatus(
        string? value
    )
    {
        return string.Equals(
            value?.Trim(),
            "inactive",
            StringComparison.OrdinalIgnoreCase
        )
            ? "inactive"
            : "active";
    }

    private static string? NormalizeNullable(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(
            value
        )
            ? null
            : value.Trim();
    }
}