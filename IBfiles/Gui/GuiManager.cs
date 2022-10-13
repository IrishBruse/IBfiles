namespace IBfiles.Gui;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Gui.Views;
using IBfiles.Logic;

using ImGuiNET;

public class GuiManager
{
    private static NavbarGui navbar = new();

    private static FolderView folderView = new();
    private static SettingsView settingsView = new();
    private static HomeView homeView = new();

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
                navbar.Gui();
                Content();
            }
            ImGui.End();
        }
        ImGui.PopStyleVar(2);
    }

    private static void Content()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundDark);
        float height = ImGui.GetWindowHeight() - ImGui.GetCursorPosY();
        float width = io.DisplaySize.X;

        _ = ImGui.BeginChild("View", new(width, height));
        {
            switch (FileManager.CurrentPageType)
            {
                case Page.Directory: folderView.Gui(); break;
                case Page.Home: homeView.Gui(); break;
                case Page.Settings: settingsView.Gui(); break;
            }

            // Right click menu
            ContextMenu.Gui();

        }
        ImGui.EndChild();
    }
}
