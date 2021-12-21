using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using SpotifyNowPlaying.Common;
using SpotifyNowPlaying.Config;

namespace SpotifyNowPlaying.Output
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static class OutputManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OutputManager));
        
        private static SpotifyPlaybackState _previousState;

        public static void Cleanup()
        {
            try
            {
                var settings = SettingsHelper.Instance.CurrentSettings;
                var fileTypes = Enum.GetValues<FileType>();

                foreach (var fileType in fileTypes)
                {
                    DeleteFile(settings, fileType);
                }
            
                log.Info("Finished cleaning up output files.");
            }
            catch (Exception e)
            {
                var message = $"An error occurred during cleanup. {e.Message}";
                log.Error(message, e);
            }
        }

        public static void Process(SpotifyPlaybackState state)
        {
            try
            {
                if (_previousState == null || _previousState.Equals(state))
                {
                    return;
                }
            
                var settings = SettingsHelper.Instance.CurrentSettings;
                var context = new OutputContext(state);

                log.Info($"Outputting playback information for {state}...");
                
                var ignored = new[] { FileType.None, FileType.Text, FileType.Image };
                var fileTypes = Enum.GetValues<FileType>().Except(ignored);
                foreach (var fileType in fileTypes)
                {
                    Save(settings, context, fileType);
                }
            
                log.Info($"Finished outputting playback information for {state}.");

                _previousState = state;
            }
            catch (Exception e)
            {
                var message = $"An error occurred processing the output for state {state}. {e.Message}";
                log.Error(message, e);
            }
        }
        
        private static void DeleteFile(string path)
        {
            try
            {
                if (!File.Exists(path)) return;

                File.Delete(path);
            
                var fileName = Path.GetFileName(path);

                log.Debug($"Deleted file '{fileName}'.");
            }
            catch (Exception e)
            {
                var message = $"An error occurred deleting the file '{path}'. {e.Message}";
                log.Error(message, e);
            }
        }
        
        private static void DeleteFile(UserSettings settings, FileType fileType)
        {
            var fileName = settings.GetFullPath(fileType);
            DeleteFile(fileName);
        }

        private static void Save(UserSettings settings, OutputContext context, FileType fileType)
        {
            if (fileType == default) return;
            if (!settings.IsOutputEnabled(fileType)) return;

            var fullPath = settings.GetFullPath(fileType);
            var fileName = Path.GetFileName(fullPath);
            var content = GetFileContent(context, fileType);

            if (FileType.Text.HasFlag(fileType))
            {
                WebClientHelper.SaveImage(content, fullPath);
            }
            else if (FileType.Image.HasFlag(fileType))
            {
                File.WriteAllText(fullPath, content);
            }

            log.Debug($"Saved text '{content}' to '{fileName}'.");
        }
        
        private static IEnumerable<FileType> GetFileTypes()
        {
            var ignored = new[] { FileType.None, FileType.Text, FileType.Image };
            return Enum.GetValues<FileType>().Except(ignored);
        }

        private static string GetFileContent(OutputContext context, FileType? fileType = null)
        {
            return fileType switch
            {
                FileType.Song            => context.Song.Name,
                FileType.Artist          => context.Artist.Name,
                FileType.Album           => context.Album.Name,
                FileType.PlaylistName    => context.Playlist.Name,
                FileType.AlbumArtwork    => context.Album.ImageUrl,
                FileType.PlaylistArtwork => context.Playlist.ImageUrl,
                _                        => throw new ArgumentOutOfRangeException(nameof(fileType))
            };
        }
    }
}
