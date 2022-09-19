namespace IBfiles.ApplicationBackend;

using System.Numerics;

public static class Colors
{
    public static readonly Vector4 BackgroundDark = Color(25, 29, 31);
    public static readonly Vector4 BackgroundNormal = Color(30, 34, 36);
    public static readonly Vector4 BackgroundLight = Color(35, 39, 42);
    public static readonly Vector4 BackgroundInput = Color(50, 56, 61);
    public static readonly Vector4 AccentLight = Color(134, 195, 50);
    public static readonly Vector4 AccentDark = Color(85, 125, 28);
    public static readonly Vector4 AccentDarker = Color(85, 125, 28, 0.75f);
    public static readonly Vector4 Text = Color(187, 187, 187);
    public static readonly Vector4 TextDisabled = Color(187, 187, 187, .6f);
    public static readonly Vector4 Error = Color(255, 0, 255);
    public static readonly Vector4 Scrollbar = Color(66, 68, 70);
    public static readonly Vector4 ScrollbarHover = Color(0x4f, 0x50, 0x51);
    public static readonly Vector4 BackgroundDim = Color(25, 29, 31, 0.75f);

    public static readonly Vector4 White = Color(255, 255, 255);

    private static Vector4 Color(int r, int g, int b)
    {
        return Color(r, g, b, 1f);
    }

    private static Vector4 Color(int r, int g, int b, float a)
    {
        return new Vector4(r / 255f, g / 255f, b / 255f, a);
    }
}
