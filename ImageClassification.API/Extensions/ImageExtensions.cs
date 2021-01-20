using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageClassification.API.Extensions
{
    public static class ImageExtensions
    {
        private readonly static List<ImageOptions.ImageFormat> _canBeUsed;
        static ImageExtensions()
        {
            _canBeUsed = new List<ImageOptions.ImageFormat>
            {
                ImageOptions.ImageFormat.jpeg, ImageOptions.ImageFormat.png
            };
        }
        public static IReadOnlyCollection<ImageOptions.ImageFormat> CanBeUsed => _canBeUsed.AsReadOnly();
        public static void AddFormat(ImageOptions.ImageFormat format)
        {
            if (!_canBeUsed.Contains(format))
            {
                _canBeUsed.Add(format);
            }
        }
        public static void RemoveFormat(ImageOptions.ImageFormat format)
        {
            _canBeUsed.Remove(format);
        }

        public static bool IsValidImage(this byte[] image)
        {
            var imageFormat = GetImageFormat(image);
            return imageFormat == ImageOptions.ImageFormat.jpeg ||
                   imageFormat == ImageOptions.ImageFormat.png;
        }
        public static class ImageOptions
        {
            public enum ImageFormat
            {
                bmp,
                jpeg,
                gif,
                tiff,
                png,
                unknown
            }

        }
        public static ImageOptions.ImageFormat GetImageFormat(byte[] bytes)
        {
            // see http://www.mikekunz.com/image_file_header.html  
            var bmp = Encoding.ASCII.GetBytes("BM");       // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");      // GIF
            var png = new byte[] { 137, 80, 78, 71 };      // PNG
            var tiff = new byte[] { 73, 73, 42 };          // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 };  // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageOptions.ImageFormat.bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ImageOptions.ImageFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageOptions.ImageFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImageOptions.ImageFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImageOptions.ImageFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageOptions.ImageFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageOptions.ImageFormat.jpeg;

            return ImageOptions.ImageFormat.unknown;
        }

        public static Stream ToStream(this Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, image.RawFormat);
            stream.Position = 0;
            return stream;
        }

        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public static string ToContentType(this ImageFormat rawFormat)
        {
            var format = new ImageFormatConverter().ConvertToString(rawFormat).ToLower();
            new FileExtensionContentTypeProvider().TryGetContentType($".{format}", out string contentType);
            return contentType;
        }
    }
}
