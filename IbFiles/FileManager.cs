namespace IBfiles;

using System.Numerics;

using ImGuiNET;

public class FileManager
{
    public void Submit()
    {
        Vector2 windowSize = ImGui.GetIO().DisplaySize;
        ImGui.SetNextWindowSize(windowSize);
        ImGui.SetNextWindowPos(Vector2.Zero);

        _ = ImGui.Begin("Viewport", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground);
        _ = ImGui.BeginTable("FileList", 3);
        for (int i = 0; i < 10; i++)
        {
            ImGui.TableNextColumn();
            ImGui.Text("A " + i);
            ImGui.TableNextColumn();
            ImGui.Text("B " + i);
            ImGui.TableNextColumn();
            ImGui.Text("C " + i);
            ImGui.TableNextRow();
        }
        ImGui.EndTable();
        ImGui.End();

        ImGui.ShowDemoWindow();
    }
}
