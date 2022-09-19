namespace IBfiles.ApplicationBackend;

using System.Text.Json;

using IBfiles.Utilities;

using Veldrid;
using Veldrid.ImageSharp;

public class IconManager
{
    private static IntPtr FileImage { get; set; }
    private static IntPtr FolderImage { get; set; }

    private static GraphicsDevice GraphicsDevice { get; set; }
    private static ImGuiController Controller { get; set; }

    private static Dictionary<string, string> extensionToIcon;
    private static Dictionary<string, string> folderToIcon;
    private static Dictionary<string, string> fileToIcon;
    private static Dictionary<string, IntPtr> ptrCache = new();

    public static void Load(GraphicsDevice graphicsDevice, ImGuiController controller)
    {
        GraphicsDevice = graphicsDevice;
        Controller = controller;

        FileImage = GetIconPtrDirectly("file");
        FolderImage = GetIconPtrDirectly("folder");

        extensionToIcon = JsonSerializer.Deserialize<Dictionary<string, string>>(ResourceLoader.GetEmbeddedResourceText("Assets/ExtensionAssociation.json"));
        folderToIcon = JsonSerializer.Deserialize<Dictionary<string, string>>(ResourceLoader.GetEmbeddedResourceText("Assets/FolderAssociation.json"));
        fileToIcon = JsonSerializer.Deserialize<Dictionary<string, string>>(ResourceLoader.GetEmbeddedResourceText("Assets/FileAssociation.json"));
    }

    public static IntPtr GetFileIcon(string filename)
    {
        filename = filename.ToLower();
        string ext = Path.GetExtension(filename).Replace(".", string.Empty);

        if (extensionToIcon.TryGetValue(ext, out string icon))
        {
            return GetIconPtrDirectly(icon);
        }
        else if (fileToIcon.TryGetValue(Path.GetFileName(filename), out string fileIcon))
        {
            return GetIconPtrDirectly(fileIcon);
        }
        else
        {
            return FileImage;
        }
    }

    public static IntPtr GetFolderIcon(string filename)
    {
        filename = filename.ToLower();
        string folder = Path.GetFileName(filename);

        if (folderToIcon.TryGetValue(folder, out string icon))
        {
            return GetIconPtrDirectly("Folder/" + icon);
        }
        else
        {
            return FolderImage;
        }
    }

    public static IntPtr GetIconPtrDirectly(string icon)
    {
        string path = "Assets/Icons/" + icon + ".png";
        if (ptrCache.TryGetValue(path, out IntPtr value))
        {
            return value;
        }
        else
        {
            ImageSharpTexture texture = new(ResourceLoader.GetEmbeddedResourceStream(path));
            Texture dTexture = texture.CreateDeviceTexture(GraphicsDevice, GraphicsDevice.ResourceFactory);
            IntPtr ptr = Controller.GetOrCreateImGuiBinding(GraphicsDevice.ResourceFactory, dTexture);
            ptrCache.Add(path, ptr);
            return ptr;
        }
    }
}
