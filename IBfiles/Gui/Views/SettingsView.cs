namespace IBfiles.Gui.Views;

using System;
using System.Reflection;
using System.Text.RegularExpressions;

using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

using NativeFileDialogSharp;

public class SettingsView
{
    public void Gui()
    {
        FileManager.UpdateTitle();// For live settings changes

        ImGui.SetCursorPosX(ImGui.GetWindowWidth() * .25f);
        ImGui.SetCursorPosY(ImGui.GetWindowHeight() * .1f);

        _ = ImGui.BeginTable("SettingsTable", 2, ImGuiTableFlags.None, new(ImGui.GetContentRegionAvail().X * .7f, 0));
        {
            FieldInfo[] fields = Settings.I.GetType().GetFields();

            foreach (FieldInfo field in fields)
            {
                DisplaySetting(field);
            }
        }
        ImGui.EndTable();

        CloseSettings();
    }

    private static void DisplaySetting(FieldInfo field)
    {
        switch (field.FieldType.Name)
        {
            case nameof(Boolean):
            AddSettingLabel(field.Name);
            ImGui.PushID(field.Name);
            {
                bool val = (bool)field.GetValue(Settings.I);
                _ = ImGui.TableNextColumn(); _ = ImGui.Checkbox("", ref val);
                field.SetValue(Settings.I, val);
            }
            ImGui.PopID();
            ImGuiExt.CursorPointer();
            break;

            case nameof(Int16):
            case nameof(Int32):
            case nameof(Int64):
            AddSettingLabel(field.Name);
            ImGui.PushID(field.Name);
            {
                int val = (int)field.GetValue(Settings.I);
                _ = ImGui.TableNextColumn();
                ImGui.PushItemWidth(90);
                _ = ImGui.InputInt("", ref val);
                ImGui.PopItemWidth();
                field.SetValue(Settings.I, val);
            }
            ImGui.PopID();
            ImGuiExt.CursorPointer();
            break;

            case nameof(String):
            AddSettingLabel(field.Name);
            ImGui.PushID(field.Name);
            {
                string val = (string)field.GetValue(Settings.I) ?? "";
                _ = ImGui.TableNextColumn();
                ImGui.PushItemWidth(90);
                // https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry
                _ = ImGui.InputText("", ref val, 256);
                ImGui.PopItemWidth();
                field.SetValue(Settings.I, val);
            }
            ImGui.PopID();
            ImGuiExt.CursorPointer();
            break;

            case nameof(FsPath):
            AddSettingLabel(field.Name);
            ImGui.PushID(field.Name);
            {
                FsPath val = (FsPath)field.GetValue(Settings.I);
                _ = ImGui.TableNextColumn();
                if (ImGui.Button(val))
                {
                    DialogResult result = Dialog.FolderPicker(val);
                    if (result.IsOk)
                    {
                        val.Path = result.Path;
                    }
                }
                ImGui.PopItemWidth();
                field.SetValue(Settings.I, val);
            }
            ImGui.PopID();
            ImGuiExt.CursorPointer();
            break;

            default: throw new NotImplementedException();
        }
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
        if (ImGui.Button("Save and Close", new(ImGui.GetWindowWidth() * .5f, 0)))
        {
            Settings.Save();
        }
        ImGuiExt.CursorPointer();
    }
}
