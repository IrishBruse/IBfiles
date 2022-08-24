namespace IBfiles.Logic;

using Silk.NET.Windowing;

public static class FileManager
{
    private static string cwd = Environment.CurrentDirectory;
    public static string CWD
    {
        get => cwd;
        set
        {
            cwd = value;

            UpdateTitle();
            UpdateDirectoryContents();

            Environment.CurrentDirectory = cwd;
        }
    }

    public static List<DirectoryEntry> DirectoryContents { get; private set; } = new();

    public static void UpdateDirectoryContents()
    {
        DirectoryContents.Clear();

        foreach (string path in Directory.EnumerateFiles(cwd))
        {
            bool isHidden = (File.GetAttributes(path) & FileAttributes.Hidden) != 0;
            DirectoryContents.Add(new(path, true, isHidden));
        }

        foreach (string path in Directory.EnumerateDirectories(cwd))
        {
            bool isHidden = (File.GetAttributes(path) & FileAttributes.Hidden) != 0;
            DirectoryContents.Add(new(path, false, isHidden));
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

    }

    public static void Load()
    {
        UpdateTitle();
    }
}
