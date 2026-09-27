using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;

namespace KhamXetNghiem.Api.Mappings;

public static class EntityMapper
{
    public static UserResponse? ToUserResponse(
        Account? account
    )
    {
        if (account is null)
        {
            return null;
        }

        return new UserResponse(
            account.UserId.ToString(),
            account.Username,
            account.Email,
            string.Empty,
            account.Role,
            account.IsActive
                ? "active"
                : "inactive"
        );
    }

    public static CustomerResponse? ToCustomerResponse(
        Customer? customer
    )
    {
        if (customer is null)
        {
            return null;
        }

        return new CustomerResponse
        {
            Id = customer.Id,

            IdKhachHang = customer.Id,

            TenKhachHang =
                customer.FullName,

            NgaySinh =
                customer.BirthDate,

            SoDienThoai =
                customer.Phone,

            GioiTinh =
                customer.Gender,

            Cccd =
                customer.CitizenId,

            DiaChi =
                customer.Address,

            Email =
                customer.Email,

            Status =
                customer.Status,

            CreatedAt =
                customer.CreatedAt,

            UpdatedAt =
                customer.UpdatedAt
        };
    }
}