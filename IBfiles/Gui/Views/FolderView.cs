namespace IBfiles.Gui.Views;

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

using IBfiles.ApplicationBackend;
using IBfiles.Logic;
using IBfiles.Utilities;

using ImGuiNET;

public class FolderView
{
    private const int ColumnsCount = 3;
    private static bool selectAll = true;

    public void Gui()
    {
        ImGuiTableFlags flags = ImGuiTableFlags.Resizable;
        flags |= ImGuiTableFlags.Hideable;
        flags |= ImGuiTableFlags.Sortable;
        flags |= ImGuiTableFlags.Reorderable;

        flags |= ImGuiTableFlags.ScrollY;

        flags |= ImGuiTableFlags.PadOuterX;
        flags |= ImGuiTableFlags.NoKeepColumnsVisible;

        flags |= ImGuiTableFlags.NoBordersInBodyUntilResize;
        flags |= ImGuiTableFlags.BordersInnerV;

        if (Settings.I.AlternateRowColors)
        {
            flags |= ImGuiTableFlags.RowBg;
        }

        if (ImGui.BeginTable("Details", ColumnsCount, flags))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
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

        ContextMenu.FolderContextMenu();

        if (ImGui.IsKeyPressed(ImGuiKey.Escape) || (ImGui.IsAnyMouseDown() && !ImGui.IsAnyItemHovered()))
        {
            FileManager.Selections.Clear();
        }

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && !ImGui.IsAnyItemHovered())
        {
            ImGui.OpenPopup("FolderContextMenu");
        }
    }

    private static void Content()
    {
        selectAll = ImGui.GetIO().KeyCtrl && ImGui.IsKeyDown(ImGuiKey.A);

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, Vector2.Zero);
        {
            DisplayHeader();

            // Table Header Seperator
            ImGui.TableNextRow(ImGuiTableRowFlags.None, Settings.I.HeaderGap);

            foreach (DirectoryEntry entry in FileManager.DirectoryContents)
            {
                ImGui.TableNextRow(ImGuiTableRowFlags.None);
                DisplayRow(entry);
            }
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
            string columnName = ImGui.TableGetColumnName(column);
            Vector2 size = ImGui.CalcTextSize(columnName);
            float width = ImGui.GetColumnWidth();

            ImGui.PushID(column);
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ((rowHeight - size.Y) / 2f));
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ((width - size.X) / 2f));
                ImGui.TableHeader(columnName);
                ImGuiExt.CursorPointer();
            }
            ImGui.PopID();
        }
    }

    private static void DisplayRow(DirectoryEntry entry)
    {
        bool pop = false;
        ImGui.TableNextColumn();
        {
            if (FileManager.Selections.Contains(entry))
            {
                pop = true;
                ImGui.PushStyleColor(ImGuiCol.Text, Colors.AccentLight);
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.AccentDarker);
                ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.AccentDarker);
            }

            if (selectAll)
            {
                FileManager.Selections.Add(entry);
            }

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

            if (entry.Editing)
            {
                unsafe
                {
                    ImGui.SetKeyboardFocusHere();

                    const ImGuiInputTextFlags flags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCharFilter;
                    string name = entry.Path;
                    bool done = ImGui.InputText(string.Empty, ref name, 244, flags, ValidateFileNameInput);
                    entry.Path = name;

                    if (done)
                    {
                        entry.Path = Path.Join(FileManager.CurrentDirectory, entry.Path);
                        entry.Editing = false;
                        if (entry.IsFile)
                        {
                            File.Create(entry.Path, 0, FileOptions.None).Dispose();
                        }
                        else
                        {
                            Directory.CreateDirectory(entry.Path);
                        }
                    }
                }
            }
            else
            {
                if (ImGui.Selectable(entry.Name, FileManager.Selections.Contains(entry), ImGuiSelectableFlags.SpanAllColumns))
                {
                    if (!selectAll)
                    {
                        if (ImGui.GetIO().KeyCtrl)
                        {
                            if (FileManager.Selections.Contains(entry))
                            {
                                FileManager.Selections.Remove(entry);
                            }
                            else
                            {
                                FileManager.Selections.Add(entry);
                            }
                        }
                        else
                        {
                            FileManager.Selections.Clear();
                            FileManager.Selections.Add(entry);
                        }
                    }
                }

                if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
                {
                    DragDropHandler.Drag();

                    ImGui.EndDragDropSource();
                }
            }

            // Interact with entry
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemHovered())
            {
                EntryHandler.Open(entry);
            }

            if (entry.IsHidden)
            {
                ImGui.PopStyleColor();
            }

            if (pop)
            {
                ImGui.PopStyleColor(3);
            }

            if (FileManager.Selections.Contains(entry))
            {
                ContextMenu.EntryContextMenu(entry);
            }
        }
        ImGui.TableNextColumn();
        {
            DisplayModifiedTime(entry.LastWriteTime);
        }
        ImGui.TableNextColumn();
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
    }

    private static unsafe int ValidateFileNameInput(ImGuiInputTextCallbackData* t)
    {
        char c = (char)t->EventChar;
        return Path.GetInvalidFileNameChars().Contains(c) ? 1 : 0;
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
