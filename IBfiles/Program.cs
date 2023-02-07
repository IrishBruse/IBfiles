using IBfiles.ApplicationBackend;
using IBfiles.Utilities;

using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;

using Veldrid;

GenerateIcon.Generate();

GraphicsBackend preferedBackend = GraphicsBackend.Vulkan;

WindowOptions options = WindowOptions.Default;
options.Size = new(800, 600);
options.API = preferedBackend.ToGraphicsAPI();
options.ShouldSwapAutomatically = false;
options.Title = "";
options.VSync = false;

IWindow window = Window.Create(options);
Application application = new(preferedBackend, window);

window.Run();

application.Dispose();
window.Dispose();
