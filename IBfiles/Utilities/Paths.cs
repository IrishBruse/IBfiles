namespace IBfiles.Utilities;

using IBfiles.Logic;

public static class Paths
{
#if DEBUG
    public static readonly string SettingsFolder = Path.GetFullPath(Path.Join(FileManager.CurrentDirectory, "..", "Config"));
#else
    public static readonly string SettingsFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IrishBruse", "Files");
#endif

    public static readonly string ImGuiIni = Path.Join(SettingsFolder, "ImGui.ini\0");
    public static readonly string JsonConfig = Path.Join(SettingsFolder, "Settings.json");
}
