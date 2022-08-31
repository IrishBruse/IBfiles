namespace IBfiles.Gui;

using System;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

using NativeFileDialogSharp;

public static class SettingsGui
{
    public static void Gui()
    {
        Vector2 center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

        ImGui.PushStyleColor(ImGuiCol.PopupBg, Colors.BackgroundLight);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(6));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 6);
        {
            if (ImGuiExt.BeginPopupModal("Settings", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
            {
                FileManager.UpdateTitle();// For live settings changes

                _ = ImGui.BeginTable("SettingsTable", 2);
                {
                    FieldInfo[] fields = Settings.I.GetType().GetFields();

                    foreach (FieldInfo field in fields)
                    {
                        DisplaySetting(field);
                    }
                }
                ImGui.EndTable();

                CloseSettings();
                ImGui.EndPopup();
            }
        }
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();
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
                _ = ImGui.InputText("", ref val, 256);// https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry
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
        if (ImGui.Button("Close", new(ImGui.GetContentRegionAvail().X, 0)))
        {
            Settings.Save();
            ImGui.CloseCurrentPopup();
        }
        ImGuiExt.CursorPointer();
    }
}
