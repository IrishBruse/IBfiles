namespace IBfiles.Gui.Views;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

public class HomeView
{
    private string selection;

    public void Gui()
    {
        float middleColumnWidth = ImGui.GetWindowWidth() - (32 + Settings.I.EdgeBorderWidth) - 96;

        ImGuiTableFlags flags = ImGuiTableFlags.PreciseWidths;

        flags |= ImGuiTableFlags.PadOuterX;
        flags |= ImGuiTableFlags.NoPadInnerX;

        flags |= ImGuiTableFlags.NoBordersInBodyUntilResize;
        flags |= ImGuiTableFlags.BordersInnerV;

        if (Settings.I.AlternateRowColors)
        {
            flags |= ImGuiTableFlags.RowBg;
        }

        if (ImGui.BeginTable("Home", 3, flags))
        {
            ImGui.TableSetupColumn("Icon", ImGuiTableColumnFlags.WidthFixed, 32 + (Settings.I.EdgeBorderWidth * 2));
            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, middleColumnWidth);
            ImGui.TableSetupColumn("Details", ImGuiTableColumnFlags.WidthFixed, 96);

            ImGui.TableNextRow(ImGuiTableRowFlags.None, Settings.I.HeaderGap);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Settings.I.HideOpticalDrives && drive.DriveType == DriveType.CDRom)
                {
                    continue;
                }

                ImGui.TableNextRow();

                bool pop = false;

                _ = ImGui.TableNextColumn();
                {
                    ImGui.Indent(Settings.I.EdgeBorderWidth);
                    if (selection == drive.Name)
                    {
                        pop = true;
                        ImGui.PushStyleColor(ImGuiCol.Text, Colors.AccentLight);
                        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
                        ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.AccentDarker);
                    }

                    IntPtr iconPtr;

                    if (drive.DriveType == DriveType.CDRom)
                    {
                        iconPtr = IconManager.GetIconExtensionPtrDirectly("disc");
                    }
                    else
                    {
                        iconPtr = IconManager.GetIconExtensionPtrDirectly("drive");
                    }
                    ImGui.Image(iconPtr, new Vector2(32), Vector2.Zero, Vector2.One, Colors.White);
                    ImGui.Unindent(Settings.I.EdgeBorderWidth);
                }

                _ = ImGui.TableNextColumn();

                string name;

                if (drive.DriveType == DriveType.CDRom)
                {
                    name = "Disk Drive ";
                }
                else
                {
                    name = drive.VolumeLabel + " ";
                }

                if (drive.Name.StartsWith("C"))
                {
                    name = "Local Disk ";
                }

                if (ImGui.Selectable(name + "(" + drive.Name[..^1] + ")", selection == drive.Name, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick, new Vector2(ImGui.GetWindowWidth(), 32)))
                {
                    selection = drive.Name;
                }

                ImGuiExt.CursorPointer();

                if (drive.IsReady && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemActive())
                {
                    FileManager.Open(drive.RootDirectory.FullName);
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 20);

                ImGui.PushStyleColor(ImGuiCol.Text, Colors.AccentLight);
                {
                    string overlay = "  " + Formatter.GetDataSize(drive.TotalSize - drive.TotalFreeSpace) + " / " + Formatter.GetDataSize(drive.TotalSize);
                    ImGui.ProgressBar((drive.TotalSize - drive.TotalFreeSpace) / (float)drive.TotalSize, new(middleColumnWidth, 14), overlay);
                }
                ImGui.PopStyleColor(1);

                _ = ImGui.TableNextColumn();
                ImGui.Indent();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 12);
                ImGui.Text(drive.DriveFormat);
                ImGui.Unindent();

                if (pop)
                {
                    ImGui.PopStyleColor(3);
                }

            }

            ImGui.EndTable();
        }
    }
}
