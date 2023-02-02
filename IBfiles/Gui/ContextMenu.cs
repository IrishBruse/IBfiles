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
            if (ImGuiExt.Selectable(SpacePadding + "New File..." + SpacePadding))
            {
                FileManager.NewFile();
            }

            if (ImGuiExt.Selectable(SpacePadding + "New Folder..." + SpacePadding))
            {
                FileManager.NewFolder();
            }

            ImGui.Separator();

            foreach (Command command in Settings.I.FolderCommands)
            {
                if (ImGuiExt.Selectable(SpacePadding + command.DisplayName + SpacePadding))
                {
                    CommandHandler.Run(command.File, command.Args, FileManager.CurrentDirectory);
                }
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
            if (ImGuiExt.Selectable(SpacePadding + "Delete" + SpacePadding))
            {
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    EntryHandler.Delete(selection);
                }
            }

            ImGui.Separator();

            if (entry.IsFile)
            {
                foreach (Command command in Settings.I.FileCommands)
                {
                    if (ImGuiExt.Selectable(SpacePadding + command.DisplayName + SpacePadding))
                    {
                        CommandHandler.Run(command.File, command.Args, entry.Path);
                    }
                }
            }
            else
            {
                foreach (Command command in Settings.I.FolderCommands)
                {
                    if (ImGuiExt.Selectable(SpacePadding + command.DisplayName + SpacePadding))
                    {
                        CommandHandler.Run(command.File, command.Args, entry.Path);
                    }
                }
            }

            ImGui.Separator();

            if (ImGuiExt.Selectable(SpacePadding + "Properties..." + SpacePadding))
            {
                foreach (DirectoryEntry selection in FileManager.Selections)
                {
                    ShowProperties(selection.Path);
                }
            }
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

        Shell32.ShellExecuteEx(ref info);
    }
}
