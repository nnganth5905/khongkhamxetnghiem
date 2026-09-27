using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class TechnicianService
    : ITechnicianService
{
    private readonly IEmployeeRepository
        _employeeRepository;

    private readonly ITechnicianWorkflowRepository
        _workflowRepository;

    public TechnicianService(
        IEmployeeRepository employeeRepository,
        ITechnicianWorkflowRepository workflowRepository
    )
    {
        _employeeRepository =
            employeeRepository;

        _workflowRepository =
            workflowRepository;
    }

    // =====================================================
    // CURRENT TECHNICIAN
    // =====================================================

    public async Task<CurrentTechnicianResponse>
        GetCurrentTechnicianAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        return new CurrentTechnicianResponse
        {
            Authenticated = true,
            UserId = technician.UserId,
            IdNhanVien = technician.EmployeeId,
            HoTen = technician.FullName,
            Email = technician.Email,
            SoDienThoai = technician.Phone,
            IdCoSo = technician.FacilityId
        };
    }

    // =====================================================
    // ADMIN LIST
    // =====================================================

    public async Task<List<TechnicianResponse>>
        GetTechniciansAsync(
            string? query,
            CancellationToken cancellationToken = default
        )
    {
        var employees =
            await _employeeRepository
                .GetAllAsync(
                    query,
                    "ktv",
                    cancellationToken
                );

        return employees
            .Select(MapTechnician)
            .ToList();
    }

    // =====================================================
    // ADMIN DETAIL
    // =====================================================

    public async Task<TechnicianResponse>
        GetTechnicianByIdAsync(
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

        if (
            employee is null
            ||
            employee.Position != "ktv"
        )
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy kỹ thuật viên với mã: {id}"
            );
        }

        return MapTechnician(
            employee
        );
    }

    // =====================================================
    // ADMIN CREATE
    // =====================================================

    public async Task<TechnicianResponse>
        CreateTechnicianAsync(
            TechnicianRequest request,
            CancellationToken cancellationToken = default
        )
    {
        ValidateTechnician(
            request
        );

        var facilityId =
            NormalizeNullable(
                request.IdCoSo
            );

        if (
            facilityId is not null
            &&
            !await _employeeRepository
                .FacilityExistsAsync(
                    facilityId,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Cơ sở {facilityId} không tồn tại."
            );
        }

        var id =
            await GenerateEmployeeIdAsync(
                cancellationToken
            );

        var employee =
            new Employee
            {
                Id = id,

                FullName =
                    request.HoTen.Trim(),

                Position =
                    "ktv",

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
                    NormalizeStatus(
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

        return MapTechnician(
            employee
        );
    }

    // =====================================================
    // ADMIN UPDATE
    // =====================================================

    public async Task<TechnicianResponse>
        UpdateTechnicianAsync(
            string id,
            TechnicianRequest request,
            CancellationToken cancellationToken = default
        )
    {
        ValidateTechnician(
            request
        );

        var employee =
            await _employeeRepository
                .FindTrackedByIdAsync(
                    id,
                    cancellationToken
                );

        if (
            employee is null
            ||
            employee.Position != "ktv"
        )
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy kỹ thuật viên với mã: {id}"
            );
        }

        var facilityId =
            NormalizeNullable(
                request.IdCoSo
            );

        if (
            facilityId is not null
            &&
            !await _employeeRepository
                .FacilityExistsAsync(
                    facilityId,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Cơ sở {facilityId} không tồn tại."
            );
        }

        var status =
            NormalizeStatus(
                request.TrangThai
            );

        employee.FullName =
            request.HoTen.Trim();

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
            status;

        if (status == "no")
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

        return MapTechnician(
            employee
        );
    }

    // =====================================================
    // ADMIN DELETE
    // =====================================================

    public async Task DeleteTechnicianAsync(
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

        if (
            employee is null
            ||
            employee.Position != "ktv"
        )
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy kỹ thuật viên với mã: {id}"
            );
        }

        employee.Status =
            "no";

        await _employeeRepository
            .DisableLinkedAccountAsync(
                id,
                cancellationToken
            );

        await _employeeRepository
            .SaveChangesAsync(
                cancellationToken
            );
    }

    // =====================================================
    // SPECIMENS
    // =====================================================

    public async Task<List<SpecimenListItemResponse>>
        GetSpecimensAsync(
            string? status,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        await RequireTechnicianAsync(
            user,
            cancellationToken
        );

        return await _workflowRepository
            .GetSpecimensAsync(
                status,
                cancellationToken
            );
    }

    public async Task<SpecimenDetailResponse>
        GetSpecimenAsync(
            string specimenId,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        await RequireTechnicianAsync(
            user,
            cancellationToken
        );

        var specimen =
            await _workflowRepository
                .GetSpecimenAsync(
                    specimenId,
                    cancellationToken
                );

        if (specimen is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy mẫu bệnh phẩm: {specimenId}"
            );
        }

        return specimen;
    }

    public async Task ReceiveSpecimenAsync(
        string specimenId,
        ReceiveSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        await _workflowRepository
            .ReceiveSpecimenAsync(
                specimenId,
                request,
                technician,
                cancellationToken
            );
    }

    public async Task RejectSpecimenAsync(
        string specimenId,
        RejectSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        await _workflowRepository
            .RejectSpecimenAsync(
                specimenId,
                request,
                technician,
                cancellationToken
            );
    }

    // =====================================================
    // WORKLIST
    // =====================================================

    public async Task<List<WorklistItemResponse>>
        GetWorklistAsync(
            string? status,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        return await _workflowRepository
            .GetWorklistAsync(
                status,
                technician,
                cancellationToken
            );
    }

    public async Task StartWorkAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        await _workflowRepository
            .StartWorkAsync(
                id,
                technician,
                cancellationToken
            );
    }

    public async Task CompleteWorkAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        await _workflowRepository
            .CompleteWorkAsync(
                id,
                technician,
                cancellationToken
            );
    }

    // =====================================================
    // RESULT
    // =====================================================

    public async Task<ResultEntryResponse>
        GetResultEntryAsync(
            long id,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        var result =
            await _workflowRepository
                .GetResultEntryAsync(
                    id,
                    technician,
                    cancellationToken
                );

        if (result is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy worklist: {id}"
            );
        }

        return result;
    }

    public async Task SaveResultAsync(
        long id,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        bool submit,
        CancellationToken cancellationToken = default
    )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        await _workflowRepository
            .SaveResultAsync(
                id,
                request,
                technician,
                submit,
                cancellationToken
            );
    }

    public async Task SubmitResultBySpecimenAsync(
        string specimenId,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    )
    {
        var technician =
            await RequireTechnicianAsync(
                user,
                cancellationToken
            );

        var worklistId =
            await _workflowRepository
                .FindWorklistIdBySpecimenAsync(
                    specimenId,
                    technician,
                    cancellationToken
                );

        if (!worklistId.HasValue)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy worklist cho mẫu: {specimenId}"
            );
        }

        await _workflowRepository
            .SaveResultAsync(
                worklistId.Value,
                request,
                technician,
                true,
                cancellationToken
            );
    }

    // =====================================================
    // REQUIRE TECHNICIAN
    // =====================================================

    private async Task<TechnicianIdentity>
        RequireTechnicianAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken
        )
    {
        if (
            user.Identity?.IsAuthenticated
            != true
        )
        {
            throw new UnauthorizedAccessException(
                "Bạn chưa đăng nhập."
            );
        }

        var login =
            user.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            )
            ??
            user.Identity?.Name;

        if (
            string.IsNullOrWhiteSpace(
                login
            )
        )
        {
            throw new ForbiddenException(
                "Không xác định được tài khoản kỹ thuật viên."
            );
        }

        var technician =
            await _workflowRepository
                .FindCurrentTechnicianAsync(
                    login,
                    cancellationToken
                );

        if (technician is null)
        {
            throw new ForbiddenException(
                "Tài khoản chưa được liên kết với hồ sơ kỹ thuật viên đang hoạt động."
            );
        }

        return technician;
    }

    // =====================================================
    // MAPPING
    // =====================================================

    private static TechnicianResponse MapTechnician(
        Employee employee
    )
    {
        return new TechnicianResponse
        {
            Id =
                employee.Id,

            HoTen =
                employee.FullName,

            IdCoSo =
                employee.FacilityId,

            SoDienThoai =
                employee.Phone,

            Email =
                employee.Email,

            TrangThai =
                employee.Status == "no"
                    ? "INACTIVE"
                    : "ACTIVE",

            Status =
                employee.Status,

            CreatedAt =
                employee.CreatedAt
        };
    }

    private static void ValidateTechnician(
        TechnicianRequest request
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                request.HoTen
            )
        )
        {
            throw new BadRequestException(
                "Họ tên kỹ thuật viên không được để trống."
            );
        }
    }

    private async Task<string> GenerateEmployeeIdAsync(
        CancellationToken cancellationToken
    )
    {
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

            if (
                !await _employeeRepository
                    .ExistsByIdAsync(
                        id,
                        cancellationToken
                    )
            )
            {
                return id;
            }
        }

        throw new InvalidOperationException(
            "Không thể tạo mã kỹ thuật viên duy nhất."
        );
    }

    private static string NormalizeStatus(
        string? value
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                value
            )
        )
        {
            return "yes";
        }

        return value
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "ACTIVE" or
            "YES" =>
                "yes",

            "INACTIVE" or
            "NO" =>
                "no",

            _ =>
                throw new BadRequestException(
                    $"Trạng thái '{value}' không hợp lệ."
                )
        };
    }

    private static string? NormalizeNullable(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static string? NormalizeEmail(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value
                .Trim()
                .ToLowerInvariant();
    }
}