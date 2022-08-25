namespace IBfiles.ApplicationBackend;

using System.Numerics;
using System.Runtime.CompilerServices;

using IBfiles.Utilities;

using ImGuiNET;

using Veldrid;

using Key = Silk.NET.Input.Key;

/// <summary>
/// A modified version of Veldrid.ImGui's ImGuiRenderer.
/// Manages input for ImGui and handles rendering ImGui's DrawLists with Veldrid.
/// </summary>
public class ImGuiController : IDisposable
{
    private bool frameBegun;

    // Veldrid objects
    private DeviceBuffer vertexBuffer;
    private DeviceBuffer indexBuffer;
    private DeviceBuffer projMatrixBuffer;
    private Texture fontTexture;
    private TextureView fontTextureView;
    private Shader vertexShader;
    private Shader fragmentShader;
    private ResourceLayout layout;
    private ResourceLayout textureLayout;
    private Pipeline pipeline;
    private ResourceSet mainResourceSet;
    private ResourceSet fontTextureResourceSet;

    private IntPtr fontAtlasID = (IntPtr)1;
    private bool controlDown;
    private bool shiftDown;
    private bool altDown;
    private bool winKeyDown;

    private int windowWidth;
    private int windowHeight;
    private Vector2 scaleFactor = Vector2.One;

    // Image trackers
    private readonly Dictionary<TextureView, ResourceSetInfo> setsByView = new();
    private readonly Dictionary<Texture, TextureView> autoViewsByTexture = new();
    private readonly Dictionary<IntPtr, ResourceSetInfo> viewsById = new();
    private readonly List<IDisposable> ownedResources = new();
    private int lastAssignedID = 100;

    /// <summary>
    /// Constructs a new ImGuiController.
    /// </summary>
    public ImGuiController(GraphicsDevice gd, OutputDescription outputDescription, int width, int height)
    {
        windowWidth = width;
        windowHeight = height;

        IntPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        ImGuiIOPtr io = ImGui.GetIO();
        _ = io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        unsafe
        {
            ImGuiNative.igGetIO()->IniFilename = null;
        }

        CreateDeviceResources(gd, outputDescription);
        SetKeyMappings();

        SetPerFrameImGuiData(1f / 60f);

        ImGui.NewFrame();
        frameBegun = true;
    }

    public void WindowResized(int width, int height)
    {
        windowWidth = width;
        windowHeight = height;
    }

    public void DestroyDeviceObjects()
    {
        Dispose();
    }

    public void CreateDeviceResources(GraphicsDevice gd, OutputDescription outputDescription)
    {
        ResourceFactory factory = gd.ResourceFactory;
        vertexBuffer = factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        vertexBuffer.Name = "ImGui.NET Vertex Buffer";
        indexBuffer = factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        indexBuffer.Name = "ImGui.NET Index Buffer";
        RecreateFontDeviceTexture(gd);

        projMatrixBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        projMatrixBuffer.Name = "ImGui.NET Projection Buffer";

        byte[] vertexShaderBytes = LoadEmbeddedShaderCode(gd.ResourceFactory, "Assets/Shaders/imgui-vertex");
        byte[] fragmentShaderBytes = LoadEmbeddedShaderCode(gd.ResourceFactory, "Assets/Shaders/imgui-frag");
        vertexShader = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, gd.BackendType == GraphicsBackend.Metal ? "VS" : "main"));
        fragmentShader = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, gd.BackendType == GraphicsBackend.Metal ? "FS" : "main"));

        VertexLayoutDescription[] vertexLayouts = new VertexLayoutDescription[]
        {
            new VertexLayoutDescription(
                new VertexElementDescription("inposition", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("intexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("incolor", VertexElementSemantic.Color, VertexElementFormat.Byte4_Norm))
        };

        layout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionMatrixBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("MainSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
        textureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)));

        GraphicsPipelineDescription pd = new(
            BlendStateDescription.SingleAlphaBlend,
            new DepthStencilStateDescription(false, false, ComparisonKind.Always),
            new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false, true),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(vertexLayouts, new[] { vertexShader, fragmentShader }),
            new ResourceLayout[] { layout, textureLayout },
            outputDescription,
            ResourceBindingModel.Default);
        pipeline = factory.CreateGraphicsPipeline(ref pd);

        mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(layout,
            projMatrixBuffer,
            gd.PointSampler));

        fontTextureResourceSet = factory.CreateResourceSet(new ResourceSetDescription(textureLayout, fontTextureView));
    }

    /// <summary>
    /// Gets or creates a handle for a texture to be drawn with ImGui.
    /// Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, TextureView textureView)
    {
        if (!setsByView.TryGetValue(textureView, out ResourceSetInfo rsi))
        {
            ResourceSet resourceSet = factory.CreateResourceSet(new ResourceSetDescription(textureLayout, textureView));
            rsi = new ResourceSetInfo(GetNextImGuiBindingID(), resourceSet);

            setsByView.Add(textureView, rsi);
            viewsById.Add(rsi.ImGuiBinding, rsi);
            ownedResources.Add(resourceSet);
        }

        return rsi.ImGuiBinding;
    }

    private IntPtr GetNextImGuiBindingID()
    {
        int newID = lastAssignedID++;
        return (IntPtr)newID;
    }

    /// <summary>
    /// Gets or creates a handle for a texture to be drawn with ImGui.
    /// Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, Texture texture)
    {
        if (!autoViewsByTexture.TryGetValue(texture, out TextureView textureView))
        {
            textureView = factory.CreateTextureView(texture);
            autoViewsByTexture.Add(texture, textureView);
            ownedResources.Add(textureView);
        }

        return GetOrCreateImGuiBinding(factory, textureView);
    }

    /// <summary>
    /// Retrieves the shader texture binding for the given helper handle.
    /// </summary>
    public ResourceSet GetImageResourceSet(IntPtr imGuiBinding)
    {
        if (!viewsById.TryGetValue(imGuiBinding, out ResourceSetInfo tvi))
        {
            throw new InvalidOperationException($"No registered ImGui binding with id {imGuiBinding}");
        }

        return tvi.ResourceSet;
    }

    public void ClearCachedImageResources()
    {
        foreach (IDisposable resource in ownedResources)
        {
            resource.Dispose();
        }

        ownedResources.Clear();
        setsByView.Clear();
        viewsById.Clear();
        autoViewsByTexture.Clear();
        lastAssignedID = 100;
    }

    private static byte[] LoadEmbeddedShaderCode(ResourceFactory factory, string name)
    {
        return factory.BackendType switch
        {
            GraphicsBackend.Direct3D11 => ResourceLoader.GetEmbeddedResourceBytes(name + ".hlsl.bytes"),
            GraphicsBackend.OpenGL => ResourceLoader.GetEmbeddedResourceBytes(name + ".glsl"),
            GraphicsBackend.Vulkan => ResourceLoader.GetEmbeddedResourceBytes(name + ".spv"),
            GraphicsBackend.Metal => ResourceLoader.GetEmbeddedResourceBytes(name + ".metallib"),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Recreates the device texture used to render text.
    /// </summary>
    public void RecreateFontDeviceTexture(GraphicsDevice gd)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        // Build
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);
        // Store our identifier
        io.Fonts.SetTexID(fontAtlasID);

        fontTexture = gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
            (uint)width,
            (uint)height,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled));
        fontTexture.Name = "ImGui.NET Font Texture";
        gd.UpdateTexture(
            fontTexture,
            pixels,
            (uint)(bytesPerPixel * width * height),
            0,
            0,
            0,
            (uint)width,
            (uint)height,
            1,
            0,
            0);
        fontTextureView = gd.ResourceFactory.CreateTextureView(fontTexture);

        io.Fonts.ClearTexData();
    }

    /// <summary>
    /// Renders the ImGui draw list data.
    /// This method requires a <see cref="GraphicsDevice"/> because it may create new DeviceBuffers if the size of vertex
    /// or index data has increased beyond the capacity of the existing buffers.
    /// A <see cref="CommandList"/> is needed to submit drawing and resource update commands.
    /// </summary>
    public void Render(GraphicsDevice gd, CommandList cl)
    {
        if (frameBegun)
        {
            frameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData(), gd, cl);
        }
    }

    /// <summary>
    /// Updates ImGui input and IO configuration state.
    /// </summary>
    public void Update(float deltaSeconds)
    {
        if (frameBegun)
        {
            ImGui.Render();
        }

        SetPerFrameImGuiData(deltaSeconds);

        frameBegun = true;
        ImGui.NewFrame();
    }

    /// <summary>
    /// Sets per-frame data based on the associated window.
    /// This is called by Update(float).
    /// </summary>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(
            windowWidth / scaleFactor.X,
            windowHeight / scaleFactor.Y);
        io.DisplayFramebufferScale = scaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }



    private static void SetKeyMappings()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
        io.KeyMap[(int)ImGuiKey.Space] = (int)Key.Space;
        io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
    }

    private void RenderImDrawData(ImDrawDataPtr drawdata, GraphicsDevice gd, CommandList cl)
    {
        uint vertexOffsetInVertices = 0;
        uint indexOffsetInElements = 0;

        if (drawdata.CmdListsCount == 0)
        {
            return;
        }

        uint totalVBSize = (uint)(drawdata.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
        if (totalVBSize > vertexBuffer.SizeInBytes)
        {
            gd.DisposeWhenIdle(vertexBuffer);
            vertexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalVBSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        }

        uint totalIBSize = (uint)(drawdata.TotalIdxCount * sizeof(ushort));
        if (totalIBSize > indexBuffer.SizeInBytes)
        {
            gd.DisposeWhenIdle(indexBuffer);
            indexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalIBSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        }

        for (int i = 0; i < drawdata.CmdListsCount; i++)
        {
            ImDrawListPtr cmdlist = drawdata.CmdListsRange[i];

            cl.UpdateBuffer(
                vertexBuffer,
                vertexOffsetInVertices * (uint)Unsafe.SizeOf<ImDrawVert>(),
                cmdlist.VtxBuffer.Data,
                (uint)(cmdlist.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()));

            cl.UpdateBuffer(
                indexBuffer,
                indexOffsetInElements * sizeof(ushort),
                cmdlist.IdxBuffer.Data,
                (uint)(cmdlist.IdxBuffer.Size * sizeof(ushort)));

            vertexOffsetInVertices += (uint)cmdlist.VtxBuffer.Size;
            indexOffsetInElements += (uint)cmdlist.IdxBuffer.Size;
        }

        // Setup orthographic projection matrix into our constant buffer
        ImGuiIOPtr io = ImGui.GetIO();
        Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(
            0f,
            io.DisplaySize.X,
            io.DisplaySize.Y,
            0.0f,
            -1.0f,
            1.0f);

        gd.UpdateBuffer(projMatrixBuffer, 0, ref mvp);

        cl.SetVertexBuffer(0, vertexBuffer);
        cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
        cl.SetPipeline(pipeline);
        cl.SetGraphicsResourceSet(0, mainResourceSet);

        drawdata.ScaleClipRects(io.DisplayFramebufferScale);

        // Render command lists
        int vtxoffset = 0;
        int idxoffset = 0;
        for (int n = 0; n < drawdata.CmdListsCount; n++)
        {
            ImDrawListPtr cmdlist = drawdata.CmdListsRange[n];
            for (int cmdi = 0; cmdi < cmdlist.CmdBuffer.Size; cmdi++)
            {
                ImDrawCmdPtr pcmd = cmdlist.CmdBuffer[cmdi];
                if (pcmd.UserCallback != IntPtr.Zero)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (pcmd.TextureId != IntPtr.Zero)
                    {
                        if (pcmd.TextureId == fontAtlasID)
                        {
                            cl.SetGraphicsResourceSet(1, fontTextureResourceSet);
                        }
                        else
                        {
                            cl.SetGraphicsResourceSet(1, GetImageResourceSet(pcmd.TextureId));
                        }
                    }

                    cl.SetScissorRect(
                        0,
                        (uint)pcmd.ClipRect.X,
                        (uint)pcmd.ClipRect.Y,
                        (uint)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                        (uint)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                    cl.DrawIndexed(pcmd.ElemCount, 1, pcmd.IdxOffset + (uint)idxoffset, (int)pcmd.VtxOffset + vtxoffset, 0);
                }
            }
            vtxoffset += cmdlist.VtxBuffer.Size;
            idxoffset += cmdlist.IdxBuffer.Size;
        }
    }

    /// <summary>
    /// Frees all graphics resources used by the renderer.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
        projMatrixBuffer.Dispose();
        fontTexture.Dispose();
        fontTextureView.Dispose();
        vertexShader.Dispose();
        fragmentShader.Dispose();
        layout.Dispose();
        textureLayout.Dispose();
        pipeline.Dispose();
        mainResourceSet.Dispose();

        foreach (IDisposable resource in ownedResources)
        {
            resource.Dispose();
        }
    }

    private struct ResourceSetInfo
    {
        public readonly IntPtr ImGuiBinding;
        public readonly ResourceSet ResourceSet;

        public ResourceSetInfo(IntPtr imGuiBinding, ResourceSet resourceSet)
        {
            ImGuiBinding = imGuiBinding;
            ResourceSet = resourceSet;
        }
    }
}
