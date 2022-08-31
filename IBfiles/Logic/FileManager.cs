namespace IBfiles.Logic;

using ImGuiNET;

using Silk.NET.Windowing;

public static class FileManager
{
    private static string cwd = Environment.CurrentDirectory;
    public static string CWD
    {
        get => cwd;
        set { cwd = value; Environment.CurrentDirectory = value; ReloadFolder(); }
    }

    public static List<string> History { get; set; } = new();

    public static List<DirectoryEntry> DirectoryContents { get; private set; } = new();
    public static bool SortDirty { get; internal set; }

    public static List<string> Selections { get; private set; } = new();

    public static void Load()
    {
        if (!string.IsNullOrEmpty(Settings.I.StartDirectory))
        {
            cwd = Settings.I.StartDirectory;
        }

        ReloadFolder();
    }

    private static EnumerationOptions enumerationOptions = new() { AttributesToSkip = FileAttributes.System, ReturnSpecialDirectories = false };
    public static void UpdateDirectoryContents()
    {
        DirectoryContents.Clear();


        foreach (string path in Directory.EnumerateFiles(cwd, "*", enumerationOptions))
        {
            AddEntry(path);
        }

        foreach (string path in Directory.EnumerateDirectories(cwd, "*", enumerationOptions))
        {
            AddEntry(path);
        }

        SortDirty = true;
    }

    private static void AddEntry(string path)
    {
        FileInfo fInfo = new(path);
        bool isFile = !fInfo.Attributes.HasFlag(FileAttributes.Directory);

        long size;
        if (isFile)
        {
            size = fInfo.Length;
        }
        else
        {
            size = Directory.EnumerateFileSystemEntries(path, "*", enumerationOptions).Count();
        }

        bool isHidden = fInfo.Attributes.HasFlag(FileAttributes.Hidden);
        DirectoryContents.Add(new(path, isFile, isHidden, fInfo.LastWriteTime, size));
    }

    public static int SortDirectory(DirectoryEntry a, DirectoryEntry b, ImGuiTableSortSpecsPtr specs)
    {
        specs.SpecsDirty = false;

        int invertSort = specs.Specs.SortDirection == ImGuiSortDirection.Descending ? -1 : 1;

        return specs.Specs.ColumnIndex switch
        {
            0 => SortFileNames(a, b, invertSort),
            1 => SortCreationDate(a, b, invertSort),
            2 => SortFileFolderSize(a, b, invertSort),
            _ => 0,
        };
    }

    private static int SortCreationDate(DirectoryEntry a, DirectoryEntry b, int invertSort)
    {
        if (a.LastWriteTime.Equals(b.LastWriteTime))
        {
            return SortFileNames(a, b, invertSort);
        }
        else
        {
            return a.LastWriteTime.CompareTo(b.LastWriteTime) * invertSort;
        }
    }

    private static int SortFileNames(DirectoryEntry a, DirectoryEntry b, int invertSort)
    {       // If both same
        if (a.IsFile == b.IsFile)
        {
            return a.Size.CompareTo(b.Size) * invertSort;
        }
        else
        {
            return (a.IsFile ? 1 : -1) * (Settings.I.FoldersFirst ? 1 : -1);
        }
    }

    private static int SortFileFolderSize(DirectoryEntry a, DirectoryEntry b, int invertSort)
    {
        // If both same
        if (a.IsFile == b.IsFile)
        {
            return a.Size.CompareTo(b.Size) * invertSort;
        }
        else
        {
            return a.IsFile ? invertSort : -invertSort;
        }
    }

    public static void UpdateTitle()
    {
        IWindow window = ApplicationBackend.Application.Window;
        if (Settings.I.TitleUsesFullPath)
        {
            window.Title = cwd;
        }
        else
        {
            window.Title = new DirectoryInfo(cwd).Name;
        }

        if (Settings.I.UseBackslashSeperator)
        {
            window.Title = window.Title.Replace('/', '\\');
        }
        else
        {
            window.Title = window.Title.Replace('\\', '/');
        }
    }

    public static void HistoryBack()
    {

    }

    public static void HistoryForward()
    {

    }

    public static void UpDirectoryLevel()
    {
        CWD = Path.GetFullPath(Path.Join(CWD, ".."));
    }

    public static void Refresh()
    {
        ReloadFolder();
    }

    private static void ReloadFolder()
    {
        UpdateTitle();
        UpdateDirectoryContents();
    }
}
