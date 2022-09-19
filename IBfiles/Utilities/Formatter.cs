namespace IBfiles.Utilities;

public static class Formatter
{
    public static string GetDataSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int i;
        double decimalBytes = bytes;
        for (i = 0; i < suffixes.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            decimalBytes = bytes / 1024.0;
        }

        return $"{decimalBytes:0.##} {suffixes[i]}";
    }
}
