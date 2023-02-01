namespace IBfiles.ApplicationBackend;

using System;

using IBfiles.Logic;
using IBfiles.Utilities;

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
    public static IWindow Window { get; set; }

    public static ImFontPtr IconsFontBig { get; set; }
    public static ImFontPtr IconsFont { get; set; }
    public static ImFontPtr CascadiaFont { get; set; }

    private GraphicsDevice GraphicsDevice { get; set; }
    private CommandList CommandList { get; set; }

    private ImGuiController controller;
    private IInputContext input;
    private GraphicsBackend preferredBackend;
    private bool focused = true;
    private bool minimized;

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

        controller = new(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, Window.Size.X, Window.Size.Y);

        ImGui.LoadIniSettingsFromDisk(Paths.ImGuiIni);

        GlobalStyle.Style();

        Window.FocusChanged += (focus) => focused = focus;
        Window.Closing += () => Console.WriteLine("Close");
        Window.FramebufferResize += (fb) => minimized = fb.X == 0 && fb.Y == 0;

        input = Window.CreateInput();

        IKeyboard keyboard = input.Keyboards[0];
        keyboard.KeyChar += KeyboardInput;
        keyboard.KeyDown += (kb, key, i) => KeyInput(key, true);
        keyboard.KeyUp += (kb, key, i) => KeyInput(key, false);

        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.Clear();

        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

        ushort[] codiconRange = new ushort[] { 60000, 60429, 0 };

        CascadiaFont = NewFont("Assets/Fonts/CascadiaCode.ttf", 15);
        IconsFontBig = NewFontWithRange("Assets/Fonts/Codicon.ttf", 26, codiconRange);
        IconsFont = NewFontWithRange("Assets/Fonts/Codicon.ttf", 15, codiconRange);

        controller.CreateDeviceResources(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription);

        IconManager.Load(GraphicsDevice, controller);
        FileManager.Load();
    }

    private static unsafe ImFontPtr NewFont(string fontFile, float pixelSize)
    {
        ImGuiIOPtr io = ImGui.GetIO();

        byte[] data = ResourceLoader.GetEmbeddedResourceBytes(fontFile);
        fixed (byte* fontPtr = data)
        {
            return io.Fonts.AddFontFromMemoryTTF((IntPtr)fontPtr, data.Length, pixelSize);
        }
    }

    private static unsafe ImFontPtr NewFontWithRange(string fontFile, float pixelSize, ushort[] range)
    {
        ImGuiIOPtr io = ImGui.GetIO();

        byte[] data = ResourceLoader.GetEmbeddedResourceBytes(fontFile);
        fixed (ushort* rangePtr = range)
        {
            fixed (byte* dataPtr = data)
            {
                return io.Fonts.AddFontFromMemoryTTF((IntPtr)dataPtr, data.Length, pixelSize, null, (IntPtr)rangePtr);
            }
        }
    }

    public void Update(double delta)
    {
        MouseInput();
        controller.Update((float)delta); // Feed the input events to our ImGui controller, which passes them through to ImGui.

        ImGuiIOPtr io = ImGui.GetIO();
        if (io.WantSaveIniSettings)
        {
            ImGui.SaveIniSettingsToDisk(Paths.ImGuiIni);
        }

        FileManager.Update();
    }

    public void Render(double delta)
    {
        if (!minimized)
        {
            Gui.GuiManager.Submit();
        }

        CommandList.Begin();
        CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
        CommandList.ClearColorTarget(0, new RgbaFloat(Colors.BackgroundDark));
        controller.Render(GraphicsDevice, CommandList);
        CommandList.End();
        GraphicsDevice.SubmitCommands(CommandList);
        GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
    }

    public void FramebufferResize(Vector2D<int> size)
    {
        GraphicsDevice.MainSwapchain.Resize((uint)size.X, (uint)size.Y);
        controller.WindowResized(size.X, size.Y);

        Window.DoUpdate();
        Window.DoRender();
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

    private void MouseInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        IMouse mouse = input.Mice[0];

        io.MouseDown[0] = mouse.IsButtonPressed(MouseButton.Left);
        io.MouseDown[1] = mouse.IsButtonPressed(MouseButton.Right);
        io.MouseDown[2] = mouse.IsButtonPressed(MouseButton.Middle);
        if (focused)
        {
            io.MousePos = mouse.Position;
        }
        else
        {
            io.MousePos = new System.Numerics.Vector2(0);
        }
        io.MouseWheel = mouse.ScrollWheels[0].Y;

        mouse.Cursor.CursorMode = CursorMode.Normal;
        io.MouseDrawCursor = false;

        switch (ImGui.GetMouseCursor())
        {
            case ImGuiMouseCursor.None: mouse.Cursor.StandardCursor = StandardCursor.Default; break;
            case ImGuiMouseCursor.Arrow: mouse.Cursor.StandardCursor = StandardCursor.Arrow; break;
            case ImGuiMouseCursor.TextInput: mouse.Cursor.StandardCursor = StandardCursor.IBeam; break;
            case ImGuiMouseCursor.Hand: mouse.Cursor.StandardCursor = StandardCursor.Hand; break;

            case ImGuiMouseCursor.NotAllowed:
            case ImGuiMouseCursor.ResizeAll:
            case ImGuiMouseCursor.ResizeNS:
            case ImGuiMouseCursor.ResizeEW:
            case ImGuiMouseCursor.ResizeNESW:
            case ImGuiMouseCursor.ResizeNWSE:
            mouse.Cursor.CursorMode = CursorMode.Hidden;
            io.MouseDrawCursor = true;
            break;
        };
    }

}
