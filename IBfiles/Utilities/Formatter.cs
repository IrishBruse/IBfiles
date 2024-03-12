namespace IBfiles.Utilities;

using System;

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
            suffixes = ["B", "KB", "MB", "GB", "TB"];
        }
        else
        {
            suffixes = ["B", "KiB", "MiB", "GiB", "TiB"];
        }

        for (i = 0; i < suffixes.Length && bytes >= size; i++, bytes /= (long)size)
        {
            decimalBytes = bytes / size;
        }

        decimalBytes = Math.Floor(decimalBytes);

        return $"{decimalBytes:0.##} {suffixes[i]}";
    }
}
