using System;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using log4net;
using Image = System.Drawing.Image;

namespace SpotifyNowPlaying.Common
{
    public static class WebClientHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebClientHelper));

        public static BitmapImage DownloadImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                using var wc = new WebClient();

                var imageData = wc.DownloadData(url);

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
                using var wc = new WebClient();

                var imageData = wc.DownloadData(url);

                using var ms = new MemoryStream(imageData);
                using var bmp = Image.FromStream(ms);

                bmp.Save(fileName);
            }
            catch (Exception e)
            {
                throw new SpotifyNowPlayingException($"An error occurred attempting to download image data from '{url}'. {e.Message}", e);
            }
        }

    }
}
