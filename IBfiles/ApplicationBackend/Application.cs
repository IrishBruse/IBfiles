namespace IBfiles.ApplicationBackend;

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
    public IWindow Window { get; init; }

    private GraphicsDevice GraphicsDevice { get; set; }
    private CommandList CommandList { get; set; }

    private ImGuiController controller;
    private ImFontPtr font;
    private IInputContext input;
    private GraphicsBackend preferredBackend;
    public static ImFontPtr Icons26Font { get; set; }
    public static ImFontPtr Cascadia13Font { get; set; }

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

        unsafe
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(Paths.ImGuiIni));
            byte[] test = System.Text.Encoding.Default.GetBytes(Paths.ImGuiIni + '\0');
            fixed (byte* stringptr = test)
            {
                ImGuiNative.igGetIO()->IniFilename = stringptr;
            }
        }

        input = Window.CreateInput();

        IKeyboard keyboard = input.Keyboards[0];
        keyboard.KeyChar += KeyboardInput;
        keyboard.KeyDown += (kb, key, i) => KeyInput(key, true);
        keyboard.KeyUp += (kb, key, i) => KeyInput(key, false);

        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.Clear();

        byte[] cascadiaCodeData = ResourceLoader.GetEmbeddedResourceBytes("Assets/Fonts/CascadiaCode.ttf");
        byte[] codiconData = ResourceLoader.GetEmbeddedResourceBytes("Assets/Fonts/Codicon.ttf");

        unsafe
        {
            fixed (byte* cascadiaCodeDataPtr = cascadiaCodeData)
            {
                Cascadia13Font = io.Fonts.AddFontFromMemoryTTF((IntPtr)cascadiaCodeDataPtr, cascadiaCodeData.Length, 13f);
            }

            ushort[] range = new ushort[] { 60000, 60429, 0 };

            fixed (ushort* arrayptr = range)
            {
                fixed (byte* codiconDataPtr = codiconData)
                {
                    Icons26Font = io.Fonts.AddFontFromMemoryTTF((IntPtr)codiconDataPtr, codiconData.Length, 26f, null, (IntPtr)arrayptr);
                }
            }
        }

        controller.CreateDeviceResources(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription);



        GlobalStyle.Style();
    }

    public void Update(double delta)
    {
        MouseInput();
        controller.Update((float)delta); // Feed the input events to our ImGui controller, which passes them through to ImGui.

    }

    public void Render(double delta)
    {
        _ = delta;

        Gui.GuiManager.Submit();

        CommandList.Begin();
        CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
        CommandList.ClearColorTarget(0, new RgbaFloat(1f, 0f, 1f, 1f));
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
        io.MousePos = mouse.Position;
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
