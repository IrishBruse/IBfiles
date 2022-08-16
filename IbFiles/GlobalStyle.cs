namespace IBfiles;

using ImGuiNET;

public static class GlobalStyle
{
    public static void Style()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        style.WindowPadding = new(0, 0);
    }
}
