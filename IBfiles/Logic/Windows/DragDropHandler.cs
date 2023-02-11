#pragma warning disable IDE0130
namespace IBfiles.Logic;
#pragma warning restore IDE0130

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using Vanara.PInvoke;
using Vanara.Windows.Shell;

using static Vanara.PInvoke.Shell32;

public class DragDropHandler
{
    public static Ole32.DROPEFFECT Effect = Ole32.DROPEFFECT.DROPEFFECT_MOVE;
    private static DropSource src = new();

    public static void Drag()
    {
        ShellItem item = new(@"A:\IBfiles\IBfiles\Icon.png");
        IDataObject data = item.DataObject;

        AddPreviewImage(data, @"A:\IBfiles\IBfiles\Icon.png");

        // data.SetData()

        Ole32.DoDragDrop(data, src, Ole32.DROPEFFECT.DROPEFFECT_COPY, out Effect);
    }

    public static void AddPreviewImage(IDataObject dataObject, string imgPath)
    {
        if (dataObject == null)
        {
            throw new ArgumentNullException(nameof(dataObject));
        }

        IDragSourceHelper ddh = (IDragSourceHelper)new DragDropHelper();
        SHDRAGIMAGE dragImage = new();

        // note you should use a thumbnail here, not a full-sized image
        System.Drawing.Bitmap thumbnail = new(imgPath);
        dragImage.sizeDragImage.cx = thumbnail.Width;
        dragImage.sizeDragImage.cy = thumbnail.Height;
        dragImage.crColorKey = new COLORREF(0xff, 0, 0xff);
        dragImage.hbmpDragImage = thumbnail.GetHbitmap();
        ddh.InitializeFromBitmap(dragImage, dataObject);
    }
}

[ComImport, Guid("4657278a-411b-11d2-839a-00c04fd918d0")] // CLSID_DragDropHelper
internal class DragDropHelper
{
}

internal class DropSource : Ole32.IDropSource
{
    public HRESULT QueryContinueDrag(bool fEscapePressed, uint grfKeyState)
    {
        Console.WriteLine(grfKeyState);

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
