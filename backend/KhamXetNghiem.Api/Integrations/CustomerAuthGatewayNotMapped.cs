using KhamXetNghiem.Api.Exceptions;

namespace KhamXetNghiem.Api.Integrations;

/// <summary>
/// Tạm thời dùng để project compile mà KHÔNG bịa schema Customer.
/// Sau khi có Customer.java + CustomerRepository.java + SHOW CREATE TABLE,
/// class này sẽ được thay bằng CustomerRepository thật.
/// </summary>
public sealed class CustomerAuthGatewayNotMapped
    : ICustomerAuthGateway
{
    public Task<CustomerAuthProfile?> FindByIdAsync(
        string customerId,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult<CustomerAuthProfile?>(
            null
        );
    }

    public Task<string> CreateCustomerAsync(
        string name,
        string email,
        string? phone,
        string? gender,
        CancellationToken cancellationToken = default
    )
    {
        throw new BadRequestException(
            "Chưa thể tạo khách hàng vì chưa có source Customer.java/CustomerRepository.java và schema bảng khách hàng. Không tự đoán mapping database."
        );
    }
}
