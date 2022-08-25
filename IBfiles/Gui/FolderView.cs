namespace IBfiles.Gui;

using System.Globalization;
using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

public static class FolderView
{
    public static void Gui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundDark);
        float height = ImGui.GetWindowHeight() - 46;
        float width = io.DisplaySize.X;
        _ = ImGui.BeginChild("FolderView", new(width, height));
        {
            const ImGuiTableFlags flags = ImGuiTableFlags.Resizable | ImGuiTableFlags.PadOuterX | ImGuiTableFlags.NoBordersInBodyUntilResize | ImGuiTableFlags.Sortable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.BordersInnerV;
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(Settings.I.BorderWidth, 0));
            {
                if (ImGui.BeginTable("Details", 3, flags, new(width, height)))
                {
                    ImGuiTableSortSpecsPtr specs = ImGui.TableGetSortSpecs();
                    if (specs.SpecsDirty && FileManager.DirectoryContents.Count > 1)
                    {
                        FileManager.DirectoryContents.Sort((a, b) => FileManager.SortDirectory(a, b, specs));
                    }
                    Content();
                    ImGui.EndTable();
                }
            }
            ImGui.PopStyleVar();
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    private static void Content()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(6, 0));
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Modified");
            ImGui.TableSetupColumn("Size");
        }
        ImGui.PopStyleVar();

        ImGui.TableHeadersRow();

        foreach (DirectoryEntry entry in FileManager.DirectoryContents)
        {
            if (entry.IsFile)
            {
                DisplayFileRow(entry);
            }
            else
            {
                DisplayFolderRow(entry);
            }
            ImGui.TableNextRow();
        }
    }

    private static void DisplayFileRow(DirectoryEntry entry)
    {
        _ = ImGui.TableNextColumn();
        {
            ImGui.Text(entry.Name);
        }
        _ = ImGui.TableNextColumn();
        {

            DisplayModifiedTime(entry.LastWriteTime);
        }
        _ = ImGui.TableNextColumn();
        {
            long bytes = entry.Size;
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double decimalBytes = bytes;
            for (i = 0; i < suffixes.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                decimalBytes = bytes / 1024.0;
            }
            ImGui.Text($"{decimalBytes:0.##} {suffixes[i]}");
        }
    }

    private static void DisplayModifiedTime(DateTime lastWritten)
    {
        TimeSpan difference = DateTime.Now - lastWritten;

        if (difference.TotalSeconds < 60)
        {
            ImGui.Text("Just Now");
        }
        else if (difference.TotalMinutes < 60)
        {
            ImGui.Text(((int)difference.TotalMinutes) + " minutes ago");
        }
        else if (difference.TotalHours < 24)
        {
            ImGui.Text(((int)difference.TotalHours) + " hours ago");
        }
        else if (difference.TotalDays < 7)
        {
            ImGui.Text(((int)difference.TotalDays) + " days ago");
        }
        else
        {
            ImGui.Text(lastWritten.ToString("MM/dd/yy 'at' h:mm tt", CultureInfo.InvariantCulture));
        }
    }

    private static void DisplayFolderRow(DirectoryEntry entry)
    {
        _ = ImGui.TableNextColumn();
        {
            ImGui.Text(entry.Name);
        }
        _ = ImGui.TableNextColumn();
        {
            DisplayModifiedTime(entry.LastWriteTime);
        }
        _ = ImGui.TableNextColumn();
        {
            string label = entry.Size == 1 ? "Item" : "Items";
            ImGui.Text($"{entry.Size} {label}");
        }
    }
}
