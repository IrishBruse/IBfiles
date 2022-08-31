namespace IBfiles.Gui;

using System.Globalization;
using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;

using ImGuiNET;

public static class FolderView
{
    private const int ColumnsCount = 3;

    public static void Gui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.BackgroundDark);
        float height = ImGui.GetWindowHeight() - 46;
        float width = io.DisplaySize.X;

        _ = ImGui.BeginChild("FolderView", new(width, height));
        {
            ImGuiTableFlags flags = ImGuiTableFlags.Resizable;
            flags |= ImGuiTableFlags.Hideable;
            flags |= ImGuiTableFlags.Sortable;
            flags |= ImGuiTableFlags.Reorderable;

            flags |= ImGuiTableFlags.PadOuterX;
            flags |= ImGuiTableFlags.NoPadInnerX;

            flags |= ImGuiTableFlags.NoBordersInBodyUntilResize;
            flags |= ImGuiTableFlags.BordersInnerV;

            if (Settings.I.AlternateRowColors)
            {
                flags |= ImGuiTableFlags.RowBg;
            }

            if (ImGui.BeginTable("Details", ColumnsCount, flags, new(width, height)))
            {
                ImGui.TableSetupColumn("Name");
                ImGui.TableSetupColumn("Modified");
                ImGui.TableSetupColumn("Size");

                ImGuiTableSortSpecsPtr specs = ImGui.TableGetSortSpecs();

                if ((specs.SpecsDirty || FileManager.SortDirty) && FileManager.DirectoryContents.Count > 1)
                {
                    FileManager.DirectoryContents.Sort((a, b) => FileManager.SortDirectory(a, b, specs));
                }

                Content();
                ImGui.EndTable();
            }
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    private static void Content()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, Vector2.Zero);
        DisplayHeader();

        // Table Header Seperator
        ImGui.TableNextRow(ImGuiTableRowFlags.None, Settings.I.HeaderGap);

        foreach (DirectoryEntry entry in FileManager.DirectoryContents)
        {
            ImGui.TableNextRow(ImGuiTableRowFlags.None);
            DisplayRow(entry);
        }
        ImGui.PopStyleVar();
    }

    private static void DisplayHeader()
    {
        const int rowHeight = 22;
        ImGui.TableNextRow(ImGuiTableRowFlags.Headers, rowHeight);

        for (int column = 0; column < ColumnsCount; column++)
        {
            ImGui.TableSetColumnIndex(column);
            string columnName = ImGui.TableGetColumnName(column); // Retrieve name passed to TableSetupColumn()
            Vector2 size = ImGui.CalcTextSize(columnName);
            float width = ImGui.GetColumnWidth();

            ImGui.PushID(column);
            {
                ImGui.SetCursorPosY((rowHeight - size.Y) / 2f);
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ((width - size.X) / 2f));
                ImGui.Text(columnName);
                ImGui.SameLine();
                ImGui.TableHeader("");
            }
            ImGui.PopID();
        }
    }

    private static void DisplayRow(DirectoryEntry entry)
    {

        bool pop = false;
        _ = ImGui.TableNextColumn();
        {
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(Settings.I.EdgeBorderWidth, 0));
            {
                if (FileManager.Selections.Contains(entry.Path))
                {
                    pop = true;
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.AccentLight);
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
                    ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.AccentDarker);
                }

                ImGui.Indent(Settings.I.EdgeBorderWidth);
                if (ImGui.Selectable(entry.Name, FileManager.Selections.Contains(entry.Path), ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                {
                    if (ImGui.GetIO().KeyCtrl)
                    {
                        if (FileManager.Selections.Contains(entry.Path))
                        {
                            FileManager.Selections.Remove(entry.Path);
                        }
                        else
                        {
                            FileManager.Selections.Add(entry.Path);
                        }
                    }
                    else
                    {
                        FileManager.Selections.Clear();
                        FileManager.Selections.Add(entry.Path);
                    }
                }
                ImGui.Unindent(Settings.I.EdgeBorderWidth);

                if (pop)
                {
                    ImGui.PopStyleColor(2);
                }
            }
            ImGui.PopStyleVar();
        }
        _ = ImGui.TableNextColumn();
        {
            DisplayModifiedTime(entry.LastWriteTime);
        }
        _ = ImGui.TableNextColumn();
        {
            if (entry.IsFile)
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
            else
            {
                string label = entry.Size == 1 ? "Item" : "Items";
                ImGui.Text($"{entry.Size} {label}");
            }
        }
        if (pop)
        {
            ImGui.PopStyleColor();
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
            int totalMinutes = (int)difference.TotalMinutes;
            ImGui.Text(totalMinutes + (totalMinutes == 1 ? " minute ago" : " minutes ago"));
        }
        else if (difference.TotalHours < 24)
        {
            int totalHours = (int)difference.TotalHours;
            ImGui.Text(totalHours + (totalHours == 1 ? " hour ago" : " hours ago"));
        }
        else if (difference.TotalDays < 7)
        {
            int totalDays = (int)difference.TotalDays;
            ImGui.Text(totalDays + (totalDays == 1 ? " day ago" : " days ago"));
        }
        else
        {
            ImGui.Text(lastWritten.ToString("dd/MM/yy 'at' h:mm tt", CultureInfo.InvariantCulture));
        }
    }
}
