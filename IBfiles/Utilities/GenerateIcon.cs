namespace IBfiles.Utilities;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class GenerateIcon
{
    [Conditional("DEBUG")]
    public static void Generate()
    {
        StringBuilder output = new();

        output.AppendLine("namespace IBfiles;");
        output.AppendLine("");
        output.AppendLine("public class Icon\n{");
        output.AppendLine("    public static readonly byte[] Data =\n    [");

        Image<Rgba32> img = Image.Load<Rgba32>(File.ReadAllBytes("Icon.png"));
        for (int y = 0; y < img.Height; y++)
        {
            output.Append("        ");
            for (int x = 0; x < img.Width; x++)
            {
                string value = $"0x{img[x, y].R:X2},0x{img[x, y].G:X2},0x{img[x, y].B:X2},0x{img[x, y].A:X2},";
                output.Append(value);
            }
            output.AppendLine("");
        }

        output.AppendLine("    ];");
        output.AppendLine("}");

        File.WriteAllText(Environment.CurrentDirectory + "/Icon.cs", output.ToString());
    }
}
