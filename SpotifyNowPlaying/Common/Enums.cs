using System;
using System.ComponentModel;

namespace SpotifyNowPlaying;

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

[Flags]
public enum FileType : ushort
{
    None            = 0,
    Song            = 1,
    Artist          = 2,
    Album           = 4,
    PlaylistName    = 8,
    AlbumArtwork    = 1024,
    PlaylistArtwork = 2048,
    Image           = AlbumArtwork | PlaylistArtwork,
    Text            = Song | Artist | Album | PlaylistName
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
