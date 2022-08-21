namespace IBfiles.Gui;

using System.Numerics;

using ImGuiNET;

public static class Settings
{
    public static unsafe void Gui()
    {
        Vector2 center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(6));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 6);
        {
            if (ImGuiExt.BeginPopupModal("Settings", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoMove))
            {
                if (ImGui.Button("Close"))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGuiExt.CursorPointer();

                ImGui.EndPopup();
            }
        }
        ImGui.PopStyleVar(2);
    }
}
