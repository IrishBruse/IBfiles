namespace IBfiles.Gui;

using IBfiles.ImguiRenderer;
using IBfiles.Logic;

using ImGuiNET;

public static class Navbar
{
    private const int ButtonSize = 40;
    private const int ButtonPadding = 20;
    private static bool editingNavbarLocation;
    private static float navbarWidth;

    public static void Gui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundNormal);
        _ = ImGui.BeginChild("Navbar", new(io.DisplaySize.X, 45));
        {
            Content();
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    private static void Content()
    {
        ImGui.PushFont(Application.Icons26Font);
        {
            NavbarButton(CodiconUnicode.ChevronLeft, FileManager.HistoryBack);
            NavbarButton(CodiconUnicode.ChevronRight, FileManager.HistoryForward);
            NavbarButton(CodiconUnicode.ChevronUp, FileManager.UpDirectoryLevel);

            ImGui.SameLine();
            float width = ImGui.GetWindowWidth() - (ImGui.GetCursorPosX() * 2f);

            ImGui.PushItemWidth(width);

            ImGui.PushFont(Application.Cascadia13Font);
            {
                ImGui.SameLine();
                if (editingNavbarLocation)
                {
                    EditingNavbar();
                }
                else
                {
                    _ = ImGui.BeginChild("test", new(width, ButtonSize));
                    ImGui.Dummy(new((width - navbarWidth) / 2f, ImGui.GetTextLineHeight()));
                    DisplayNavbar();
                    ImGui.EndChild();
                }

                ImGui.SetCursorPosY(2.5f);
            }
            ImGui.PopFont();

            ImGui.PopItemWidth();

            NavbarButton(CodiconUnicode.Refresh, () => Console.WriteLine("Reloaded"));
            NavbarButton(CodiconUnicode.Search, () => Console.WriteLine("Search"));
            NavbarButton(CodiconUnicode.Menu, () => ImGui.OpenPopup("Settings"));
        }
        ImGui.PopFont();

        Settings.Gui();
    }

    private static void NavbarButton(CodiconUnicode icon, Action callback)
    {
        // ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(2.5f));
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
            if (ImGui.Button($"{(char)icon}", new(ButtonSize)))
            {
                callback.Invoke();
            }
            ImGuiExt.CursorPointer();
        }
        // ImGui.PopStyleVar();
    }

    private static void EditingNavbar()
    {
        ImGui.SetKeyboardFocusHere();
        string newPath = Environment.CurrentDirectory;
        if (ImGui.InputText("", ref newPath, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (Directory.Exists(newPath))
            {
                Environment.CurrentDirectory = newPath;
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
        ImGui.SameLine();

        string path = Environment.CurrentDirectory;

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
        // ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2));
        // ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0));
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
                        fsPath += paths[j] + '/';
                    }
                    Console.WriteLine(fsPath);
                }
            }
            ImGui.PopID();

            ImGuiExt.CursorPointer();

            ImGui.SameLine();

            ImGui.Text(" / ");

            ImGui.SameLine();
        }
        ImGui.PopStyleVar();
        ImGui.PopStyleColor();

        navbarWidth = ImGui.GetCursorPosX() - startX;
    }
}
