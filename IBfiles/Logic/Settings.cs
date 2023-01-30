namespace IBfiles.Logic;

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using IBfiles.Utilities;

#pragma warning disable CA1051

public class Settings
{
    public bool TitleUsesFullPath;
    public bool UseBackslashSeperator;
    public int HeaderGap = 8;
    public bool FoldersFirst = true;
    public bool AlternateRowColors;
    public FsPath StartDirectory = new("Home");
    public bool HideOpticalDrives;
    public bool DecimalFileSize;
    public Dictionary<string, string> FileCommands = new();
    public Dictionary<string, string> FolderCommands = new();

    public static void Load()
    {
        if (!File.Exists(Paths.JsonConfig))
        {
            File.WriteAllText(Paths.JsonConfig, "{}");
        }
        string json = File.ReadAllText(Paths.JsonConfig);
        I = JsonSerializer.Deserialize<Settings>(json, SerializeOptions);
    }

    public static void Save()
    {
        string text = JsonSerializer.Serialize(I, SerializeOptions);
        File.WriteAllText(Paths.JsonConfig, text);
    }

    [JsonIgnore] public static Settings I { get; set; }

    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
            new FsPathConverter(),
        },
        IncludeFields = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
#pragma warning restore CA1051
