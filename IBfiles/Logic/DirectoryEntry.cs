namespace IBfiles.Logic;

public record DirectoryEntry(string Path, bool IsFile, bool IsHidden, DateTime LastWriteTime, long Size)
{
    /// <summary>
    /// The file/directory name
    /// </summary>
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
