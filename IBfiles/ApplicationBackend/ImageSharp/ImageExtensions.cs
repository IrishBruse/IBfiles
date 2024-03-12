namespace IBfiles.ApplicationBackend.ImageSharp;

using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class ImageExtensions
{
    public static Span<Rgba32> GetPixelSpan(this Image<Rgba32> image)
    {
        if (image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> pixelMemory))
        {
            return pixelMemory.Span;
        }

        int bufferSize = image.Width * image.Height;
        Rgba32[] buffer = new Rgba32[bufferSize];
        Span<Rgba32> span = new(buffer);
        image.CopyPixelDataTo(span);
        return span;
    }
}
