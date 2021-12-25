using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Logging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Shared.Classes.Image
{
    public static class ImageUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string IANA_MEDIA_TYPE_GIF = "image/gif";
        public const string IANA_MEDIA_TYPE_PNG = "image/png";
        public const string IANA_MEDIA_TYPE_JPG = "image/jpg";
        public const string IANA_MEDIA_TYPE_JPEG = "image/jpeg";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Loads the given image asynchronously and scales it arcordingly in case <paramref name="maxHeight"/> and <paramref name="maxWidth"/> are set to values greater than 0.
        /// </summary>
        /// <param name="file">The <see cref="StorageFile"/> where we should load the image from.</param>
        /// <param name="maxWidth">The maximum width in pixel for the image. -1 to keep the original width.</param>
        /// <param name="maxHeight">The maximum height in pixel for the image. -1 to keep the original height.</param>
        /// <returns>Returns the loaded and scaled image on success. Null else.</returns>
        public static async Task<SoftwareBitmap> LoadImageAsync(StorageFile file, double maxWidth = -1, double maxHeight = -1)
        {
            Debug.Assert(file is not null);
            try
            {
                Logger.Info($"Lading imge '{file.Path}'...");
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    Logger.Info($"Image loaded successfully.");

                    Logger.Info($"Scaling image...");
                    double scale = 1.0;
                    if (maxWidth > 0)
                    {
                        scale = maxWidth / decoder.PixelWidth;
                    }

                    if (maxHeight > 0)
                    {
                        double heightScale = maxHeight / decoder.PixelHeight;
                        if (heightScale < scale)
                        {
                            scale = heightScale;
                        }
                    }

                    uint newHeight = (uint)Math.Round(decoder.PixelHeight * scale);
                    uint newWidth = (uint)Math.Round(decoder.PixelWidth * scale);
                    BitmapTransform transform = new BitmapTransform { ScaledWidth = newWidth, ScaledHeight = newHeight };

                    SoftwareBitmap img = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
                    Logger.Info($"Image scaled to {img.PixelWidth}x{img.PixelHeight}.");
                    return img;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to load image '{file.Path}'.", e);
                return null;
            }
        }

        /// <summary>
        /// Converts the given data to a <see cref="SoftwareBitmap"/> and returns it.
        /// </summary>
        /// <param name="data">A valid <see cref="SoftwareBitmap"/> in binary representation.</param>
        /// <returns>The resulting <see cref="SoftwareBitmap"/> from the given <paramref name="data"/>.</returns>
        public static async Task<SoftwareBitmap> ToBitmapImageAsync(byte[] data)
        {
            Debug.Assert(data is not null);
            IRandomAccessStream stream = data.AsBuffer().AsStream().AsRandomAccessStream();
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            return await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        }

        /// <summary>
        /// Converts the given Base64 string to a <see cref="BitmapImage"/> and returns it.
        /// </summary>
        /// <param name="base64">A valid <see cref="BitmapImage"/> in Base64 representation.</param>
        /// <returns>The resulting <see cref="BitmapImage"/> from the given <paramref name="base64"/>.</returns>
        public static Task<SoftwareBitmap> ToBitmapImageAsync(string base64)
        {
            Debug.Assert(base64 is not null);
            return ToBitmapImageAsync(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// Converts the given <see cref="SoftwareBitmap"/> to a byte array and returns it.
        /// </summary>
        public static async Task<byte[]> ToByteArrayAsync(SoftwareBitmap img, bool isAnimated)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(isAnimated ? BitmapEncoder.GifEncoderId : BitmapEncoder.PngEncoderId, stream);
                encoder.SetSoftwareBitmap(img);
                await encoder.FlushAsync();
                Windows.Storage.Streams.Buffer buffer = new Windows.Storage.Streams.Buffer((uint)stream.Size);
                await stream.ReadAsync(buffer, (uint)stream.Size, InputStreamOptions.None);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Computes a SHA1 hash of the given image and returns the result as a hex string.
        /// </summary>
        public static string HashImage(byte[] img)
        {
            using (System.Security.Cryptography.SHA1CryptoServiceProvider sha = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                return SharedUtils.ToHexString(sha.ComputeHash(img));
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
