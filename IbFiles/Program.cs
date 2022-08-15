using System.Drawing;

using ImGuiNET;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

WindowOptions windowOptions = WindowOptions.Default;
windowOptions.Size = new Vector2D<int>(800, 600);
windowOptions.Title = "Test";
windowOptions.VSync = true;
using IWindow window = Window.Create(windowOptions);

ImGuiController controller = default;
GL gl = default;
IInputContext inputContext = default;

// Events
window.Load += () => controller = new ImGuiController(gl = window.CreateOpenGL(), window, inputContext = window.CreateInput());

window.Render += (double delta) =>
{
    // Make sure ImGui is up-to-date
    controller.Update((float)delta);

    // This is where you'll do any rendering beneath the ImGui context
    // Here, we just have a blank screen.
    gl.ClearColor(Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));
    gl.Clear((uint)ClearBufferMask.ColorBufferBit);

    // This is where you'll do all of your ImGUi rendering
    // Here, we're just showing the ImGui built-in demo window.
    ImGui.ShowDemoWindow();

    // Make sure ImGui renders too!
    controller.Render();
};

window.Closing += () =>
{                // Dispose our controller first
    controller?.Dispose();

    // Dispose the input context
    inputContext?.Dispose();

    // Unload OpenGL
    gl?.Dispose();
};


window.FramebufferResize += (s) => gl.Viewport(s);

window.Run();
