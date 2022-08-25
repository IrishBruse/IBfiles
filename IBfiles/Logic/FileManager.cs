namespace IBfiles.Logic;

using ImGuiNET;

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

    public static void Load()
    {
        ReloadFolder();
    }

    public static void UpdateDirectoryContents()
    {
        DirectoryContents.Clear();

        foreach (string path in Directory.EnumerateFiles(cwd))
        {
            AddEntry(path);
        }

        foreach (string path in Directory.EnumerateDirectories(cwd, "*", new EnumerationOptions() { ReturnSpecialDirectories = false }))
        {
            AddEntry(path);
        }
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
            size = Directory.EnumerateFileSystemEntries(path, "*", new EnumerationOptions() { ReturnSpecialDirectories = false }).Count();
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
            0 => a.Path.CompareTo(b.Path) * invertSort,
            1 => a.LastWriteTime.CompareTo(b.LastWriteTime) * invertSort,
            2 => SortFileFolderSize(a, b, invertSort),
            _ => 0,
        };
    }

    private static int SortFileFolderSize(DirectoryEntry a, DirectoryEntry b, int invertSort)
    {
        if (a.IsFile)
        {
            if (b.IsFile)
            {
                return a.Size.CompareTo(b.Size) * invertSort;
            }
            else
            {
                return 1 * invertSort;
            }
        }
        else
        {
            if (b.IsFile)
            {
                return -1 * invertSort;
            }
            else
            {
                return a.Size.CompareTo(b.Size) * invertSort;
            }
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
