namespace IBfiles.Logic;

using System;
using System.Runtime.InteropServices.ComTypes;

using Vanara.PInvoke;
using Vanara.Windows.Shell;

public class DragDropHandler
{
    public static Ole32.DROPEFFECT Effect = Ole32.DROPEFFECT.DROPEFFECT_MOVE;
    private static DropSource src = new();

    public static void Drag()
    {
        ShellItem item = new(@"A:\IBfiles\IBfiles\Icon.png");
        IDataObject data = item.DataObject;

        Ole32.DoDragDrop(data, src, Ole32.DROPEFFECT.DROPEFFECT_COPY, out Effect);
    }
}

internal class DropSource : Ole32.IDropSource
{
    public HRESULT QueryContinueDrag(bool fEscapePressed, uint grfKeyState)
    {
        Console.WriteLine(grfKeyState);
        // return HRESULT.DRAGDROP_S_USEDEFAULTCURSORS;

        if (fEscapePressed)
        {
            return HRESULT.DRAGDROP_S_CANCEL;
        }

        if (grfKeyState == 0)
        {
            return HRESULT.DRAGDROP_S_DROP;
        }

        if (grfKeyState == 1)
        {
            return HRESULT.S_OK;
        }

        if (grfKeyState == 4)
        {
            return HRESULT.DRAGDROP_S_DROP;
        }

        return HRESULT.E_UNEXPECTED;
    }

    public HRESULT GiveFeedback(Ole32.DROPEFFECT dwEffect)
    {
        DragDropHandler.Effect = dwEffect;
        return HRESULT.S_OK;
    }
}
