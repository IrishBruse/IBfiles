namespace IBfiles.Gui;

using System;
using System.Numerics;
using System.Reflection;

using IBfiles.Logic;

using ImGuiNET;

public static class SettingsGui
{
    public static void Gui()
    {
        Vector2 center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(6));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 6);
        {
            if (ImGuiExt.BeginPopupModal("Settings", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
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
    }

    private static void DisplaySetting(FieldInfo field)
    {
        switch (field.FieldType.Name)
        {
            case nameof(Boolean):
            AddSettingLabel(field.Name);
            {
                bool val = (bool)field.GetValue(Settings.I);
                ImGui.TableNextColumn(); ImGui.Checkbox("", ref val);
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
        _ = ImGui.TableNextColumn(); ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4); ImGui.Text(name + " ");
        ImGui.PushID(name);
    }

    private static unsafe void CloseSettings()
    {
        if (ImGui.Button("Close", new(ImGui.CalcItemWidth(), 0)))
        {
            Settings.Save();
            ImGui.CloseCurrentPopup();
        }
        ImGuiExt.CursorPointer();
    }
}
