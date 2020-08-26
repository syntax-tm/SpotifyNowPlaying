using System.Diagnostics;
using System.Linq;
using log4net;
using SpotifyNowPlaying.Extensions;

namespace SpotifyNowPlaying
{
    public static class SpotifyHelper
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(SpotifyHelper));

        private const string PROCESS_NAME = "Spotify";
        private const string DEFAULT_WINDOW_TITLE = "Spotify";

        public static TrackInfo CurrentlyPlaying { get; private set; }
        public static TrackInfo PreviouslyPlaying { get; private set; }

        public static string CurrentTitle => CurrentlyPlaying?.Title ?? "Paused";

        public static bool? GetCurrentlyPlaying()
        {
            var processes = Process.GetProcessesByName(PROCESS_NAME);
            if (!processes.Any())
            {
                log.Warn("Spotify is not currently running. Please start Spotify.");
                return null;
            }

            // get the process with an active window (if any)
            var spotifyProcess = processes.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            var title = spotifyProcess?.MainWindowTitle;
            
            TrackInfo current = null;

            if (!string.IsNullOrWhiteSpace(title) && !title.ContainsIgnoreCase(DEFAULT_WINDOW_TITLE))
            {
                current = new TrackInfo(title);
            }

            var isPlaying = current != null;
            var wasPlaying = PreviouslyPlaying != null;

            // playback has never started
            if (!isPlaying && !wasPlaying)
            {
                log.Warn("Spotify is running but playback is stopped.");
                return null;
            }
            
            // currently playing is different from previous, so set previous to the saved current
            // and then update the saved current with the new info
            // TODO: change the previously playing to a Stack instead of only storing one track
            var isChanged = current?.Title != CurrentlyPlaying?.Title;
            if (isChanged)
            {
                PreviouslyPlaying = CurrentlyPlaying;
                CurrentlyPlaying = current;
            }
            
            // log the current and previous (after updating, if necessary)
            log.Info($"Currently playing: {CurrentlyPlaying?.Title}");
            log.Info($"Prev song: {PreviouslyPlaying?.Title}");

            return isChanged;
        }
    }
}
