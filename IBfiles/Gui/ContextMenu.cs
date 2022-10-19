namespace IBfiles.Gui;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

public class ContextMenu
{
    public static void Gui()
    {
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 6));
        if (ImGui.BeginPopupContextWindow("ContextMenu", ImGuiPopupFlags.MouseButtonRight))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 3));

            if (FileManager.Selections.Count != 0)
            {
                FileContext();
            }
            else
            {
                FolderContext();
            }

            ImGui.PopStyleVar();

            ImGui.EndPopup();
        }
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }

    private static void FileContext()
    {
        if (ImGui.Selectable("Delete"))
        {
            foreach (DirectoryEntry selection in FileManager.Selections)
            {
                EntryHandler.Delete(selection);
            }
        }
        ImGuiExt.CursorPointer();
    }

    private static void FolderContext()
    {
        if (ImGui.Selectable("New File"))
        {
            FileManager.NewFile();
        }
        ImGuiExt.CursorPointer();

        if (ImGui.Selectable("New Folder"))
        {
            FileManager.NewFolder();
        }
        ImGuiExt.CursorPointer();
    }
}
