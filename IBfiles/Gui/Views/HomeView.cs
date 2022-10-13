namespace IBfiles.Gui.Views;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

public class HomeView
{
    public void Gui()
    {
        const float iconColumnSize = 32;
        const float detailsColumnSize = 128;
        float nameColumnWidth = ImGui.GetWindowWidth() - (iconColumnSize + Settings.I.EdgeBorderWidth) - detailsColumnSize;

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
            ImGui.TableSetupColumn("Icon", ImGuiTableColumnFlags.WidthFixed, iconColumnSize + (Settings.I.EdgeBorderWidth * 2));
            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, nameColumnWidth);
            ImGui.TableSetupColumn("Details", ImGuiTableColumnFlags.WidthFixed, detailsColumnSize);

            ImGui.TableNextRow(ImGuiTableRowFlags.None, Settings.I.HeaderGap);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Settings.I.HideOpticalDrives && drive.DriveType == DriveType.CDRom)
                {
                    continue;
                }

                var driveEntry = new DirectoryEntry(drive.RootDirectory.FullName, false, false, DateTime.MinValue, drive.TotalSize);

                bool selected = FileManager.Selections.Contains(driveEntry);

                ImGui.TableNextRow();

                bool pop = false;

                _ = ImGui.TableNextColumn();

                ImGui.Indent(Settings.I.EdgeBorderWidth);
                if (selected)
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

                if (ImGui.Selectable(name + "(" + drive.Name[..^1] + ")", selected, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick, new Vector2(ImGui.GetWindowWidth(), 32)))
                {
                    FileManager.Selections.Clear();
                    FileManager.Selections.Add(driveEntry);
                }

                ImGuiExt.CursorPointer();

                if (drive.IsReady && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemActive())
                {
                    FileManager.Open(drive.RootDirectory.FullName);
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 20);

                if (drive.IsReady)
                {
                    string overlay = " " + Formatter.GetDataSize(drive.TotalSize - drive.TotalFreeSpace) + " Used";
                    ImGui.ProgressBar((drive.TotalSize - drive.TotalFreeSpace) / (float)drive.TotalSize, new(nameColumnWidth, 14), overlay);
                }

                _ = ImGui.TableNextColumn();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 6);

                ImGui.Indent();
                if (drive.IsReady)
                {
                    ImGui.Text(drive.DriveFormat);

                    string overlay = Formatter.GetDataSize(drive.TotalSize);
                    ImGui.Text(overlay);
                }
                ImGui.Unindent();


                if (pop)
                {
                    ImGui.PopStyleColor(3);
                }

            }
            ImGui.EndTable();

            if (ImGui.IsKeyPressed(ImGuiKey.Escape) || (!ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !ImGui.GetIO().KeyCtrl))
            {
                FileManager.Selections.Clear();
            }
        }
    }
}
