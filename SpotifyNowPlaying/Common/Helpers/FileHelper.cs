using System;
using System.IO;
using log4net;

namespace SpotifyNowPlaying;

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
