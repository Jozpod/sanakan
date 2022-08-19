using Sanakan.Game;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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

        public static Stream ToJpgStream(this Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, _jpgEncoder);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static Stream ToPngStream(this Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, _pngEncoder);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static string SaveToPath(this Image image, string path, Common.IFileSystem fileSystem)
        {
            var extension = path.Split(".").Last().ToLower();
            var encoder = (extension == "png") ? _pngEncoder : _jpgEncoder;
            using var fileStream = fileSystem.OpenWrite(path);
            image.Save(fileStream, encoder);

            return path;
        }

        public static string SaveToPath(this Image image, string path, int width, Common.IFileSystem fileSystem, int height = 0)
        {
            var extension = path.Split(".").Last().ToLower();
            var encoder = (extension == "png") ? _pngEncoder : _jpgEncoder;
            image.Mutate(x => x.Resize(new Size(width, height)));
            using var fileStream = fileSystem.OpenWrite(path);
            image.Save(fileStream, encoder);

            return path;
        }

        public static Image<T> ResizeAsNew<T>(this Image<T> img, int width, int height = 0)
            where T : unmanaged, IPixel<T>
        {
            var nImg = img.Clone();
            nImg.Mutate(x => x.Resize(new Size(width, height)));
            return nImg;
        }

        public static void Round(this IImageProcessingContext imageProcessingContext, float radius)
        {
            var size = imageProcessingContext.GetCurrentSize();

            var drawingOptions = new DrawingOptions
            {
                GraphicsOptions = new GraphicsOptions
                {
                    AlphaCompositionMode = PixelAlphaCompositionMode.DestOut
                }
            };
            var corners = BuildCorners(size.Width, size.Height, radius);

            imageProcessingContext.Fill(drawingOptions, Colors.Black, corners);
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            var cornerToptLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            var rightPos = imageWidth - cornerToptLeft.Bounds.Width + 1;
            var bottomPos = imageHeight - cornerToptLeft.Bounds.Height + 1;

            var cornerTopRight = cornerToptLeft.RotateDegree(90).Translate(rightPos, 0);
            var cornerBottomLeft = cornerToptLeft.RotateDegree(-90).Translate(0, bottomPos);
            var cornerBottomRight = cornerToptLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}
