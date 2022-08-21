namespace IBfiles.ImguiRenderer;

using ImGuiNET;

public static class GlobalStyle
{
    public static void Style()
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        style.WindowPadding = new(0, 0);
        style.WindowBorderSize = 0f;
        style.ItemSpacing = new(0, 0);

        style.FrameRounding = 6f;

        style.Colors[(int)ImGuiCol.Button] = new(0, 0, 0, 0);
        style.Colors[(int)ImGuiCol.ButtonHovered] = Colors.BackgroundLight;
        style.Colors[(int)ImGuiCol.ButtonActive] = Colors.BackgroundInput;
    }
}
