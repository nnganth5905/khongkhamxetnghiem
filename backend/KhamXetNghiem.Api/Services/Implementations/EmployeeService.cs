using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class EmployeeService
    : IEmployeeService
{
    private readonly IEmployeeRepository
        _employeeRepository;

    public EmployeeService(
        IEmployeeRepository employeeRepository
    )
    {
        _employeeRepository =
            employeeRepository;
    }

    // =====================================================
    // LIST
    // =====================================================

    public async Task<List<EmployeeResponse>>
        GetAllEmployeesAsync(
            string? query,
            string? role,
            CancellationToken cancellationToken = default
        )
    {
        string? dbRole = null;

        if (
            !string.IsNullOrWhiteSpace(
                role
            )
        )
        {
            dbRole =
                NormalizeRoleToDatabase(
                    role
                );
        }

        var employees =
            await _employeeRepository
                .GetAllAsync(
                    query,
                    dbRole,
                    cancellationToken
                );

        return employees
            .Select(
                MapToResponse
            )
            .ToList();
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<EmployeeResponse>
        GetEmployeeByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
    {
        var employee =
            await _employeeRepository
                .FindByIdAsync(
                    id,
                    cancellationToken
                );

        if (employee is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy nhân viên với mã: {id}"
            );
        }

        return MapToResponse(
            employee
        );
    }

    // =====================================================
    // CREATE
    // =====================================================

    public async Task<EmployeeResponse>
        CreateEmployeeAsync(
            EmployeeRequest request,
            CancellationToken cancellationToken = default
        )
    {
        ValidateRequest(
            request
        );

        var facilityId =
            NormalizeNullable(
                request.IdCoSo
            );

        await ValidateFacilityAsync(
            facilityId,
            cancellationToken
        );

        var employeeId =
            await GenerateEmployeeIdAsync(
                cancellationToken
            );

        var employee =
            new Employee
            {
                Id =
                    employeeId,

                FullName =
                    request.HoTen
                        .Trim(),

                Position =
                    NormalizeRoleToDatabase(
                        request.VaiTro
                    ),

                Phone =
                    NormalizeNullable(
                        request.SoDienThoai
                    ),

                Email =
                    NormalizeEmail(
                        request.Email
                    ),

                FacilityId =
                    facilityId,

                Status =
                    NormalizeStatusToDatabase(
                        request.TrangThai
                    )
            };

        await _employeeRepository
            .AddAsync(
                employee,
                cancellationToken
            );

        await _employeeRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return MapToResponse(
            employee
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task<EmployeeResponse>
        UpdateEmployeeAsync(
            string id,
            EmployeeRequest request,
            CancellationToken cancellationToken = default
        )
    {
        ValidateRequest(
            request
        );

        var employee =
            await _employeeRepository
                .FindTrackedByIdAsync(
                    id,
                    cancellationToken
                );

        if (employee is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy nhân viên với mã: {id}"
            );
        }

        var facilityId =
            NormalizeNullable(
                request.IdCoSo
            );

        await ValidateFacilityAsync(
            facilityId,
            cancellationToken
        );

        var newStatus =
            NormalizeStatusToDatabase(
                request.TrangThai
            );

        employee.FullName =
            request.HoTen
                .Trim();

        employee.Position =
            NormalizeRoleToDatabase(
                request.VaiTro
            );

        employee.Phone =
            NormalizeNullable(
                request.SoDienThoai
            );

        employee.Email =
            NormalizeEmail(
                request.Email
            );

        employee.FacilityId =
            facilityId;

        employee.Status =
            newStatus;

        /*
         * Nếu admin khóa nhân viên từ form update,
         * account liên kết cũng phải bị khóa.
         */
        if (
            newStatus == "no"
        )
        {
            await _employeeRepository
                .DisableLinkedAccountAsync(
                    employee.Id,
                    cancellationToken
                );
        }

        await _employeeRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return MapToResponse(
            employee
        );
    }

    // =====================================================
    // DELETE = SOFT DELETE
    // =====================================================

    public async Task DeleteEmployeeAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var employee =
            await _employeeRepository
                .FindTrackedByIdAsync(
                    id,
                    cancellationToken
                );

        if (employee is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy nhân viên với mã: {id}"
            );
        }

        /*
         * Không DELETE record vật lý.
         */
        employee.Status =
            "no";

        /*
         * Nếu có account đăng nhập:
         * khóa luôn account.
         */
        await _employeeRepository
            .DisableLinkedAccountAsync(
                employee.Id,
                cancellationToken
            );

        await _employeeRepository
            .SaveChangesAsync(
                cancellationToken
            );
    }

    // =====================================================
    // FACILITY
    // =====================================================

    private async Task ValidateFacilityAsync(
        string? facilityId,
        CancellationToken cancellationToken
    )
    {
        if (
            facilityId is null
        )
        {
            return;
        }

        var exists =
            await _employeeRepository
                .FacilityExistsAsync(
                    facilityId,
                    cancellationToken
                );

        if (!exists)
        {
            throw new BadRequestException(
                $"Cơ sở {facilityId} không tồn tại."
            );
        }
    }

    // =====================================================
    // VALIDATE
    // =====================================================

    private static void ValidateRequest(
        EmployeeRequest request
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                request.HoTen
            )
        )
        {
            throw new BadRequestException(
                "Họ tên không được để trống."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                request.VaiTro
            )
        )
        {
            throw new BadRequestException(
                "Vai trò không được để trống."
            );
        }
    }

    // =====================================================
    // GENERATE ID
    // =====================================================

    private async Task<string>
        GenerateEmployeeIdAsync(
            CancellationToken cancellationToken
        )
    {
        /*
         * IDNhanVien varchar(10)
         * NV + 8 ký tự = 10.
         */
        for (
            var attempt = 0;
            attempt < 20;
            attempt++
        )
        {
            var id =
                "NV"
                +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            var exists =
                await _employeeRepository
                    .ExistsByIdAsync(
                        id,
                        cancellationToken
                    );

            if (!exists)
            {
                return id;
            }
        }

        throw new InvalidOperationException(
            "Không thể tạo mã nhân viên duy nhất."
        );
    }

    // =====================================================
    // ROLE
    // =====================================================

    public static string NormalizeRoleToDatabase(
        string role
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                role
            )
        )
        {
            throw new BadRequestException(
                "Vai trò không được để trống."
            );
        }

        var normalized =
            role
                .Trim()
                .ToUpperInvariant()
                .Replace(
                    "-",
                    "_"
                )
                .Replace(
                    " ",
                    "_"
                );

        return normalized switch
        {
            "BACSI" or
            "DOCTOR" =>
                "bacsi",

            "LETAN" or
            "RECEPTION" or
            "RECEPTIONIST" =>
                "letan",

            "KTV" or
            "TECHNICIAN" =>
                "ktv",

            "ADMIN" =>
                "admin",

            "DIEU_DUONG" or
            "DIEUDUONG" or
            "NURSE" =>
                "dieu_duong",

            "KHAC" or
            "STAFF" or
            "EMPLOYEE" =>
                "khac",

            _ =>
                throw new BadRequestException(
                    $"Vai trò '{role}' không hợp lệ."
                )
        };
    }

    public static string MapDatabaseRoleToApi(
        string? role
    )
    {
        return role
            ?.Trim()
            .ToLowerInvariant()
        switch
        {
            "bacsi" =>
                "DOCTOR",

            "letan" =>
                "RECEPTIONIST",

            "ktv" =>
                "TECHNICIAN",

            "admin" =>
                "ADMIN",

            "dieu_duong" =>
                "NURSE",

            "khac" =>
                "STAFF",

            _ =>
                "STAFF"
        };
    }

    // =====================================================
    // STATUS
    // =====================================================

    public static string NormalizeStatusToDatabase(
        string? status
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                status
            )
        )
        {
            return "yes";
        }

        var normalized =
            status
                .Trim()
                .ToUpperInvariant();

        return normalized switch
        {
            "YES" or
            "ACTIVE" =>
                "yes",

            "NO" or
            "INACTIVE" =>
                "no",

            _ =>
                throw new BadRequestException(
                    $"Trạng thái '{status}' không hợp lệ."
                )
        };
    }

    public static string MapDatabaseStatusToApi(
        string? status
    )
    {
        return string.Equals(
            status,
            "no",
            StringComparison.OrdinalIgnoreCase
        )
            ? "INACTIVE"
            : "ACTIVE";
    }

    // =====================================================
    // STRING
    // =====================================================

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

    private static string? NormalizeEmail(
        string? value
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                value
            )
        )
        {
            return null;
        }

        return value
            .Trim()
            .ToLowerInvariant();
    }

    // =====================================================
    // MAP
    // =====================================================

    private static EmployeeResponse MapToResponse(
        Employee employee
    )
    {
        return new EmployeeResponse
        {
            Id =
                employee.Id,

            HoTen =
                employee.FullName,

            VaiTro =
                MapDatabaseRoleToApi(
                    employee.Position
                ),

            Position =
                employee.Position,

            SoDienThoai =
                employee.Phone,

            Email =
                employee.Email,

            IdCoSo =
                employee.FacilityId,

            TrangThai =
                MapDatabaseStatusToApi(
                    employee.Status
                ),

            Status =
                employee.Status,

            CreatedAt =
                employee.CreatedAt
        };
    }
}