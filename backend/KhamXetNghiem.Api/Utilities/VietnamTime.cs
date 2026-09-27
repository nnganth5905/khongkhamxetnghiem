namespace KhamXetNghiem.Api.Utilities;

public static class VietnamTime
{
    private static readonly TimeZoneInfo Zone =
        ResolveVietnamTimeZone();

    public static DateTime Now
    {
        get
        {
            var value =
                TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    Zone
                );

            return DateTime.SpecifyKind(
                value,
                DateTimeKind.Unspecified
            );
        }
    }

    private static TimeZoneInfo ResolveVietnamTimeZone()
    {
        try
        {
            return TimeZoneInfo
                .FindSystemTimeZoneById(
                    "Asia/Ho_Chi_Minh"
                );
        }
        catch
        {
            try
            {
                return TimeZoneInfo
                    .FindSystemTimeZoneById(
                        "SE Asia Standard Time"
                    );
            }
            catch
            {
                return TimeZoneInfo
                    .CreateCustomTimeZone(
                        "Vietnam",
                        TimeSpan.FromHours(7),
                        "Vietnam",
                        "Vietnam"
                    );
            }
        }
    }
}