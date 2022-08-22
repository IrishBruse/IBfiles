namespace IBfiles.Gui;

using System.Numerics;

using IBfiles.Logic;

using ImGuiNET;

public static class SettingsGui
{
    public static unsafe void Gui()
    {
        Vector2 center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

        ImGui.SetNextWindowSize(new(200, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(6));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 6);
        {
            if (ImGuiExt.BeginPopupModal("Settings", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 6));
                {
                    ImGui.Text("TitleUsesFullPath ");
                }
                ImGui.PopStyleVar();
                ImGui.SameLine();
                ImGui.Checkbox("", ref Settings.I.TitleUsesFullPath);

                CloseSettings();
                ImGui.EndPopup();
            }
        }
        ImGui.PopStyleVar(2);
    }

    private static unsafe void CloseSettings()
    {
        ImGui.SetCursorPosX(75f);
        if (ImGui.Button("Close", new(50, 0)))
        {
            ImGui.CloseCurrentPopup();
        }
        ImGuiExt.CursorPointer();
    }
}
