using System;
using System.IO;
using JetBrains.Annotations;
using log4net;
using SpotifyNowPlaying.Common;

namespace SpotifyNowPlaying.Output
{
    public static class OutputManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OutputManager));

        private const string OUTPUT_DIRECTORY = @"C:\test";

        private static SpotifyPlaybackState _previousState;

        public static void Cleanup()
        {
            DeleteFile(@"song.txt");
            DeleteFile(@"artist.txt");
            DeleteFile(@"album.txt");
            DeleteFile(@"playlist.txt");
            DeleteFile(@"album.jpg");
            DeleteFile(@"playlist.jpg");

            log.Info("Finished cleaning up output files.");
        }

        public static void Process(SpotifyPlaybackState state)
        {
            if (_previousState != null && _previousState.Equals(state))
            {
                return;
            }

            var context = new OutputContext(state);

            log.Info($"Outputting playback information for {state}...");

            SaveText(@"song.txt", context.Song.Name);
            SaveText(@"artist.txt", context.Artist.Name);
            SaveText(@"album.txt", context.Album.Name);
            SaveText(@"playlist.txt", context.Playlist.Name);
            
            SaveImage(@"album.jpg", context.Album.ImageUrl);
            SaveImage(@"playlist.jpg", context.Playlist.ImageUrl);

            log.Info($"Finished outputting playback information for {state}.");

            _previousState = state;
        }

        private static string GetFullPath(string path)
        {
            return Path.Combine(OUTPUT_DIRECTORY, path);
        }

        private static void DeleteFile(string path)
        {
            var fullPath = GetFullPath(path);

            if (!File.Exists(fullPath)) return;

            File.Delete(fullPath);

            log.Debug($"Deleted file '{path}'.");
        }

        private static void SaveText([NotNull] string path, string text)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            var fullPath = GetFullPath(path);

            File.WriteAllText(fullPath, text);

            log.Debug($"Saved text '{text}' to '{path}'.");
        }

        private static void SaveImage(string path, string url)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            
            var fullPath = GetFullPath(path);

            if (string.IsNullOrWhiteSpace(url))
            {
                DeleteFile(path);

                return;
            }
            
            WebClientHelper.SaveImage(url, fullPath);
        }
    }
}
