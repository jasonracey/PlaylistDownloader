namespace PlaylistDownloaderConsole;

public static class Extensions
{
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : class
    {
        return enumerable
            .Where(t => t != null)
            .Select(t => t!);
    }
}