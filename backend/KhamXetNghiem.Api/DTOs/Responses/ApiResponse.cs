namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed record ApiResponse<T>(
    bool Success,
    string? Message,
    T? Data,
    DateTime Timestamp
)
{
    public static ApiResponse<T> Ok(T? data)
    {
        return new ApiResponse<T>(
            true,
            null,
            data,
            DateTime.Now
        );
    }

    public static ApiResponse<T> Ok(string message, T? data)
    {
        return new ApiResponse<T>(
            true,
            message,
            data,
            DateTime.Now
        );
    }

    public static ApiResponse<T> Error(string message)
    {
        return new ApiResponse<T>(
            false,
            message,
            default,
            DateTime.Now
        );
    }
}
