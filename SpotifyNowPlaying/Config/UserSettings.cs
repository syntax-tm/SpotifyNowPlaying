using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpotifyNowPlaying.Config
{
    public class UserSettings
    {
        private const string DEFAULT_SONG_FILE_NAME = "song.txt";
        private const string DEFAULT_ARTIST_FILE_NAME = "artist.txt";
        private const string DEFAULT_ALBUM_FILE_NAME = "album.txt";
        private const string DEFAULT_PLAYLIST_FILE_NAME = "playlist.txt";
        private const string DEFAULT_ALBUM_ARTWORK_FILE_NAME = "cover.jpg";
        private const string DEFAULT_PLAYLIST_ARTWORK_FILE_NAME = "playlist.jpg";
        private const string DEFAULT_OUTPUT_DIRECTORY = "output";
        
        public int RefreshInterval { get; set; }

        public string OutputDirectory { get; set; }

        public bool OutputSongFile { get; set; }
        public string OutputSongFileName { get; set; }

        public bool OutputArtistFile { get; set; }
        public string OutputArtistFileName { get; set; }

        public bool OutputAlbumFile { get; set; }
        public string OutputAlbumFileName { get; set; }

        public bool OutputPlaylistFile { get; set; }
        public string OutputPlaylistFileName { get; set; }

        public bool OutputAlbumArtworkFile { get; set; }
        public string OutputAlbumArtworkFileName { get; set; }
        
        public bool OutputPlaylistArtworkFile { get; set; }
        public string OutputPlaylistArtworkFileName { get; set; }

        public UserSettings()
        {

        }
        
        public static UserSettings Default => new()
        {
            OutputSongFile = true,
            OutputArtistFile = true,
            OutputAlbumFile = true,
            OutputPlaylistFile = true,
            OutputAlbumArtworkFile = true,
            OutputPlaylistArtworkFile = true,
            OutputSongFileName = DEFAULT_SONG_FILE_NAME,
            OutputArtistFileName = DEFAULT_ARTIST_FILE_NAME,
            OutputAlbumFileName = DEFAULT_ALBUM_FILE_NAME,
            OutputPlaylistFileName = DEFAULT_PLAYLIST_FILE_NAME,
            OutputAlbumArtworkFileName = DEFAULT_ALBUM_ARTWORK_FILE_NAME,
            OutputPlaylistArtworkFileName = DEFAULT_PLAYLIST_ARTWORK_FILE_NAME,
            OutputDirectory = DEFAULT_OUTPUT_DIRECTORY
        };
    }
}
