namespace IBfiles.Logic;

using System;

using Microsoft.VisualBasic.FileIO;

public class EntryHandler
{
    public static void Open(DirectoryEntry entry)
    {
        if (entry.IsFile)
        {
            OpenFile(entry);
        }
        else
        {
            OpenFolder(entry);
        }
    }

    private static void OpenFolder(DirectoryEntry entry)
    {
        FileManager.Open(entry.Path);
    }

    private static void OpenFile(DirectoryEntry entry)
    {
        Console.WriteLine(entry);
    }

    public static void Delete(DirectoryEntry entry)
    {
        if (entry.IsFile)
        {
            FileSystem.DeleteFile(entry.Path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        }
        else
        {
            FileSystem.DeleteDirectory(entry.Path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        }
    }
}
