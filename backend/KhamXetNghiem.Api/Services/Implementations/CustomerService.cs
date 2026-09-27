using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Mappings;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class CustomerService
    : ICustomerService
{
    private readonly ICustomerRepository
        _customerRepository;

    private readonly IAccountRepository
        _accountRepository;

    public CustomerService(
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository
    )
    {
        _customerRepository =
            customerRepository;

        _accountRepository =
            accountRepository;
    }

    // =====================================================
    // CURRENT CUSTOMER
    // =====================================================

    public async Task<CustomerResponse>
        GetCurrentCustomerAsync(
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default
        )
    {
        var account =
            await GetCurrentAccountAsync(
                user,
                cancellationToken
            );

        if (
            string.IsNullOrWhiteSpace(
                account.IdKhachHang
            )
        )
        {
            throw new ResourceNotFoundException(
                "Tài khoản chưa được liên kết với khách hàng."
            );
        }

        var customer =
            await _customerRepository
                .FindByIdAsync(
                    account.IdKhachHang,
                    cancellationToken
                );

        if (customer is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy khách hàng với mã: {account.IdKhachHang}"
            );
        }

        return EntityMapper
            .ToCustomerResponse(
                customer
            )!;
    }

    // =====================================================
    // UPDATE CURRENT CUSTOMER
    // =====================================================

    public async Task<CustomerResponse>
        UpdateCurrentCustomerAsync(
            ClaimsPrincipal user,
            CustomerRequest request,
            CancellationToken cancellationToken = default
        )
    {
        var account =
            await GetCurrentAccountAsync(
                user,
                cancellationToken
            );

        if (
            string.IsNullOrWhiteSpace(
                account.IdKhachHang
            )
        )
        {
            throw new ResourceNotFoundException(
                "Tài khoản chưa được liên kết với khách hàng."
            );
        }

        return await UpdateCustomerInternalAsync(
            account.IdKhachHang,
            request,
            cancellationToken
        );
    }

    // =====================================================
    // ADMIN LIST
    // =====================================================

    public async Task<List<CustomerResponse>>
        GetAllCustomersAsync(
            string? query,
            CancellationToken cancellationToken = default
        )
    {
        var customers =
            await _customerRepository
                .GetAllAsync(
                    query,
                    cancellationToken
                );

        return customers
            .Select(
                c =>
                    EntityMapper
                        .ToCustomerResponse(c)!
            )
            .ToList();
    }

    // =====================================================
    // ADMIN DETAIL
    // =====================================================

    public async Task<CustomerResponse>
        GetCustomerByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
    {
        var customer =
            await _customerRepository
                .FindByIdAsync(
                    id,
                    cancellationToken
                );

        if (customer is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy khách hàng với mã: {id}"
            );
        }

        return EntityMapper
            .ToCustomerResponse(
                customer
            )!;
    }

    // =====================================================
    // ADMIN CREATE
    // =====================================================

    public async Task<CustomerResponse>
        CreateCustomerAsync(
            CustomerRequest request,
            CancellationToken cancellationToken = default
        )
    {
        ValidateCustomerRequest(
            request
        );

        var citizenId =
            NormalizeNullable(
                request.Cccd
            );

        if (
            citizenId is not null
            &&
            await _customerRepository
                .ExistsByCitizenIdAsync(
                    citizenId,
                    null,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                "CCCD/CMND này đã tồn tại trong hệ thống."
            );
        }

        var customerId =
            await GenerateCustomerIdAsync(
                cancellationToken
            );

        var customer =
            new Customer
            {
                Id =
                    customerId,

                FullName =
                    request.TenKhachHang
                        .Trim(),

                BirthDate =
                    request.NgaySinh,

                Gender =
                    NormalizeGender(
                        request.GioiTinh
                    ),

                Phone =
                    NormalizeNullable(
                        request.SoDienThoai
                    ),

                CitizenId =
                    citizenId,

                Address =
                    NormalizeNullable(
                        request.DiaChi
                    ),

                Email =
                    NormalizeEmail(
                        request.Email
                    ),

                Status =
                    NormalizeStatus(
                        request.Status
                    )
            };

        await _customerRepository
            .AddAsync(
                customer,
                cancellationToken
            );

        await _customerRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return EntityMapper
            .ToCustomerResponse(
                customer
            )!;
    }

    // =====================================================
    // ADMIN UPDATE
    // =====================================================

    public Task<CustomerResponse>
        UpdateCustomerAsync(
            string id,
            CustomerRequest request,
            CancellationToken cancellationToken = default
        )
    {
        return UpdateCustomerInternalAsync(
            id,
            request,
            cancellationToken
        );
    }

    // =====================================================
    // ADMIN SOFT DELETE
    // =====================================================

    public async Task DeleteCustomerAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var customer =
            await _customerRepository
                .FindTrackedByIdAsync(
                    id,
                    cancellationToken
                );

        if (customer is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy khách hàng với mã: {id}"
            );
        }

        customer.Status =
            "no";

        await _customerRepository
            .SaveChangesAsync(
                cancellationToken
            );
    }

    // =====================================================
    // INTERNAL UPDATE
    // =====================================================

    private async Task<CustomerResponse>
        UpdateCustomerInternalAsync(
            string id,
            CustomerRequest request,
            CancellationToken cancellationToken
        )
    {
        ValidateCustomerRequest(
            request
        );

        var customer =
            await _customerRepository
                .FindTrackedByIdAsync(
                    id,
                    cancellationToken
                );

        if (customer is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy khách hàng với mã: {id}"
            );
        }

        var citizenId =
            NormalizeNullable(
                request.Cccd
            );

        if (
            citizenId is not null
            &&
            await _customerRepository
                .ExistsByCitizenIdAsync(
                    citizenId,
                    id,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                "CCCD/CMND này đã được sử dụng bởi khách hàng khác."
            );
        }

        customer.FullName =
            request.TenKhachHang
                .Trim();

        customer.BirthDate =
            request.NgaySinh;

        customer.Gender =
            NormalizeGender(
                request.GioiTinh
            );

        customer.Phone =
            NormalizeNullable(
                request.SoDienThoai
            );

        customer.CitizenId =
            citizenId;

        customer.Address =
            NormalizeNullable(
                request.DiaChi
            );

        customer.Email =
            NormalizeEmail(
                request.Email
            );

        customer.Status =
            NormalizeStatus(
                request.Status
            );

        await _customerRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return EntityMapper
            .ToCustomerResponse(
                customer
            )!;
    }

    // =====================================================
    // CURRENT ACCOUNT
    // =====================================================

    private async Task<Account>
        GetCurrentAccountAsync(
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

        var email =
            user.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            )
            ??
            user.Identity?.Name;

        if (
            string.IsNullOrWhiteSpace(
                email
            )
        )
        {
            throw new UnauthorizedAccessException(
                "JWT không chứa thông tin người dùng."
            );
        }

        var account =
            await _accountRepository
                .FindByEmailAsync(
                    email,
                    cancellationToken
                );

        if (account is null)
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy tài khoản hiện tại."
            );
        }

        return account;
    }

    // =====================================================
    // VALIDATE
    // =====================================================

    private static void ValidateCustomerRequest(
        CustomerRequest request
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                request.TenKhachHang
            )
        )
        {
            throw new BadRequestException(
                "Họ tên không được để trống."
            );
        }

        if (
            request.NgaySinh.HasValue
            &&
            request.NgaySinh.Value
                >
                DateOnly.FromDateTime(
                    DateTime.Today
                )
        )
        {
            throw new BadRequestException(
                "Ngày sinh không được lớn hơn ngày hiện tại."
            );
        }
    }

    // =====================================================
    // CUSTOMER ID
    // =====================================================

    private async Task<string>
        GenerateCustomerIdAsync(
            CancellationToken cancellationToken
        )
    {
        for (
            var attempt = 0;
            attempt < 10;
            attempt++
        )
        {
            /*
             * KH + 8 ký tự = đúng tối đa 10 ký tự
             * của IDKhachHang varchar(10).
             */
            var id =
                "KH"
                +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            if (
                !await _customerRepository
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
            "Không thể sinh mã khách hàng duy nhất."
        );
    }

    // =====================================================
    // NORMALIZERS
    // =====================================================

    public static string? NormalizeGender(
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

        var normalized =
            value
                .Trim()
                .ToLowerInvariant();

        return normalized switch
        {
            "nam" =>
                "nam",

            "nu" or
            "nữ" =>
                "nu",

            "khac" or
            "khác" =>
                "khac",

            _ =>
                throw new BadRequestException(
                    "Giới tính chỉ chấp nhận: nam, nữ hoặc khác."
                )
        };
    }

    private static string NormalizeStatus(
        string? value
    )
    {
        return string.Equals(
            value?.Trim(),
            "no",
            StringComparison.OrdinalIgnoreCase
        )
            ? "no"
            : "yes";
    }

    private static string? NormalizeNullable(
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

        return value.Trim();
    }

    public static string? NormalizeEmail(
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
}