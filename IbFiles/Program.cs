using Silk.NET.Maths;
using Silk.NET.Windowing;

WindowOptions windowOptions = WindowOptions.Default;
windowOptions.Size = new Vector2D<int>(800, 600);
windowOptions.ShouldSwapAutomatically = false;
windowOptions.Title = "Test";
windowOptions.VSync = true;
IWindow window = Window.Create(windowOptions);

// Events
window.Load += OnLoad;
window.Render += OnRender;
window.Closing += OnClosing;
window.Resize += OnResize;

window.Run();

void OnLoad()
{
}


void OnRender(double dt)
{
}


void OnClosing()
{
}


void OnResize(Vector2D<int> size)
{
}
