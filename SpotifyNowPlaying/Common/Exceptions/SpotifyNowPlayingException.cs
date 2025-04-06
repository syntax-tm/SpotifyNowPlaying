using System;

namespace SpotifyNowPlaying;

[Serializable]
public class SpotifyNowPlayingException : Exception
{
    public SpotifyNowPlayingException(string message, Exception innerException = null)
        : base(message, innerException)
    {

    }
}
