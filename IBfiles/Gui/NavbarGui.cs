namespace IBfiles.Gui;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

public static class NavbarGui
{
    private const int ButtonSize = 40;
    private const int ButtonPadding = 20;
    private static bool editingNavbarLocation;
    private static float navbarWidth;

    public static void Gui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(3f));
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundNormal);
        {
            _ = ImGui.BeginChild("Navbar", new(io.DisplaySize.X, 46));
            {
                Content();
            }
            ImGui.EndChild();
        }
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }

    private static void Content()
    {
        ImGui.PushFont(Application.Icons26Font);
        {
            NavbarButton(CodiconUnicode.ChevronLeft, FileManager.HistoryBack);
            NavbarButton(CodiconUnicode.ChevronRight, FileManager.HistoryForward);
            NavbarButton(CodiconUnicode.ChevronUp, FileManager.UpDirectoryLevel);

            ImGui.SameLine();
            float width = ImGui.GetWindowWidth() - (ImGui.GetCursorPosX() * 2f) - 6;

            ImGui.PushItemWidth(width);

            ImGui.PushFont(Application.Cascadia13Font);
            {
                ImGui.SameLine();
                _ = ImGui.BeginChild("test", new(width, ButtonSize));
                ImGui.SetCursorPosX((width - navbarWidth) / 2f);
                if (editingNavbarLocation)
                {
                    EditingNavbar();
                }
                else
                {
                    DisplayNavbar();
                }
                ImGui.EndChild();

                ImGui.SetCursorPosY(2.5f);
            }
            ImGui.PopFont();

            ImGui.PopItemWidth();

            NavbarButton(CodiconUnicode.Refresh, () => Console.WriteLine("Reloaded"));
            NavbarButton(CodiconUnicode.Search, () => Console.WriteLine("Search"));
            NavbarButton(CodiconUnicode.Menu, () => ImGui.OpenPopup("Settings"));
        }
        ImGui.PopFont();

        SettingsGui.Gui();
    }

    private static void NavbarButton(CodiconUnicode icon, Action callback)
    {
        ImGui.SameLine();
        ImGui.SetCursorPosY(3);
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 3);
        if (ImGui.Button($"{(char)icon}", new(ButtonSize)))
        {
            callback.Invoke();
        }
        ImGuiExt.CursorPointer();
    }

    private static void EditingNavbar()
    {
        ImGui.SetKeyboardFocusHere();
        string newPath = FileManager.CWD;
        if (ImGui.InputText("", ref newPath, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (Directory.Exists(newPath))
            {
                FileManager.CWD = newPath;
            }
            else
            {
                // TODO Popup error
            }
            editingNavbarLocation = false;
        }
    }

    private static void DisplayNavbar()
    {

        string path = FileManager.CWD;

        float startX = ImGui.GetCursorPosX();

        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows))
        {
            editingNavbarLocation = true;
        }
        ImGui.SetCursorPosY(8f + (13f / 2));
        _ = ImGui.GetFontSize();

        string[] paths;

        if (string.IsNullOrEmpty(path))
        {
            paths = Array.Empty<string>();
        }
        else if (path.Contains('/'))
        {
            paths = path.Split('/');
        }
        else
        {
            paths = path.Split('\\');
        }

        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.BackgroundInput);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4);
        {
            for (int i = 0; i < paths.Length; i++)
            {
                string p = paths[i];

                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }

                ImGui.PushID(p + i);
                {
                    if (ImGui.Button(p))
                    {
                        string fsPath = "";
                        for (int j = 0; j <= i; j++)
                        {
                            fsPath += paths[j] + (Settings.I.UseBackslashSeperator ? '\\' : '/');
                        }
                        FileManager.CWD = fsPath;
                    }
                }
                ImGui.PopID();

                ImGuiExt.CursorPointer();

                ImGui.SameLine();

                ImGui.Text(Settings.I.UseBackslashSeperator ? " \\ " : " / ");

                ImGui.SameLine();
            }
        }
        ImGui.PopStyleVar();
        ImGui.PopStyleColor();

        navbarWidth = ImGui.GetCursorPosX() - startX;
    }
}
