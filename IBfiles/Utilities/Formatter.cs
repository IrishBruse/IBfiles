namespace IBfiles.Utilities;

using IBfiles.Logic;

public static class Formatter
{
    public static string GetDataSize(long bytes)
    {
        int i;
        double decimalBytes = bytes;

        double size = Settings.I.DecimalFileSize ? 1000 : 1024;

        string[] suffixes;

        if (Settings.I.DecimalFileSize)
        {
            suffixes = new string[] { "B", "KB", "MB", "GB", "TB" };
        }
        else
        {
            suffixes = new string[] { "B", "KiB", "MiB", "GiB", "TiB" };
        }

        for (i = 0; i < suffixes.Length && bytes >= size; i++, bytes /= (long)size)
        {
            decimalBytes = bytes / size;
        }

        return $"{decimalBytes:0.##} {suffixes[i]}";
    }
}
