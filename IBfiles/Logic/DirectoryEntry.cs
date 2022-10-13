namespace IBfiles.Logic;

public class DirectoryEntry
{
    public string Path { get; set; }
    public bool IsFile { get; init; }
    public bool IsHidden { get; init; }
    public DateTime LastWriteTime { get; init; }
    public long Size { get; init; }

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

    public DirectoryEntry(string path, bool isFile, bool isHidden, DateTime lastWriteTime, long size)
    {
        Path = path;
        IsFile = isFile;
        IsHidden = isHidden;
        LastWriteTime = lastWriteTime;
        Size = size;
    }

    public override string ToString()
    {
        return Path + " - " + IsFile + " - " + IsHidden + " - " + LastWriteTime + " - " + Size;
    }
}
