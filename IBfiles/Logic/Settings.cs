namespace IBfiles.Logic;

using System.Text.Json;
using System.Text.Json.Serialization;

using IBfiles.Utilities;

#pragma warning disable CA1051

public class Settings
{
    public bool TitleUsesFullPath;
    public bool UseBackslashSeperator;
    public int EdgeBorderWidth = 10;
    public int HeaderGap = 8;
    public bool FoldersFirst = true;
    public bool ConfigFilesLast = true;
    public bool AlternateRowColors;
    public FsPath StartDirectory;

    static Settings()
    {
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

