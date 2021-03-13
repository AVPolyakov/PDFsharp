#if WITHOUT_DRAWING
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Metadata;

namespace PdfSharp.Drawing
{
    internal class ImageHandler: IDisposable
    {
        public static ImageHandler FromFile(string path)
        {
            var image = Image.Load(path, out var imgFormat);
            return new ImageHandler(image, imgFormat);
        }

        public static ImageHandler FromStream(Stream stream)
        {
            var image = Image.Load(stream, out var imgFormat);
            return new ImageHandler(image, imgFormat);
        }

        private readonly Image _image;
        private readonly IImageFormat _imageFormat;
        
        public ImageHandler(Image image, IImageFormat imageFormat)
        {
            _image = image;
            _imageFormat = imageFormat;
        }
        
        public XImageFormat ImageFormat
        {
            get
            {
                if (_imageFormat == PngFormat.Instance)
                    return XImageFormat.Png;
                if (_imageFormat == JpegFormat.Instance)
                    return XImageFormat.Jpeg;
                throw new ArgumentException(nameof(_imageFormat));
            }
        }

        public int Width => _image.Width;
        public int Height => _image.Height;

        public double HorizontalResolution => GetResolution(_image.Metadata.HorizontalResolution);

        public double VerticalResolution => GetResolution(_image.Metadata.VerticalResolution);
        
        public void SaveToBmp(MemoryStream memory) => _image.SaveAsBmp(memory, new BmpEncoder{BitsPerPixel = BmpBitsPerPixel.Pixel32});
        
        public void Dispose() => _image.Dispose();

        private double GetResolution(double resolution)
        {
            switch (_image.Metadata.ResolutionUnits)
            {
                case PixelResolutionUnit.PixelsPerInch:
                    return resolution;
                case PixelResolutionUnit.PixelsPerCentimeter:
                    return resolution * 2.54;
                case PixelResolutionUnit.PixelsPerMeter:
                    return resolution * 2.54 / 100;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_image.Metadata.ResolutionUnits));
            }
        }
    }
}
#endif