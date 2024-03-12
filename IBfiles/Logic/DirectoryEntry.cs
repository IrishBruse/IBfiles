namespace IBfiles.Logic;

using System;

public class DirectoryEntry(string path, bool isFile, bool isHidden, DateTime lastWriteTime, long size)
{
    public string Path { get; set; } = path;
    public bool IsFile { get; init; } = isFile;
    public bool IsHidden { get; init; } = isHidden;
    public DateTime LastWriteTime { get; init; } = lastWriteTime;
    public long Size { get; init; } = size;

    public bool Editing { get; set; }

    /// <summary> The file/directory name </summary>
    public string Name
    {
        get => System.IO.Path.GetFileName(Path);
        set
        {
            string dir = System.IO.Path.GetPathRoot(Path);
            if (string.IsNullOrEmpty(dir))
            {
                dir = System.IO.Path.GetDirectoryName(Path);
            }
            Path = System.IO.Path.Join(dir, value);
        }
    }

    public override string ToString()
    {
        return Path + " - " + IsFile + " - " + IsHidden + " - " + LastWriteTime + " - " + Size;
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        string path = ((DirectoryEntry)obj).Path;
        return Path.Equals(path, StringComparison.Ordinal);
    }
}
