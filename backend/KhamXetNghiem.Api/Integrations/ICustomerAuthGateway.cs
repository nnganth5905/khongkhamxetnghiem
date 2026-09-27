namespace KhamXetNghiem.Api.Integrations;

public interface ICustomerAuthGateway
{
    Task<CustomerAuthProfile?> FindByIdAsync(
        string customerId,
        CancellationToken cancellationToken = default
    );

    Task<string> CreateCustomerAsync(
        string name,
        string email,
        string? phone,
        string? gender,
        CancellationToken cancellationToken = default
    );
}
