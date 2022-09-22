namespace IBfiles.Gui;

using System.Diagnostics;
using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

public class NavbarGui
{
    private const int ButtonSize = 40;
    private const int ButtonPadding = 20;
    private static bool editingNavbarLocation;
    private static float navbarWidth;

    private static void Content()
    {
        ImGui.PushFont(Application.IconsFontBig);
        {
            NavbarButton(CodiconUnicode.ChevronLeft, FileManager.HistoryBack, FileManager.History.Count > 0 && FileManager.History[^1] != null);
            NavbarButton(CodiconUnicode.ChevronRight, FileManager.HistoryForward, FileManager.History.Count > 0 && FileManager.History[^1] != null);
            NavbarButton(CodiconUnicode.ChevronUp, FileManager.UpDirectoryLevel, Path.IsPathFullyQualified(FileManager.CurrentDirectory));

            ImGui.SameLine();
            float width = ImGui.GetWindowWidth() - (ImGui.GetCursorPosX() * 2f) - 6;

            ImGui.PushItemWidth(width);

            ImGui.PushFont(Application.CascadiaFont);
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

            NavbarButton(CodiconUnicode.Refresh, FileManager.Refresh, true);
            NavbarButton(CodiconUnicode.Search, () => Console.WriteLine("Search"), true);
            NavbarButton(CodiconUnicode.Menu, () => FileManager.Open(Page.Settings), true);
        }
        ImGui.PopFont();
    }

    private static void NavbarButton(CodiconUnicode icon, Action callback, bool enabled)
    {
        ImGui.SameLine();
        ImGui.SetCursorPosY(3);
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 3);

        if (enabled == false)
        {
            ImGui.BeginDisabled();
        }

        if (ImGui.Button($"{(char)icon}", new(ButtonSize)) && enabled)
        {
            callback.Invoke();
        }

        if (enabled == false)
        {
            ImGui.EndDisabled();
        }

        ImGuiExt.CursorPointer();
    }

    private static void EditingNavbar()
    {
        ImGui.SetKeyboardFocusHere();
        string newPath = FileManager.CurrentDirectory;
        if (ImGui.InputText("", ref newPath, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (Directory.Exists(newPath))
            {
                FileManager.Open(newPath);
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
        string path = FileManager.CurrentDirectory;

        float startX = ImGui.GetCursorPosX();

        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows))
        {
            editingNavbarLocation = true;
        }
        ImGui.SetCursorPosY(5f + (13f / 2));
        _ = ImGui.GetFontSize();

        Debug.Assert(path != null);

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
            ImGui.PushFont(Application.IconsFont);
            {
                if (ImGui.Button($"{(char)CodiconUnicode.Home}"))
                {
                    FileManager.Open(Page.Home);
                }
                ImGuiExt.CursorPointer();
            }
            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.Text(Settings.I.UseBackslashSeperator ? " \\ " : " / ");
            ImGui.SameLine();

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
                        int ups = paths.Length - 1 - i;
                        for (int j = 0; j < ups; j++)
                        {
                            FileManager.UpDirectoryLevel();
                        }
                    }
                    ImGuiExt.CursorPointer();
                }
                ImGui.PopID();


                ImGui.SameLine();

                ImGui.Text(Settings.I.UseBackslashSeperator ? " \\ " : " / ");

                ImGui.SameLine();
            }
        }
        ImGui.PopStyleVar();
        ImGui.PopStyleColor();

        navbarWidth = ImGui.GetCursorPosX() - startX;
    }

    public void Gui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(3f));
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundNormal);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0));
        {
            _ = ImGui.BeginChild("Navbar", new(io.DisplaySize.X, 46));
            {
                Content();
            }
            ImGui.EndChild();
        }
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(2);
    }
}
