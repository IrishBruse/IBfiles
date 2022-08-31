namespace IBfiles.Utilities;

using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public struct FsPath
{
    public string Path { get; set; }

    public static implicit operator string(FsPath fsPath)
    {
        return fsPath.Path;
    }

    public override string ToString()
    {
        return Path;
    }
}

internal class FsPathConverter : JsonConverter<FsPath>
{
    public override FsPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new FsPath() { Path = reader.GetString() };
    }

    public override void Write(Utf8JsonWriter writer, FsPath val, JsonSerializerOptions options)
    {
        writer.WriteStringValue(val);
    }
}
