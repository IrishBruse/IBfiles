namespace IBfiles;

using System.Numerics;

using ImGuiNET;

public class FileManager
{
    private const int ButtonSize = 40;
    private const int ButtonPadding = 20;
    private static bool editingNavbarLocation;
    private static string path = "C:/Windows/Test";

    public static void Submit()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        Vector2 windowSize = io.DisplaySize;
        ImGui.SetNextWindowSize(windowSize);
        ImGui.SetNextWindowPos(Vector2.Zero);

        _ = ImGui.Begin("Viewport", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs);
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundNormal);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 0);
            _ = ImGui.BeginChild("Navbar", new(io.DisplaySize.X, 45));
            {
                Navbar();
            }
            ImGui.EndChild();
            ImGui.PopStyleColor();
        }
        ImGui.End();
    }

    private static void Navbar()
    {
        ImGui.SetCursorPosY(2.5f);

        ImGui.PushFont(Application.Icons26Font);

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
        _ = ImGui.Button($"{(char)CodiconUnicode.ChevronLeft}", new(ButtonSize));
        HoverCursor();

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
        _ = ImGui.Button($"{(char)CodiconUnicode.ChevronRight}", new(ButtonSize));
        HoverCursor();

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
        _ = ImGui.Button($"{(char)CodiconUnicode.ChevronUp}", new(ButtonSize));
        HoverCursor();

        ImGui.SameLine();
        int width = (int)ImGui.GetWindowWidth();
        width -= (int)ImGui.GetCursorPosX();
        width -= ButtonSize * 3; // button
        width -= (int)(ButtonPadding * 2.5f); // button padding

        ImGui.PushItemWidth(width);

        ImGui.PushFont(Application.Cascadia13Font);

        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.ChildBg, 0xFF888888);
        if (editingNavbarLocation)
        {
            ImGui.SetKeyboardFocusHere();
            if (ImGui.InputText("", ref path, 256, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                Console.WriteLine("type");
                editingNavbarLocation = false;
            }
        }
        else
        {
            _ = ImGui.BeginChild("test", new(width, ButtonSize));
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows))
            {
                Console.WriteLine("edit");
                editingNavbarLocation = true;
            }
            ImGui.SetCursorPosY(5f + (13f / 2));
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
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, 0);
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

                HoverCursor();

                ImGui.SameLine();

                ImGui.Text("/");
                ImGui.SameLine();
            }
            // ImGui.PopStyleVar(2);
            ImGui.EndChild();
        }

        ImGui.PopStyleColor();

        ImGui.SetCursorPosY(2.5f);

        ImGui.PopFont();

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
        _ = ImGui.InvisibleButton("", new(ButtonSize));

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
        _ = ImGui.InvisibleButton("", new(ButtonSize));

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
        _ = ImGui.Button($"{(char)CodiconUnicode.Menu}", new(ButtonSize));
        HoverCursor();

        ImGui.PopFont();
    }

    private static void HoverCursor()
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
    }
}
