namespace IBfiles.Gui;

using System.Numerics;

using ImGuiNET;

public static class Navbar
{
    private const int ButtonSize = 40;
    private const int ButtonPadding = 20;
    private static bool editingNavbarLocation;
    private static float navbarWidth;
    private static string path = "";

    public static void Gui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundNormal);
        ImGui.BeginChild("Navbar", new(io.DisplaySize.X, 45));
        {
            Content();
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    private static void Content()
    {
        ImGui.SetCursorPosY(2.5f);

        ImGui.PushFont(Application.Icons26Font);

        NavbarButton(CodiconUnicode.ChevronLeft, () =>
        {

        });

        NavbarButton(CodiconUnicode.ChevronRight, () =>
        {

        });

        NavbarButton(CodiconUnicode.ChevronUp, () =>
        {

        });

        ImGui.SameLine();
        int width = (int)ImGui.GetWindowWidth();
        width -= (int)ImGui.GetCursorPosX();
        const int buttonCount = 3;
        width -= ButtonSize * buttonCount;
        width -= ButtonPadding * buttonCount;

        ImGui.PushItemWidth(width);

        ImGui.PushFont(Application.Cascadia13Font);

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

        ImGui.PopFont();

        NavbarButton(CodiconUnicode.Refresh, () => Console.WriteLine("Reloaded"));

        NavbarButton(CodiconUnicode.Search, () => Console.WriteLine("Search"));

        NavbarButton(CodiconUnicode.Menu, () => ImGui.OpenPopup("Settings"));

        ImGui.PopFont();

        Settings.Gui();
    }

    private static void NavbarButton(CodiconUnicode icon, Action callback)
    {
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
        if (ImGui.Button($"{(char)icon}", new(ButtonSize)))
        {
            callback.Invoke();
        }
        ImGuiExt.CursorPointer();
    }

    private static void EditingNavbar()
    {
        ImGui.SetKeyboardFocusHere();
        string newPath = path;
        if (ImGui.InputText("", ref newPath, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (Directory.Exists(newPath))
            {
                path = newPath;
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

        float startX = ImGui.GetCursorPosX();

        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows))
        {
            editingNavbarLocation = true;
        }
        ImGui.SetCursorPosY(8f + (13f / 2));
        _ = ImGui.GetFontSize();

        string[] paths;

        if (path.Length == 0)
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
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0));
        for (int i = 0; i < paths.Length; i++)
        {
            string p = paths[i];

            if (ImGui.Button(p))
            {
                string fsPath = "";
                for (int j = 0; j <= i; j++)
                {
                    fsPath += paths[j] + '/';
                }
                Console.WriteLine(fsPath);
            }

            ImGuiExt.CursorPointer();

            ImGui.SameLine();

            ImGui.Text(" / ");

            ImGui.SameLine();
        }
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor();

        navbarWidth = ImGui.GetCursorPosX() - startX;

    }
}
