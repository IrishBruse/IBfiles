namespace IBfiles;

using ImGuiNET;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;

using Veldrid;

using Key = Silk.NET.Input.Key;
using MouseButton = Silk.NET.Input.MouseButton;

public class Application : IDisposable
{
    public IWindow Window { get; init; }

    private GraphicsDevice GraphicsDevice { get; set; }
    private CommandList CommandList { get; set; }
    private ImGuiController controller;
    private ImFontPtr font;
    private IInputContext input;
    private GraphicsBackend preferredBackend;

    public Application(GraphicsBackend preferredBackend)
    {
        this.preferredBackend = preferredBackend;
    }

    public void Load()
    {
        GraphicsDeviceOptions graphicsOptions = new();
        graphicsOptions.PreferStandardClipSpaceYDirection = true;
        graphicsOptions.PreferDepthRangeZeroToOne = true;
        graphicsOptions.SyncToVerticalBlank = true;
        GraphicsDevice = Window.CreateGraphicsDevice(graphicsOptions, preferredBackend);

        CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();
        controller = new ImGuiController(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, Window.Size.X, Window.Size.Y);

        input = Window.CreateInput();

        IKeyboard keyboard = input.Keyboards[0];
        keyboard.KeyChar += KeyboardInput;
        keyboard.KeyDown += (kb, key, i) => KeyInput(key, true);
        keyboard.KeyUp += (kb, key, i) => KeyInput(key, false);

        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.Clear();
        font = io.Fonts.AddFontFromFileTTF(@"A:\IBfiles\IBfiles\CascadiaCode.ttf", 13f);
        controller.CreateDeviceResources(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription);

        GlobalStyle.Style();
    }

    public void Update(double delta)
    {
        MouseInput();
        controller.Update((float)delta); // Feed the input events to our ImGui controller, which passes them through to ImGui.
    }

    private void MouseInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        IMouse mouse = input.Mice[0];

        io.MouseDown[0] = mouse.IsButtonPressed(MouseButton.Left);
        io.MouseDown[1] = mouse.IsButtonPressed(MouseButton.Right);
        io.MouseDown[2] = mouse.IsButtonPressed(MouseButton.Middle);
        io.MousePos = mouse.Position;
        io.MouseWheel = mouse.ScrollWheels[0].Y;
    }

    public void Render(double delta)
    {
        // ImGui.PushFont(font);

        FileManager.Submit();

        CommandList.Begin();
        CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
        CommandList.ClearColorTarget(0, new RgbaFloat(25 / 255f, 29 / 255f, 31 / 255f, 1f));
        controller.Render(GraphicsDevice, CommandList);
        CommandList.End();
        GraphicsDevice.SubmitCommands(CommandList);
        GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
    }

    public void FramebufferResize(Vector2D<int> size)
    {
        GraphicsDevice.MainSwapchain.Resize((uint)size.X, (uint)size.Y);
        controller.WindowResized(size.X, size.Y);

        Window.DoRender();
        Window.DoUpdate();
    }

    public void Closing()
    {
        Clear();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Clear();
    }

    private void Clear()
    {
        // Clean up Veldrid resources
        GraphicsDevice.WaitForIdle();
        controller.Dispose();
        CommandList.Dispose();
        GraphicsDevice.Dispose();
    }

    private static void KeyInput(Key key, bool down)
    {
        if (key == Key.Unknown)
        {
            return;
        }

        ImGuiIOPtr io = ImGui.GetIO();

        io.KeysDown[(int)key] = down;

        if (key == Key.ControlLeft)
        {
            io.KeyCtrl = down;
        }
        else if (key == Key.ShiftLeft)
        {
            io.KeyShift = down;
        }
        else if (key == Key.AltLeft)
        {
            io.KeyAlt = down;
        }
        else if (key == Key.SuperLeft)
        {
            io.KeySuper = down;
        }
    }

    private void KeyboardInput(IKeyboard keyboard, char input)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.AddInputCharacter(input);
    }

}
