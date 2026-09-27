namespace KhamXetNghiem.Api.Exceptions;

public sealed class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(
        string message
    ) : base(message)
    {
    }

    public ResourceNotFoundException(
        string resource,
        string id
    ) : base(
        $"{resource} không tồn tại với ID: {id}"
    )
    {
    }
}
