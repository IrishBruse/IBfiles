namespace IBfiles.Gui;

using IBfiles.ApplicationBackend;

using ImGuiNET;

public static class FolderView
{
    public static void Gui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundDark);
        _ = ImGui.BeginChild("FolderView", new(io.DisplaySize.X, ImGui.GetWindowHeight() - 46));
        {
            ImGui.Text("test");
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }
}
