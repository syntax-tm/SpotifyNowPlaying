using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace SpotifyNowPlaying.Output
{
    public class OutputContext
    {
        [JsonProperty("song")]
        public SongContext Song { get; set; }
        [JsonProperty("artist")]
        public ArtistContext Artist { get; set; }
        [JsonProperty("album")]
        public AlbumContext Album { get; set; }
        [JsonProperty("playlist")]
        public PlaylistContext Playlist { get; set; }

        public OutputContext()
        {
            Song = new ();
            Artist = new ();
            Album = new ();
            Playlist = new ();
        }

        public OutputContext([JetBrains.Annotations.NotNull] SpotifyPlaybackState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (state.Track is null)
            {
                Song = new ();
                Artist = new ();
                Album = new ();
                Playlist = new ();

                return;
            }

            var trackJson = JsonConvert.SerializeObject(state.Track);
            Song = JsonConvert.DeserializeObject<SongContext>(trackJson);

            var artistJson = JsonConvert.SerializeObject(state.Track.Artists.First());
            Artist = JsonConvert.DeserializeObject<ArtistContext>(artistJson);

            var albumJson = JsonConvert.SerializeObject(state.Track.Album);
            Album = JsonConvert.DeserializeObject<AlbumContext>(albumJson);

            if (state.Playlist == null)
            {
                Playlist = new ();
            }
            else
            {
                var playlistJson = JsonConvert.SerializeObject(state.Playlist);
                Playlist = JsonConvert.DeserializeObject<PlaylistContext>(playlistJson);
            }
        }
    }
    
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SongContext
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("popularity")]
        public int Popularity { get; set; }
        [JsonProperty("trackNumber")]
        public int TrackNumber { get; set; }
        [JsonProperty("isLocal")]
        public bool IsLocal { get; set; }
        [JsonProperty("explicit")]
        public bool Explicit { get; set; }
        [JsonProperty("discNumber")]
        public int DiscNumber { get; set; }
        [JsonProperty("url")]
        public string Uri { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ArtistContext
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("uri")]
        public string Uri { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class AlbumContext
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("uri")]
        public string Uri { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("albumType")]
        public string AlbumType { get; set; }
        [JsonProperty("releaseDate")]
        public DateTime ReleaseDate { get; set; }
        [JsonProperty("totalTracks")]
        public int TotalTracks { get; set; }
        [JsonProperty("imageUrl")]
        public string ImageUrl => Images?.FirstOrDefault()?.Url;
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PlaylistContext
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("uri")]
        public string Uri { get; set; }
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("collaborative")]
        public bool Collaborative { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("owner")]
        public PublicUser Owner { get; set; }
        [JsonProperty("public")]
        public bool Public { get; set; }
        [JsonProperty("snapshotId")]
        public string SnapshotId { get; set; }
        [JsonProperty("imageUrl")]
        public string ImageUrl => Images?.FirstOrDefault()?.Url;
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
