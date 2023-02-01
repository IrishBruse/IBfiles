namespace IBfiles.Gui;

using System.Numerics;
using System.Runtime.InteropServices;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

using Vanara.PInvoke;

public class ContextMenu
{
    private const string SpacePadding = "  ";

    public static void FolderContextMenu()
    {
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 6));

        if (ImGui.BeginPopupContextItem("FolderContextMenu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverExistingPopup))
        {
            if (ImGui.Selectable(SpacePadding + "New File..." + SpacePadding))
            {
                FileManager.NewFile();
            }
            ImGuiExt.CursorPointer();

            if (ImGui.Selectable(SpacePadding + "New Folder..." + SpacePadding))
            {
                FileManager.NewFolder();
            }
            ImGuiExt.CursorPointer();

            ImGui.Separator();

            foreach (Command command in Settings.I.FolderCommands)
            {
                if (ImGui.Selectable(SpacePadding + command.DisplayName + SpacePadding))
                {
                    CommandHandler.Run(command.File, command.Args, FileManager.CurrentDirectory);
                }
                ImGuiExt.CursorPointer();
            }

            ImGui.EndPopup();
        }

        ImGui.PopStyleVar();
        ImGui.PopStyleColor();
    }

    public static void EntryContextMenu(DirectoryEntry entry)
    {
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 6));

        if (ImGui.BeginPopupContextItem("EntryContextMenu", ImGuiPopupFlags.MouseButtonRight))
        {
            if (ImGui.Selectable(SpacePadding + "Delete" + SpacePadding))
            {
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    EntryHandler.Delete(selection);
                }
            }
            ImGuiExt.CursorPointer();

            ImGui.Separator();

            if (entry.IsFile)
            {
                foreach (Command command in Settings.I.FileCommands)
                {
                    if (ImGui.Selectable(SpacePadding + command.DisplayName + SpacePadding))
                    {
                        CommandHandler.Run(command.File, command.Args, entry.Path);
                    }
                    ImGuiExt.CursorPointer();
                }
            }
            else
            {
                foreach (Command command in Settings.I.FolderCommands)
                {
                    if (ImGui.Selectable(SpacePadding + command.DisplayName + SpacePadding))
                    {
                        CommandHandler.Run(command.File, command.Args, entry.Path);
                    }
                    ImGuiExt.CursorPointer();
                }
            }

            ImGui.Separator();

            if (ImGui.Selectable(SpacePadding + "Properties..." + SpacePadding))
            {
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    ShowProperties(selection.Path);
                }
            }
            ImGuiExt.CursorPointer();
            ImGui.EndPopup();
        }

        ImGui.PopStyleVar();
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
