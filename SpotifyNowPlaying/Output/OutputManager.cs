using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using log4net;
using SpotifyNowPlaying.Common;
using SpotifyNowPlaying.Config;

namespace SpotifyNowPlaying.Output
{
    public static class OutputManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OutputManager));
        
        private static SpotifyPlaybackState _previousState;

        public static void Cleanup()
        {
            try
            {
                var settings = SettingsHelper.Instance.CurrentSettings;
                var ignored = new[] { FileType.None, FileType.Text, FileType.Image };
                var fileTypes = Enum.GetValues<FileType>().Except(ignored);

                foreach (var fileType in fileTypes)
                {
                    DeleteFile(settings, fileType);
                }
            
                log.Info(@"Finished cleaning up output files.");
            }
            catch (Exception e)
            {
                var message = $@"An error occurred during cleanup. {e.Message}";
                log.Error(message, e);
            }
        }

        public static void Process(SpotifyPlaybackState state)
        {
            try
            {
                if (state.Equals(_previousState))
                {
                    return;
                }
            
                var settings = SettingsHelper.Instance.CurrentSettings;
                var context = new OutputContext(state);

                log.Info($@"Outputting playback information for {state}...");
                
                var ignored = new[] { FileType.None, FileType.Text, FileType.Image };
                var fileTypes = Enum.GetValues<FileType>().Except(ignored);
                foreach (var fileType in fileTypes)
                {
                    Save(settings, context, fileType);
                }
            
                log.Info($@"Finished outputting playback information for {state}.");

                _previousState = state;
            }
            catch (Exception e)
            {
                var message = $@"An error occurred processing the output for state {state}. {e.Message}";
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

            if (fileType.HasFlag(FileType.Text))
            {
                File.WriteAllText(fileName, string.Empty);
            }
            else if (fileType.HasFlag(FileType.Image))
            {
                new Bitmap(640, 640).Save(fileName, ImageFormat.Bmp);
            }
        }

        private static void Save(UserSettings settings, OutputContext context, FileType fileType)
        {
            if (fileType == default) return;
            if (!settings.IsOutputEnabled(fileType)) return;

            var fullPath = settings.GetFullPath(fileType);
            var fileName = Path.GetFileName(fullPath);
            var content = GetFileContent(context, fileType);

            DirectoryHelper.EnsureExists(settings.OutputDirectory);

            if (FileType.Text.HasFlag(fileType))
            {
                File.WriteAllText(fullPath, content);
            }
            else if (FileType.Image.HasFlag(fileType))
            {
                WebClientHelper.SaveImage(content, fullPath);
            }

            log.Debug($@"Saved text '{content}' to '{fileName}'.");
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
