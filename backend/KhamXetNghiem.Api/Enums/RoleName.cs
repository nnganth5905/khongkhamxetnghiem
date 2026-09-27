namespace KhamXetNghiem.Api.Enums;

public enum RoleName
{
    ADMIN,
    DOCTOR,
    CUSTOMER,
    RECEPTIONIST,
    TECHNICIAN
}

public static class RoleNameExtensions
{
    public static RoleName FromValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return RoleName.CUSTOMER;
        }

        var normalized = value
            .Trim()
            .ToUpperInvariant()
            .Replace("-", string.Empty)
            .Replace("_", string.Empty)
            .Replace(" ", string.Empty);

        return normalized switch
        {
            "ADMIN" => RoleName.ADMIN,

            "BACSI" or
            "DOCTOR" or
            "BS" => RoleName.DOCTOR,

            "KHACHHANG" or
            "CUSTOMER" or
            "PATIENT" => RoleName.CUSTOMER,

            "LETAN" or
            "TIEPTAN" or
            "RECEPTIONIST" or
            "RECEPTION" => RoleName.RECEPTIONIST,

            "KTV" or
            "KYTHUATVIEN" or
            "TECHNICIAN" => RoleName.TECHNICIAN,

            _ => RoleName.CUSTOMER
        };
    }

    public static string Authority(this RoleName role)
    {
        return $"ROLE_{role}";
    }
}
