namespace IBfiles;

using System.Diagnostics;
using System.Text;

using IBfiles.ApplicationBackend;

using Silk.NET.Core;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Veldrid;

public class Program
{
    public static void Main()
    {
        GenerateIconCS();

        GraphicsBackend preferedBackend = GraphicsBackend.Vulkan;

        WindowOptions options = WindowOptions.Default;
        options.Size = new(800, 600);
        options.API = preferedBackend.ToGraphicsAPI();
        options.ShouldSwapAutomatically = false;
        options.Title = "";
        options.VSync = false;

        IWindow window = Window.Create(options);
        Application.Window = window;
        Application application = new(preferedBackend);

        window.Load += () =>
        {
            window.Center();

            RawImage icon = new(32, 32, Icon.Data);
            window.SetWindowIcon(ref icon);
        };

        window.Load += application.Load;
        window.Update += application.Update;
        window.Closing += application.Closing;
        window.Render += application.Render;
        window.FramebufferResize += application.FramebufferResize;

        window.Run();
    }

    [Conditional("DEBUG")]
    private static void GenerateIconCS()
    {
        StringBuilder output = new();

        _ = output.AppendLine("namespace IBfiles;");
        _ = output.AppendLine("");
        _ = output.AppendLine("public class Icon\n{");
        _ = output.AppendLine("    public static readonly byte[] Data = new byte[]\n{");

        Image<Rgba32> img = Image.Load<Rgba32>(File.ReadAllBytes("Icon.png"));
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                string value = $"{img[x, y].R}, {img[x, y].G}, {img[x, y].B}, {img[x, y].A},";
                _ = output.Append(value);
            }
            _ = output.AppendLine("");
        }

        _ = output.AppendLine("\n    };");
        _ = output.AppendLine("}");


        File.WriteAllText(Environment.CurrentDirectory + "/Icon.cs", output.ToString());
    }
}
