namespace IBfiles.ApplicationBackend;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using IBfiles.ApplicationBackend.ImageSharp;
using IBfiles.Utilities;

using Veldrid;

public class IconManager
{
    private static IntPtr FileImage { get; set; }
    private static IntPtr FolderImage { get; set; }

    private static GraphicsDevice GraphicsDevice { get; set; }
    private static ImGuiController Controller { get; set; }

    private static Dictionary<string, string> extensionToIcon;
    private static Dictionary<string, string> folderToIcon;
    private static Dictionary<string, string> fileToIcon;
    private static Dictionary<string, IntPtr> ptrCache = [];

    public static void Load(GraphicsDevice graphicsDevice, ImGuiController controller)
    {
        GraphicsDevice = graphicsDevice;
        Controller = controller;

        FileImage = GetIconExtensionPtrDirectly("file");
        FolderImage = GetIconExtensionPtrDirectly("folder");

        extensionToIcon = JsonSerializer.Deserialize<Dictionary<string, string>>(ResourceLoader.GetEmbeddedResourceText("Assets/ExtensionAssociation.json"));
        folderToIcon = JsonSerializer.Deserialize<Dictionary<string, string>>(ResourceLoader.GetEmbeddedResourceText("Assets/FolderAssociation.json"));
        fileToIcon = JsonSerializer.Deserialize<Dictionary<string, string>>(ResourceLoader.GetEmbeddedResourceText("Assets/FileAssociation.json"));
    }

    public static IntPtr GetFileIcon(string filePath)
    {
        filePath = filePath.ToLower(System.Globalization.CultureInfo.CurrentCulture);
        string ext = Path.GetExtension(filePath).Replace(".", string.Empty);
        string filename = Path.GetFileName(filePath);

        if (fileToIcon.TryGetValue(filename, out string fileIcon))
        {
            return GetIconExtensionPtrDirectly(fileIcon);
        }
        else if (extensionToIcon.TryGetValue(ext, out string icon))
        {
            return GetIconExtensionPtrDirectly(icon);
        }
        else
        {
            return FileImage;
        }
    }

    public static IntPtr GetFolderIcon(string filename)
    {
        filename = filename.ToLower(System.Globalization.CultureInfo.CurrentCulture);
        string folder = Path.GetFileName(filename);

        if (folderToIcon.TryGetValue(folder, out string icon))
        {
            return GetIconExtensionPtrDirectly("Folder/" + icon);
        }
        else
        {
            return FolderImage;
        }
    }

    public static IntPtr GetIconExtensionPtrDirectly(string icon)
    {
        string path = "Assets/Icons/" + icon + ".png";
        if (ptrCache.TryGetValue(path, out IntPtr value))
        {
            return value;
        }
        else
        {
            Stream stream = ResourceLoader.GetEmbeddedResourceStream(path);
            ImageSharpTexture texture = new(stream);
            Texture dTexture = texture.CreateDeviceTexture(GraphicsDevice, GraphicsDevice.ResourceFactory);
            IntPtr ptr = Controller.GetOrCreateImGuiBinding(GraphicsDevice.ResourceFactory, dTexture);
            ptrCache.Add(path, ptr);
            return ptr;
        }
    }
}
