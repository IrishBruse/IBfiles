namespace IBfiles.Gui.Views;

using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

public class HomeView
{
    private const float IconColumnSize = 32;
    private const float DetailsColumnSize = 128 + 16;
    private static readonly float NameColumnWidth = ImGui.GetWindowWidth() - DetailsColumnSize;

    public void Gui()
    {
        ImGuiTableFlags flags = ImGuiTableFlags.PreciseWidths;

        flags |= ImGuiTableFlags.PadOuterX;

        flags |= ImGuiTableFlags.NoBordersInBodyUntilResize;
        flags |= ImGuiTableFlags.BordersInnerV;

        if (Settings.I.AlternateRowColors)
        {
            flags |= ImGuiTableFlags.RowBg;
        }

        if (ImGui.BeginTable("Home", 3, flags))
        {
            ImGui.TableSetupColumn("Icon", ImGuiTableColumnFlags.WidthFixed, IconColumnSize);
            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, NameColumnWidth);
            ImGui.TableSetupColumn("Details", ImGuiTableColumnFlags.WidthFixed, DetailsColumnSize);

            ImGui.TableNextRow(ImGuiTableRowFlags.None, Settings.I.HeaderGap);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Settings.I.HideOpticalDrives && drive.DriveType == DriveType.CDRom)
                {
                    continue;
                }

                DisplayRow(drive);
            }
            ImGui.EndTable();

            if (ImGui.IsKeyPressed(ImGuiKey.Escape) || (!ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !ImGui.GetIO().KeyCtrl))
            {
                FileManager.Selections.Clear();
            }
        }
    }

    private static void DisplayRow(DriveInfo drive)
    {
        long size = drive.IsReady ? drive.TotalSize : -1;

        DirectoryEntry driveEntry = new(drive.Name, false, false, DateTime.MinValue, size);

        bool selected = FileManager.Selections.Contains(driveEntry);

        ImGui.TableNextRow();

        bool pop = false;

        _ = ImGui.TableNextColumn();

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

        if (drive.IsReady)
        {
            // Interact with entry
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemHovered())
            {
                FileManager.Open(drive.RootDirectory.FullName);
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 20);

            string overlay = " " + Formatter.GetDataSize(drive.TotalSize - drive.TotalFreeSpace) + " Used";
            ImGui.ProgressBar((drive.TotalSize - drive.TotalFreeSpace) / (float)drive.TotalSize, new(NameColumnWidth, 14), overlay);

            _ = ImGui.TableNextColumn();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 6);

            ImGui.Text(drive.DriveFormat);

            overlay = Formatter.GetDataSize(drive.TotalSize);
            ImGui.Text(overlay);
        }

        if (pop)
        {
            ImGui.PopStyleColor(3);
        }
    }
}
