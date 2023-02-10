namespace IBfiles.Logic;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ImGuiNET;

using Silk.NET.Windowing;

using Vanara.PInvoke;

public static class FileManager
{
    private static string currentDirectory = Environment.CurrentDirectory;
    public static string CurrentDirectory { get => currentDirectory; private set { currentDirectory = value; History.Add(value); } }
    public static Page CurrentPageType { get; private set; } = Page.Directory;

    private static bool queueReloadFolder;

    public static List<string> History { get; set; } = new();

    public static List<DirectoryEntry> DirectoryContents { get; private set; } = new();
    public static bool SortDirty { get; set; }

    public static List<DirectoryEntry> Selections { get; private set; } = new();
    public static IWindow Window { get; set; }

    private static EnumerationOptions enumerationOptions = new() { AttributesToSkip = FileAttributes.System, ReturnSpecialDirectories = false };

    public static void Load()
    {
        Settings.Load();

        if (!string.IsNullOrEmpty(Settings.I.StartDirectory))
        {
            if (Directory.Exists(Settings.I.StartDirectory))
            {
                CurrentDirectory = Settings.I.StartDirectory;
                ReloadFolder();
            }
            else if (Settings.I.StartDirectory == "Home")
            {
                Open(Page.Home);
            }
        }

        Ole32.OleInitialize();
        Ole32.CoInitialize();


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
        if (a.Editing)
        {
            return 1;
        }

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

        if (Selections.Count > 0)
        {
            if (ImGui.IsKeyPressed(ImGuiKey.Delete, false))
            {
                foreach (DirectoryEntry selection in Selections)
                {
                    EntryHandler.Delete(selection);
                }

                ReloadFolder();
            }

            if (ImGui.IsKeyPressed(ImGuiKey.Enter, false))
            {
                foreach (DirectoryEntry selection in Selections)
                {
                    EntryHandler.Open(selection);
                }

                ReloadFolder();
            }

        }

        if (ImGui.IsKeyDown(ImGuiKey.LeftAlt) && ImGui.IsKeyPressed(ImGuiKey.UpArrow, false))
        {
            UpDirectoryLevel();
        }
    }

    public static void UpdateTitle()
    {
        if (Settings.I.TitleUsesFullPath)
        {
            Window.Title = CurrentDirectory;
        }
        else
        {
            Window.Title = new DirectoryInfo(CurrentDirectory).Name;
        }

        if (Settings.I.UseBackslashSeperator)
        {
            Window.Title = Window.Title.Replace('/', '\\');
        }
        else
        {
            Window.Title = Window.Title.Replace('\\', '/');
        }
    }

    public static void Open(string path)
    {
        Environment.CurrentDirectory = path;
        queueReloadFolder = true;

        CurrentPageType = Page.Directory;
        CurrentDirectory = path;
    }

    public static void Open(Page page)
    {
        CurrentPageType = page;

        CurrentDirectory = page switch
        {
            Page.Home => "Home",
            Page.Settings => "Settings",
            _ => throw new NotImplementedException(),
        };

        UpdateTitle();
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
            Open(Page.Home);
        }
        else
        {
            CurrentDirectory = Path.GetFullPath(Path.Join(CurrentDirectory, ".."));
            queueReloadFolder = true;
        }
    }

    public static void Refresh()
    {
        if (CurrentPageType != Page.Directory)
        {
            return;
        }

        ReloadFolder();
    }

    private static void ReloadFolder()
    {
        UpdateTitle();
        UpdateDirectoryContents();
    }

    public static void NewFile()
    {
        DirectoryEntry item = new(string.Empty, true, false, DateTime.Now, 0);
        item.Editing = true;
        DirectoryContents.Add(item);
    }

    public static void NewFolder()
    {
        DirectoryEntry item = new(string.Empty, false, false, DateTime.Now, 0);
        item.Editing = true;
        DirectoryContents.Add(item);
    }
}

public enum Page
{
    Directory,
    Home,
    Settings,
}
