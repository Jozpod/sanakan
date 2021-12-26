using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace Sanakan.Game.Tests
{
    public static class Utils
    {
        public static Stream CreateFakeImage()
        {
            var cardImage = new Image<Rgba32>(50, 50);
            var stream = new MemoryStream();
            cardImage.Save(stream, new PngEncoder());
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public static Stream CreateFakeImage(int width = 50, int height = 50)
        {
            var cardImage = new Image<Rgba32>(width, height);
            var stream = new MemoryStream();
            cardImage.Save(stream, new PngEncoder());
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        // Copyright (c) 2008-2013 Hafthor Stefansson
        // Distributed under the MIT/X11 software license
        // Ref: http://www.opensource.org/licenses/mit-license.php.
        public static unsafe bool CompareByteArrays(byte[] a1, byte[] a2)
        {
            if (a1 == a2)
            {
                return true;
            }

            if (a1 == null || a2 == null || a1.Length != a2.Length)
            {
                return false;
            }

            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                {
                    if (*((long*)x1) != *((long*)x2)) return false;
                }

                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) { return false; }

                    x1 += 4;
                    x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) { return false; }

                    x1 += 2; x2 += 2;
                }
                if ((l & 1) != 0)
                {
                    if (*((byte*)x1) != *((byte*)x2)) return false;
                }

                return true;
            }
        }
    }
}
