namespace IBfiles.Gui.Views;

using System.Globalization;
using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

public class FolderView
{
    private const int ColumnsCount = 3;

    public void Gui()
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

        if (ImGui.BeginTable("Details", ColumnsCount, flags))
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
            _ = ImGui.TableSetColumnIndex(column);
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
                ImGuiExt.CursorPointer();
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

                IntPtr iconPtr;
                if (entry.IsFile)
                {
                    iconPtr = IconManager.GetFileIcon(entry.Path);
                }
                else
                {
                    iconPtr = IconManager.GetFolderIcon(entry.Path);
                }

                Vector4 tint_col = new(1, 1, 1, entry.IsHidden ? 0.6f : 1f);

                ImGui.Image(iconPtr, new Vector2(16), Vector2.Zero, Vector2.One, tint_col);
                ImGui.SameLine();

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 4);

                if (entry.IsHidden)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.TextDisabled);
                }

                if (ImGui.Selectable(entry.Name, FileManager.Selections.Contains(entry.Path), ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                {
                    if (ImGui.GetIO().KeyCtrl)
                    {
                        if (FileManager.Selections.Contains(entry.Path))
                        {
                            _ = FileManager.Selections.Remove(entry.Path);
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

                    if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        EntryHandler.Open(entry);
                    }
                }

                if (entry.IsHidden)
                {
                    ImGui.PopStyleColor();
                }

                ImGuiExt.CursorPointer();

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
                ImGui.Text(Formatter.GetDataSize(entry.Size));
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
