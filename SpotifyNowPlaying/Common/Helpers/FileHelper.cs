using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace SpotifyNowPlaying.Common
{
    public static class FileHelper
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(FileHelper));

        public static void DeleteIfExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(path);
            if (!File.Exists(path)) return;

            File.Delete(path);
        }

    }
}
