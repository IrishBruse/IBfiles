namespace IBfiles.Utilities;

public static class Paths
{
    public static readonly string SettingsFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IrishBruse", "Files");
    public static readonly string ImGuiIni = Path.Join(SettingsFolder, "ImGui.ini");
    public static readonly string JsonConfig = Path.Join(SettingsFolder, "Settings.json");
}
