namespace KhamXetNghiem.Api.Enums;

public enum Role
{
    CUSTOMER,
    DOCTOR,
    RECEPTIONIST,
    TECHNICIAN,
    ADMIN
}

public static class RoleExtensions
{
    public static Role FromDatabaseValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Role.CUSTOMER;
        }

        var normalized = value
            .Trim()
            .ToUpperInvariant()
            .Replace("-", string.Empty)
            .Replace("_", string.Empty)
            .Replace(" ", string.Empty);

        return normalized switch
        {
            "ADMIN" => Role.ADMIN,
            "BACSI" or "DOCTOR" or "BS" => Role.DOCTOR,
            "LETAN" or "TIEPTAN" or "RECEPTIONIST" or "RECEPTION" => Role.RECEPTIONIST,
            "KTV" or "KYTHUATVIEN" or "TECHNICIAN" => Role.TECHNICIAN,
            "KHACHHANG" or "CUSTOMER" or "PATIENT" => Role.CUSTOMER,
            _ => Role.CUSTOMER
        };
    }
}
