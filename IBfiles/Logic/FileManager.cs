namespace IBfiles.Logic;

using IBfiles.ApplicationBackend;

using ImGuiNET;

using Silk.NET.Windowing;

public static class FileManager
{
    public static string CurrentDirectory { get; private set; } = Environment.CurrentDirectory;

    private static bool queueReloadFolder;

    public static List<string> History { get; set; } = new();

    public static List<DirectoryEntry> DirectoryContents { get; private set; } = new();
    public static bool SortDirty { get; internal set; }

    public static List<string> Selections { get; private set; } = new();
    private static EnumerationOptions enumerationOptions = new() { AttributesToSkip = FileAttributes.System, ReturnSpecialDirectories = false };

    public static void Load()
    {
        Settings.Load();

        if (!string.IsNullOrEmpty(Settings.I.StartDirectory))
        {
            if (Directory.Exists(Settings.I.StartDirectory))
            {
                CurrentDirectory = Settings.I.StartDirectory;
            }
        }

        ReloadFolder();
    }

    public static void UpdateDirectoryContents()
    {
        DirectoryContents.Clear();

        foreach (string path in Directory.EnumerateFiles(CurrentDirectory, "*", enumerationOptions))
        {
            AddEntry(path);
        }

        foreach (string path in Directory.EnumerateDirectories(CurrentDirectory, "*", enumerationOptions))
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
    {
        // If both same
        if (a.IsFile == b.IsFile)
        {
            return a.Name.CompareTo(b.Name) * invertSort;
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

    public static void Update()
    {
        if (queueReloadFolder)
        {
            queueReloadFolder = false;
            ReloadFolder();
        }
    }

    public static void UpdateTitle()
    {
        IWindow window = Application.Window;
        if (Settings.I.TitleUsesFullPath)
        {
            window.Title = CurrentDirectory;
        }
        else
        {
            window.Title = new DirectoryInfo(CurrentDirectory).Name;
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

    public static void Open(string path, bool isPage = false)
    {
        if (!isPage)
        {
            Environment.CurrentDirectory = path;
            queueReloadFolder = true;
        }

        CurrentDirectory = path;
    }

    public static void HistoryBack()
    {

    }

    public static void HistoryForward()
    {

    }

    public static void UpDirectoryLevel()
    {
        if (CurrentDirectory.Length == 3 && CurrentDirectory[1] == ':' && CurrentDirectory[2] == '\\')
        {
            Open("Home", true);
        }
        else
        {
            CurrentDirectory = Path.GetFullPath(Path.Join(CurrentDirectory, ".."));
            queueReloadFolder = true;
        }
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
