using System;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace SpotifyNowPlaying.Common
{
    public class SpotifyPlaybackState : IEquatable<SpotifyPlaybackState>
    {
        public string Id => Track?.Id ?? string.Empty;
        public string PlaylistId => Playlist?.Id ?? string.Empty;

        public bool IsPlaying => Track != null;
        public bool IsStopped => !IsPlaying;
        public bool HasPlaylist => Playlist != null;

        public FullTrack Track { get; private set; }
        public FullPlaylist Playlist { get; private set; }

        protected SpotifyPlaybackState()
        {

        }
        
        public static async Task<SpotifyPlaybackState> Create(CurrentlyPlaying current)
        {
            if (current == null) return new SpotifyPlaybackState();
            if (!current.IsPlaying) return new SpotifyPlaybackState();

            var state = new SpotifyPlaybackState();
            
            if (current.Item is FullTrack track)
            {
                state.Track = track;
            }

            if (current.Context?.Type == "playlist")
            {
                var playlistId = current.Context.Href.Split('/').Last();
                var playlist = await SpotifyClientHelper.Client.Playlists.Get(playlistId);
                
                state.Playlist = playlist;
            }

            return state;
        }

        public override bool Equals(object obj)
        {
            if (obj is SpotifyPlaybackState otherState)
            {
                return Equals(otherState);
            }
            return false;
        }

        public override string ToString()
        {
            return $"{Id}:{PlaylistId}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(SpotifyPlaybackState other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            return GetHashCode() == other.GetHashCode();
        }
    }
}
