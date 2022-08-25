namespace IBfiles.Logic;

public record DirectoryEntry(string Path, bool IsFile, bool IsHidden, DateTime LastWriteTime, long Size)
{
    public string Name
    {
        get
        {
            if (IsFile)
            {
                return System.IO.Path.GetFileName(Path);
            }
            else
            {
                return System.IO.Path.GetFileName(Path);
            }
        }
    }
}
