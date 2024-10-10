namespace WebLibServer.Extensions;

public static class ObjectExts
{
    public static bool IsNullOrEmptyString(this object val)
    {
        return val == null || (val is string && string.IsNullOrWhiteSpace(val.ToString()?.Trim()));
    }
}
