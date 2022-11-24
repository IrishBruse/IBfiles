namespace IBfiles.Gui;

using System.Runtime.InteropServices;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

using Vanara.PInvoke;

public class ContextMenu
{
    public static void FolderContextMenu()
    {
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);

        if (ImGui.BeginPopupContextItem("FolderContextMenu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverExistingPopup))
        {
            ImGui.Dummy(new(0, 2));

            if (ImGui.Selectable("New File..."))
            {
                FileManager.NewFile();
            }
            ImGuiExt.CursorPointer();

            if (ImGui.Selectable("New Folder..."))
            {
                FileManager.NewFolder();
            }
            ImGuiExt.CursorPointer();

            ImGui.Separator();

            ImGui.Dummy(new(0, 2));
            ImGui.EndPopup();
        }

        ImGui.PopStyleColor();
    }

    public static void EntryContextMenu()
    {
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);

        if (ImGui.BeginPopupContextItem("EntryContextMenu", ImGuiPopupFlags.MouseButtonRight))
        {
            ImGui.Dummy(new(0, 2));
            if (ImGui.Selectable("Delete"))
            {
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    EntryHandler.Delete(selection);
                }
            }
            ImGuiExt.CursorPointer();

            ImGui.Separator();

            if (ImGui.Selectable("Properties..."))
            {
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    ShowProperties(selection.Path);
                }
            }
            ImGuiExt.CursorPointer();
            ImGui.Dummy(new(0, 2));
            ImGui.EndPopup();
        }

        ImGui.PopStyleColor();
    }

    private static void ShowProperties(string filepath)
    {
        Shell32.SHELLEXECUTEINFO info = new();

        info.cbSize = Marshal.SizeOf(info);
        info.lpVerb = "properties";
        info.lpFile = filepath;
        info.nShellExecuteShow = ShowWindowCommand.SW_SHOW;
        info.fMask = Shell32.ShellExecuteMaskFlags.SEE_MASK_INVOKEIDLIST;

        _ = Shell32.ShellExecuteEx(ref info);
    }
}
