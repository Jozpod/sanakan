﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Sanakan.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ImageExtension
    {
        private static IImageEncoder _jpgEncoder = new JpegEncoder() { Quality = 85 };
        private static IImageEncoder _pngEncoder = new PngEncoder();

        public static Stream ToJpgStream<T>(this Image<T> img)
            where T : struct, IPixel<T>
        {
            var stream = new MemoryStream();
            img.Save(stream, _jpgEncoder);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static Stream ToPngStream<T>(this Image<T> image)
            where T : struct, IPixel<T>
        {
            var stream = new MemoryStream();
            image.Save(stream, _pngEncoder);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static string SaveToPath<T>(this Image<T> image, string path, Common.IFileSystem fileSystem)
            where T : struct, IPixel<T>
        {
            var extension = path.Split(".").Last().ToLower();
            var encoder = (extension == "png") ? _pngEncoder : _jpgEncoder;
            using var fileStream = fileSystem.OpenWrite(path);
            image.Save(fileStream, encoder);

            return path;
        }

        public static string SaveToPath<T>(this Image<T> image, string path, int width, Common.IFileSystem fileSystem, int height = 0)
            where T : struct, IPixel<T>
        {
            var extension = path.Split(".").Last().ToLower();
            var encoder = (extension == "png") ? _pngEncoder : _jpgEncoder;
            image.Mutate(x => x.Resize(new Size(width, height)));
            using var fileStream = fileSystem.OpenWrite(path);
            image.Save(fileStream, encoder);

            return path;
        }

        public static Image<T> ResizeAsNew<T>(this Image<T> img, int width, int height = 0)
            where T : struct, IPixel<T>
        {
            var nImg = img.Clone();
            nImg.Mutate(x => x.Resize(new Size(width, height)));
            return nImg;
        }

        public static void Round(this IImageProcessingContext<Rgba32> img, float radius)
        {
            var size = img.GetCurrentSize();
            var gOptions = new GraphicsOptions(true) { AlphaCompositionMode = PixelAlphaCompositionMode.DestOut };
            img.Fill(gOptions, Rgba32.Black, BuildCorners(size.Width, size.Height, radius));
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            IPath cornerToptLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            float rightPos = imageWidth - cornerToptLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerToptLeft.Bounds.Height + 1;

            IPath cornerTopRight = cornerToptLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerToptLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerToptLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}
