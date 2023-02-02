namespace IBfiles.Gui.Views;

using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

using NativeFileDialogSharp;

public class SettingsView
{
    private float width;
    private float height;

    public void Gui()
    {
        if (ImGui.BeginChild("Settings", new(ImGuiExt.ReactiveWidth(0.7f, 500, 800), height)))
        {
            width = ImGui.GetContentRegionAvail().X;
            height = ImGui.GetContentRegionAvail().Y;

            ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.BackgroundInput);
            ImGui.PushStyleColor(ImGuiCol.Button, Colors.AccentDark);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.BackgroundInput);

            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2);

            if (ImGui.BeginTable("SettingsTable", 2, ImGuiTableFlags.None, new(width, 0)))
            {
                DisplaySettings(Settings.I);
                ImGui.EndTable();
            }

            CloseSettings();

            ImGui.PopStyleVar();

            ImGui.PopStyleColor(3);

            ImGui.EndChild();
        }
    }

    private void DisplaySettings(Settings settings)
    {
        DisplayBoolean(ref settings.TitleUsesFullPath, nameof(settings.TitleUsesFullPath));
        DisplayBoolean(ref settings.UseBackslashSeperator, nameof(settings.UseBackslashSeperator));
        DisplayNumber(ref settings.HeaderGap, nameof(settings.HeaderGap));
        DisplayBoolean(ref settings.FoldersFirst, nameof(settings.FoldersFirst));
        DisplayBoolean(ref settings.AlternateRowColors, nameof(settings.AlternateRowColors));
        DisplayPath(ref settings.StartDirectory, nameof(settings.StartDirectory));
        DisplayBoolean(ref settings.HideOpticalDrives, nameof(settings.HideOpticalDrives));
        DisplayBoolean(ref settings.DecimalFileSize, nameof(settings.DecimalFileSize));
        DisplayCommands(ref settings.FileCommands, nameof(settings.FileCommands));
        DisplayCommands(ref settings.FolderCommands, nameof(settings.FolderCommands));
    }

    private void DisplayCommands(ref List<Command> value, string name)
    {
        AddSettingLabel(name + ":");
        ImGui.PushID(name);

        ImGui.TableNextColumn();

        ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.BackgroundInput);

        for (int i = 0; i < value.Count; i++)
        {
            Command command = value[i];

            ImGui.TableNextColumn();

            ImGui.PushID("Command_DisplayName_" + i);
            ImGui.InputText(string.Empty, ref command.DisplayName, 256);
            ImGui.PopID();

            ImGui.TableNextColumn();

            ImGui.PushID("Command_File_" + i);
            ImGui.InputText(string.Empty, ref command.File, 256);
            ImGui.PopID();

            ImGui.SameLine();

            ImGui.PushID("Command_Args_" + i);
            ImGui.InputText(string.Empty, ref command.Args, 256);
            ImGui.PopID();
        }

        ImGui.TableNextColumn();

        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.AccentDark);
        if (ImGuiExt.Button("Add Item"))
        {
            value.Add(new("New Item", "File", "Args %1"));
        }

        ImGui.TableNextColumn();

        ImGui.PopStyleColor();
        ImGui.PopStyleVar();

        ImGui.PopStyleColor();
    }

    private void DisplayPath(ref FsPath value, string name)
    {
        AddSettingLabel(name);
        ImGui.PushID(name);

        ImGui.TableNextColumn();

        string input = value.Path;
        ImGui.InputText(string.Empty, ref input, 256);
        value.Path = input;

        ImGui.SameLine();

        if (ImGui.Button("Pick"))
        {
            DialogResult result = Dialog.FolderPicker(value);
            if (result.IsOk)
            {
                value.Path = result.Path;
            }
        }

        ImGuiExt.CursorPointer();
    }

    private void DisplayNumber(ref int value, string name)
    {
        AddSettingLabel(name);
        ImGui.PushID(name);

        ImGui.TableNextColumn();

        ImGui.PushStyleColor(ImGuiCol.Button, Colors.BackgroundDark);
        ImGui.InputInt(string.Empty, ref value);
        ImGui.PopStyleColor();

        ImGui.PopID();
    }

    private void DisplayBoolean(ref bool state, string name)
    {
        AddSettingLabel(name);
        ImGui.PushID(name);

        ImGui.TableNextColumn(); ImGui.Checkbox(string.Empty, ref state);
        ImGuiExt.CursorPointer();

        ImGui.PopID();
    }

    private static void AddSettingLabel(string name)
    {
        ImGui.TableNextColumn();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);
        string res = Regex.Replace(name, "([A-Z])", " $1").Trim();
        ImGui.Text(res);
    }

    private static unsafe void CloseSettings()
    {
        if (ImGuiExt.Button("Save", new(ImGui.GetContentRegionAvail().X, 0)))
        {
            Settings.Save();
        }
        ImGuiExt.CursorPointer();
    }
}
