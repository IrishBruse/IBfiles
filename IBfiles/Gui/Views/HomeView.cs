namespace IBfiles.Gui.Views;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

public class HomeView
{
    private string selection;

    public void Gui()
    {
        ImGuiTableFlags flags = ImGuiTableFlags.None;

        flags |= ImGuiTableFlags.PadOuterX;
        flags |= ImGuiTableFlags.NoPadInnerX;

        flags |= ImGuiTableFlags.NoBordersInBodyUntilResize;
        flags |= ImGuiTableFlags.BordersInnerV;

        if (Settings.I.AlternateRowColors)
        {
            flags |= ImGuiTableFlags.RowBg;
        }

        if (ImGui.BeginTable("Home", 1, flags))
        {
            ImGui.TableSetupColumn("Name");

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Settings.I.HideOpticalDrives && drive.DriveType == DriveType.CDRom)
                {
                    continue;
                }

                bool pop = false;

                if (selection == drive.Name)
                {
                    pop = true;
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.AccentLight);
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
                    ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.AccentDarker);
                }

                _ = ImGui.TableNextColumn();

                IntPtr iconPtr;

                if (drive.DriveType == DriveType.CDRom)
                {
                    iconPtr = IconManager.GetIconPtrDirectly("disc");
                }
                else
                {
                    iconPtr = IconManager.GetIconPtrDirectly("drive");
                }
                ImGui.Image(iconPtr, new Vector2(32), Vector2.Zero, Vector2.One, Colors.White);

                ImGui.SameLine();

                if (ImGui.Selectable(drive.Name, selection == drive.Name, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick, new Vector2(ImGui.GetWindowWidth(), 32)))
                {
                    selection = drive.Name;
                }

                ImGuiExt.CursorPointer();

                ImGui.TableNextRow();

                if (pop)
                {
                    ImGui.PopStyleColor(3);
                }
            }

            ImGui.EndTable();
        }
    }
}
