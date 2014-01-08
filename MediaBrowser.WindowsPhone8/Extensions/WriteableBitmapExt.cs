using System;
using System.Windows.Media.Imaging;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class WriteableBitmapExt
    {
        public static WriteableBitmap CentreCrop(this WriteableBitmap image, int height, int width)
        {
            var scaledSize = image.PixelWidth > image.PixelHeight
                                 ? (image.PixelHeight * ((double)691 / image.PixelWidth))
                                 : (image.PixelWidth * ((double)336 / image.PixelHeight));

            var resizedImage = image.PixelWidth > image.PixelHeight
                                               ? image.Resize(691, (int)Math.Floor(scaledSize), WriteableBitmapExtensions.Interpolation.Bilinear)
                                               : image.Resize((int)Math.Floor(scaledSize), 336, WriteableBitmapExtensions.Interpolation.Bilinear);

            if (width > height)
            {
                var top = resizedImage.PixelHeight > resizedImage.PixelWidth
                    // ReSharper disable PossibleLossOfFraction
                              ? (int)Math.Floor((double)((resizedImage.PixelHeight - 336) / 2))
                    // ReSharper restore PossibleLossOfFraction
                              : 0;
                var left = resizedImage.PixelWidth > resizedImage.PixelHeight
                               ? 0
                    // ReSharper disable PossibleLossOfFraction
                               : (int)Math.Floor((double)((resizedImage.PixelWidth - 691) / 2));
                // ReSharper restore PossibleLossOfFraction

                resizedImage = resizedImage.Crop(left, top, width, height);
            }
            else
            {
                var top = resizedImage.PixelHeight > resizedImage.PixelWidth
                    // ReSharper disable PossibleLossOfFraction
                              ? (int)Math.Floor((double)((resizedImage.PixelHeight - 336) / 2))
                    // ReSharper restore PossibleLossOfFraction
                              : 0;
                var left = resizedImage.PixelWidth > resizedImage.PixelHeight
                    // ReSharper disable PossibleLossOfFraction
                               ? (int)Math.Floor((double)((resizedImage.PixelWidth - 336) / 2))
                    // ReSharper restore PossibleLossOfFraction
                               : 0;
                resizedImage = resizedImage.Crop(left, top, width, height);
            }

            return resizedImage;
        }
    }
}
