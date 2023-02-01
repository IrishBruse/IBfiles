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
    public void Gui()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2);
        FileManager.UpdateTitle();// For live settings changes

        ImGui.SetCursorPosX(ImGui.GetWindowWidth() * .25f);
        ImGui.SetCursorPosY(ImGui.GetWindowHeight() * .1f);

        _ = ImGui.BeginTable("SettingsTable", 2, ImGuiTableFlags.None, new(ImGui.GetContentRegionAvail().X * .7f, 0));
        {
            DisplaySettings(Settings.I);
        }
        ImGui.EndTable();

        CloseSettings();

        ImGui.PopStyleVar();
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
        DisplayCommand(ref settings.FileCommands, nameof(settings.FileCommands));
        DisplayCommand(ref settings.FolderCommands, nameof(settings.FolderCommands));
    }

    private void DisplayKeyValue(ref Dictionary<string, string> value, string name)
    {
        AddSettingLabel(name + ":");
        ImGui.PushID(name);

        _ = ImGui.TableNextColumn();

        ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.BackgroundInput);

        foreach ((string k, string v) in value)
        {
            _ = ImGui.TableNextColumn();

            ImGui.SetNextItemWidth(ImGui.GetColumnWidth());

            string inputk = k;
            ImGui.PushID(k);
            _ = ImGui.InputText("", ref inputk, 256);
            ImGui.PopID();

            _ = ImGui.TableNextColumn();

            ImGui.SetNextItemWidth(ImGui.GetColumnWidth());

            string inputv = v;
            ImGui.PushID(v);
            _ = ImGui.InputText("", ref inputv, 256);
            ImGui.PopID();
        }

        _ = ImGui.TableNextColumn();

        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.AccentDark);
        if (ImGui.Selectable("Add", false))
        {
            value.Add("", "");
        }
        ImGuiExt.CursorPointer();

        _ = ImGui.TableNextColumn();

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }

    private void DisplayCommand(ref Dictionary<string, Command> value, string name)
    {
        AddSettingLabel(name + ":");
        ImGui.PushID(name);

        _ = ImGui.TableNextColumn();

        ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.BackgroundInput);

        foreach ((string k, Command v) in value)
        {
            _ = ImGui.TableNextColumn();

            ImGui.SetNextItemWidth(ImGui.GetColumnWidth());

            string inputk = k;
            ImGui.PushID(k);
            _ = ImGui.InputText("", ref inputk, 256);
            ImGui.PopID();

            _ = ImGui.TableNextColumn();

            ImGui.SetNextItemWidth(ImGui.GetColumnWidth());

            string inputv = v.File;
            ImGui.PushID(v.File);
            _ = ImGui.InputText("", ref inputv, 256);
            v.File = inputv;
            ImGui.PopID();
        }

        _ = ImGui.TableNextColumn();

        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.AccentDark);
        if (ImGui.Selectable("Add", false))
        {
            value.Add("Display Text", new("File", "Args %1"));
        }
        ImGuiExt.CursorPointer();

        _ = ImGui.TableNextColumn();

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }

    private void DisplayPath(ref FsPath value, string name)
    {
        AddSettingLabel(name);
        ImGui.PushID(name);

        _ = ImGui.TableNextColumn();

        string input = value.Path;
        _ = ImGui.InputText(string.Empty, ref input, 256);
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

        _ = ImGui.TableNextColumn();

        _ = ImGui.InputInt(string.Empty, ref value);

        ImGuiExt.CursorPointer();

        ImGui.PopID();
    }

    private void DisplayBoolean(ref bool state, string name)
    {
        AddSettingLabel(name);
        ImGui.PushID(name);

        _ = ImGui.TableNextColumn(); _ = ImGui.Checkbox("", ref state);
        ImGuiExt.CursorPointer();

        ImGui.PopID();
    }

    private static void AddSettingLabel(string name)
    {
        _ = ImGui.TableNextColumn();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);
        string res = Regex.Replace(name, "([A-Z])", " $1").Trim();
        ImGui.Text(res);
    }

    private static unsafe void CloseSettings()
    {
        ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X * .25f);
        if (ImGui.Button("Save", new(ImGui.GetWindowWidth() * .5f, 0)))
        {
            Settings.Save();
        }
        ImGuiExt.CursorPointer();
    }
}
