#pragma warning disable IDE0130
namespace ImGuiNET;

using System;
using System.Numerics;
#pragma warning restore IDE0130

using System.Runtime.InteropServices;
using System.Text;

using IBfiles.ApplicationBackend;

public static class ImGuiExt
{
    private const int StackAllocationSizeLimit = 2048;

    public static void CursorPointer()
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
    }

    public static bool Button(string label, Vector2 size = default)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, Colors.White);
        bool clicked = ImGui.Button(label, size);
        ImGui.PopStyleColor();
        CursorPointer();
        return clicked;
    }

    public static bool Selectable(string label)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, Colors.White);
        bool selected = ImGui.Selectable(label);
        ImGui.PopStyleColor();
        CursorPointer();
        return selected;
    }

    public static unsafe bool BeginPopupModal(string name, ImGuiWindowFlags flags)
    {
        byte* native_name;
        int name_byteCount = 0;

        if (name != null)
        {
            name_byteCount = Encoding.UTF8.GetByteCount(name);
            if (name_byteCount > StackAllocationSizeLimit)
            {
                native_name = Allocate(name_byteCount + 1);
            }
            else
            {
                byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                native_name = native_name_stackBytes;
            }
            int native_name_offset = GetUtf8(name, native_name, name_byteCount);
            native_name[native_name_offset] = 0;
        }
        else
        {
            native_name = null;
        }

        byte* native_p_open = null;
        byte ret = ImGuiNative.igBeginPopupModal(native_name, native_p_open, flags);
        if (name_byteCount > StackAllocationSizeLimit)
        {
            Free(native_name);
        }

        return ret != 0;
    }

    private static unsafe byte* Allocate(int byteCount)
    {
        return (byte*)(void*)Marshal.AllocHGlobal(byteCount);
    }

    private static unsafe void Free(byte* ptr)
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }

    private static unsafe int GetUtf8(string s, byte* utf8Bytes, int utf8ByteCount)
    {
        fixed (char* ptr = s)
        {
            char* chars = ptr;
            return Encoding.UTF8.GetBytes(chars, s.Length, utf8Bytes, utf8ByteCount);
        }
    }

    public static float ReactiveSize(float min, float max)
    {
        float width = ImGui.GetContentRegionAvail().X;

        if (width < min)
        {
            ImGui.SetCursorPosX(10f);

            return width - 20;// Full width
        }
        else
        {
            float sizeDifference = max - min;
            float percentage = (width - min) / sizeDifference;

            percentage = Math.Clamp(percentage, 0, 1f);

            float result = Lerp(min - 20, max, percentage / 2f);

            ImGui.SetCursorPosX((width - result) * 0.5f);

            return result;
        }
    }

    private static float Lerp(float a, float b, float t)
    {
        return (a * (1 - t)) + (b * t);
    }
}
