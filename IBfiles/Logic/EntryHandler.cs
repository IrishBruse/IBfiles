namespace IBfiles.Logic;

using System;
using System.Diagnostics;

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
        using Process fileopener = new();

        fileopener.StartInfo.FileName = "explorer";
        fileopener.StartInfo.Arguments = "\"" + entry.Path + "\"";
        _ = fileopener.Start();
    }

    public static void Delete(DirectoryEntry entry)
    {
        if (entry.IsFile)
        {
            FileSystem.DeleteFile(entry.Path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        }
        else
        {
            try
            {
                FileSystem.DeleteDirectory(entry.Path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
