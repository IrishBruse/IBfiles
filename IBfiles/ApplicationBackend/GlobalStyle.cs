namespace IBfiles.ApplicationBackend;

using ImGuiNET;

public static class GlobalStyle
{
    public static void Style()
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        style.WindowPadding = new(0, 0);
        style.WindowBorderSize = 0f;
        style.ItemSpacing = new(0, 0);

        style.ScrollbarSize = 20;
        style.ScrollbarRounding = 0;
        style.FramePadding = new(4f, 4f);

        style.PopupBorderSize = 0f;
        style.PopupRounding = 6f;

        style.FrameRounding = 6f;
        System.Numerics.Vector4 error = Colors.Error;

        style.Colors[(int)ImGuiCol.Text] = Colors.Text;
        style.Colors[(int)ImGuiCol.TextDisabled] = Colors.TextDisabled;
        style.Colors[(int)ImGuiCol.WindowBg] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.ChildBg] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.PopupBg] = Colors.BackgroundLight;
        style.Colors[(int)ImGuiCol.Border] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.BorderShadow] = Colors.BackgroundDark;
        style.Colors[(int)ImGuiCol.FrameBg] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.FrameBgHovered] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.FrameBgActive] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.TitleBg] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.TitleBgActive] = Colors.BackgroundLight;
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.MenuBarBg] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.ScrollbarBg] = Colors.BackgroundLight;
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = Colors.Scrollbar;
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = Colors.ScrollbarHover;
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = Colors.Scrollbar;
        style.Colors[(int)ImGuiCol.CheckMark] = Colors.AccentLight;
        style.Colors[(int)ImGuiCol.SliderGrab] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.SliderGrabActive] = Colors.AccentLight;
        style.Colors[(int)ImGuiCol.Button] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.ButtonHovered] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.ButtonActive] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.Header] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.HeaderHovered] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.HeaderActive] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.Separator] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.SeparatorHovered] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.SeparatorActive] = Colors.AccentLight;
        style.Colors[(int)ImGuiCol.ResizeGrip] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = Colors.AccentLight;
        style.Colors[(int)ImGuiCol.ResizeGripActive] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.Tab] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.TabHovered] = Colors.BackgroundLight;
        style.Colors[(int)ImGuiCol.TabActive] = Colors.BackgroundDark;
        style.Colors[(int)ImGuiCol.TabUnfocused] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.TabUnfocusedActive] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.DockingPreview] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.DockingEmptyBg] = Colors.BackgroundNormal;
        style.Colors[(int)ImGuiCol.PlotLines] = error;
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = error;
        style.Colors[(int)ImGuiCol.PlotHistogram] = error;
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = error;
        style.Colors[(int)ImGuiCol.TableHeaderBg] = Colors.BackgroundDark;
        style.Colors[(int)ImGuiCol.TableBorderStrong] = Colors.BackgroundInput;
        style.Colors[(int)ImGuiCol.TableBorderLight] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.TableRowBg] = Colors.BackgroundDark;
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = Colors.BackgroundDark;
        style.Colors[(int)ImGuiCol.TextSelectedBg] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.DragDropTarget] = Colors.AccentDark;
        style.Colors[(int)ImGuiCol.NavHighlight] = error;
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = error;
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = error;
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = Colors.BackgroundDim;
    }
}
