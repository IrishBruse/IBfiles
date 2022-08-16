namespace IBfiles;

using System.Numerics;

using ImGuiNET;

public class FileManager
{
    public static void Submit()
    {

        ImGuiIOPtr io = ImGui.GetIO();
        Vector2 windowSize = io.DisplaySize;
        ImGui.SetNextWindowSize(size: windowSize);
        ImGui.SetNextWindowPos(Vector2.Zero);

        ImGui.Begin("Viewport", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs);
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(1, 0, 1, 1));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 0);
            ImGui.BeginChild("Navbar", new(io.DisplaySize.X, 40));
            {
                ImGui.Text("test");
            }
            ImGui.EndChild();
            ImGui.PopStyleColor();
        }
        ImGui.End();

    }
}
