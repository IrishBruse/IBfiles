namespace IBfiles.Logic;

using System.Text.Json;
using System.Text.Json.Serialization;

using IBfiles.Utilities;

#pragma warning disable CA1051

public class Settings
{
    public bool TitleUsesFullPath;
    public bool UseBackslashSeperator;
    public int BorderWidth = 10;
    public bool FoldersFirst = true;

    static Settings()
    {
        string json = File.ReadAllText(Paths.JsonConfig);
        JsonSerializerOptions options = new() { IncludeFields = true };
        I = JsonSerializer.Deserialize<Settings>(json, options);
    }

    public static void Save()
    {
        JsonSerializerOptions options = new() { IncludeFields = true, WriteIndented = true };
        string text = JsonSerializer.Serialize(I, options);
        File.WriteAllText(Paths.JsonConfig, text);
    }

    [JsonIgnore] public static Settings I { get; set; }
}
#pragma warning restore CA1051

