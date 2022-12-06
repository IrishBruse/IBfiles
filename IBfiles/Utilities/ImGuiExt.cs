#pragma warning disable IDE0130
namespace ImGuiNET;

using System;
#pragma warning restore IDE0130

using System.Runtime.InteropServices;
using System.Text;

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
}
