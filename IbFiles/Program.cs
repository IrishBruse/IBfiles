namespace IBfiles;

using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;

using Veldrid;

public class Program
{
    public static void Main()
    {
        GraphicsBackend preferedBackend = GraphicsBackend.Vulkan;

        WindowOptions options = WindowOptions.Default;
        options.API = preferedBackend.ToGraphicsAPI();
        options.ShouldSwapAutomatically = false;
        options.Title = "Monoboy";
        options.VSync = false;

        IWindow window = Window.Create(options);
        Application application = new(preferedBackend) { Window = window };

        window.Load += application.Load;
        window.Update += application.Update;
        window.Closing += application.Closing;
        window.Render += application.Render;
        window.FramebufferResize += application.FramebufferResize;

        window.Run();
    }
}
