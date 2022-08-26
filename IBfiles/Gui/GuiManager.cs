namespace IBfiles.Gui;

using System.Numerics;

using ImGuiNET;

public class GuiManager
{
    public static void Submit()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        Vector2 windowSize = io.DisplaySize;
        ImGui.SetNextWindowSize(windowSize);
        ImGui.SetNextWindowPos(Vector2.Zero);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0));
        {
            _ = ImGui.Begin("Viewport", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs);
            {
                NavbarGui.Gui();
                FolderView.Gui();
            }
            ImGui.End();
        }
        ImGui.PopStyleVar(2);

        ImGui.ShowMetricsWindow();
    }
}
