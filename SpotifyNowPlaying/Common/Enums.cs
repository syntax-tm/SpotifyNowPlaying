using System.ComponentModel;

namespace SpotifyNowPlaying.Common
{
    [DefaultValue(Text)]
    public enum OutputFileType
    {
        Text = 0,
        Image
    }

    [DefaultValue(Normal)]
    public struct AppExitCode
    {
        public const int UnhandledException = -3;
        public const int LoginCancelled = -2;
        public const int LoginFailed = -1;
        public const int Normal = 0;
    }

    public enum FileCategory
    {
        [Description("")]
        Default,
        [Description("playlists")]
        Playlist,
        [Description("users")]
        User,
        [Description("tracks")]
        Track,
        [Description("albums")]
        Album,
        [Description("misc")]
        Misc,
        [Description("settings")]
        Settings
    }
}
