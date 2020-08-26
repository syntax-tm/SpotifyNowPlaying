using System;
using System.Diagnostics;
using JetBrains.Annotations;
using SpotifyNowPlaying.Extensions;

namespace SpotifyNowPlaying
{
    [DebuggerDisplay("{Title}")]
    public class TrackInfo : IEquatable<TrackInfo>
    {
        public string Artist { get; set; }
        public string Song { get; set; }
        public string Title => $"{Artist} - {Song}";

        public TrackInfo()
        {

        }

        public TrackInfo([NotNull] string windowTitle)
        {
            var splitTitle = windowTitle.Split(new[] { '-' }, 2);

            Artist = splitTitle[0].Trim();
            Song = splitTitle[1].Trim();
        }

        public TrackInfo([NotNull] string artist, [NotNull] string song)
        {
            Artist = artist;
            Song = song;
        }

        public override string ToString()
        {
            return Title;
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }

        public bool Equals(TrackInfo other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Title.EqualsIgnoreCase(other.Title);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(TrackInfo)) throw new ArgumentException(nameof(obj));

            return Equals((TrackInfo) obj);
        }

        public static bool operator ==(TrackInfo a, TrackInfo b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(TrackInfo a, TrackInfo b)
        {
            return !(a == b);
        }
    }
}
