using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using log4net;

namespace SpotifyNowPlaying
{
    public static class WebClientHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebClientHelper));

        private static readonly HttpClient _client = new ();

        public static BitmapImage DownloadImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                var downloadTask = DownloadImageAsync(url);

                return downloadTask.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new SpotifyNowPlayingException($"An error occurred attempting to download image data from '{url}'. {e.Message}", e);
            }
        }

        public static async Task<BitmapImage> DownloadImageAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                var imageData = await _client.GetByteArrayAsync(url);

                using var ms = new MemoryStream(imageData);
                var bmp = new BitmapImage();

                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                return bmp;
            }
            catch (Exception e)
            {
                throw new SpotifyNowPlayingException($"An error occurred attempting to download image data from '{url}'. {e.Message}", e);
            }
        }

        public static void SaveImage(string url, string fileName)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            try
            {
                var downloadTask = SaveImageAsync(url, fileName);
                downloadTask.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new SpotifyNowPlayingException($"An error occurred attempting to download image data from '{url}'. {e.Message}", e);
            }
        }

        public static async Task SaveImageAsync(string url, string fileName)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            try
            {
                var imageData = await _client.GetByteArrayAsync(url);

                await File.WriteAllBytesAsync(fileName, imageData);
            }
            catch (Exception e)
            {
                throw new SpotifyNowPlayingException($"An error occurred attempting to download image data from '{url}'. {e.Message}", e);
            }
        }

    }
}
