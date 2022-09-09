namespace IBfiles.Logic;

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

    private static void OpenFolder(DirectoryEntry folder)
    {
        FileManager.CWD = folder.Path;
    }

    private static void OpenFile(DirectoryEntry file)
    {

    }
}
