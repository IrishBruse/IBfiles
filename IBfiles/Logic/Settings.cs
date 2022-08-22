namespace IBfiles.Logic;

#pragma warning disable CA1051
public class Settings
{
    public static Settings I { get; set; }

    public bool TitleUsesFullPath;

    static Settings()
    {
        I = new();
    }
}
#pragma warning restore CA1051

