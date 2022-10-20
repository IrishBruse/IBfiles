namespace IBfiles.Gui;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

public class ContextMenu
{
    public static void Gui()
    {
        // EntryContextGui();
        // DirectoryContextGui();
    }

    private static void EntryContextGui()
    {
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(2, 6));
        if (ImGui.BeginPopupContextItem("EntryContextMenu"))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 3));

            if (ImGui.Selectable("Delete"))
            {
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    EntryHandler.Delete(selection);
                }
            }
            ImGuiExt.CursorPointer();

            if (ImGui.Selectable("Properties"))
            {
                Console.WriteLine("test");
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    // FileProperties.ShowFileProperties(selection.Path);
                }
            }
            ImGuiExt.CursorPointer();

            ImGui.PopStyleVar();

            ImGui.EndPopup();
        }
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }


    private static void DirectoryContextGui()
    {
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 6));
        if (ImGui.BeginPopupContextItem("DirectoryContextMenu"))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 3));

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

            ImGui.PopStyleVar();

            ImGui.EndPopup();
        }
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }
}
