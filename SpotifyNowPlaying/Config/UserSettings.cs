using System;
using System.IO;
using System.Reflection;
using DevExpress.Mvvm;
using SpotifyNowPlaying.Common;

namespace SpotifyNowPlaying.Config
{
    public class UserSettings : BindableBase
    {
        private const string DEFAULT_SONG_FILE_NAME = "song.txt";
        private const string DEFAULT_ARTIST_FILE_NAME = "artist.txt";
        private const string DEFAULT_ALBUM_FILE_NAME = "album.txt";
        private const string DEFAULT_PLAYLIST_FILE_NAME = "playlist.txt";
        private const string DEFAULT_ALBUM_ARTWORK_FILE_NAME = "cover.jpg";
        private const string DEFAULT_PLAYLIST_ARTWORK_FILE_NAME = "playlist.jpg";
        private const string DEFAULT_OUTPUT_DIRECTORY = "output";

        private bool _isLoading = true;

        public bool IsDirty
        {
            get => GetProperty(() => IsDirty);
            set => SetProperty(() => IsDirty, value);
        }

        public virtual int RefreshInterval
        {
            get => GetProperty(() => RefreshInterval);
            set => SetProperty(() => RefreshInterval, value, OnPropertyChanged);
        }

        public string OutputDirectory
        {
            get => GetProperty(() => OutputDirectory);
            set => SetProperty(() => OutputDirectory, value, OnPropertyChanged);
        }

        public bool OutputSongFile
        {
            get => GetProperty(() => OutputSongFile);
            set => SetProperty(() => OutputSongFile, value, OnPropertyChanged);
        }
        public string OutputSongFileName
        {
            get => GetProperty(() => OutputSongFileName);
            set => SetProperty(() => OutputSongFileName, value, OnPropertyChanged);
        }

        public bool OutputArtistFile
        {
            get => GetProperty(() => OutputArtistFile);
            set => SetProperty(() => OutputArtistFile, value, OnPropertyChanged);
        }
        public string OutputArtistFileName 
        {
            get => GetProperty(() => OutputArtistFileName);
            set => SetProperty(() => OutputArtistFileName, value, OnPropertyChanged);
        }

        public bool OutputAlbumFile
        {
            get => GetProperty(() => OutputAlbumFile);
            set => SetProperty(() => OutputAlbumFile, value, OnPropertyChanged);
        }
        public string OutputAlbumFileName 
        {
            get => GetProperty(() => OutputAlbumFileName);
            set => SetProperty(() => OutputAlbumFileName, value, OnPropertyChanged);
        }

        public bool OutputPlaylistFile
        {
            get => GetProperty(() => OutputPlaylistFile);
            set => SetProperty(() => OutputPlaylistFile, value, OnPropertyChanged);
        }
        public string OutputPlaylistFileName
        {
            get => GetProperty(() => OutputPlaylistFileName);
            set => SetProperty(() => OutputPlaylistFileName, value, OnPropertyChanged);
        }

        public bool OutputAlbumArtworkFile
        {
            get => GetProperty(() => OutputAlbumArtworkFile);
            set => SetProperty(() => OutputAlbumArtworkFile, value, OnPropertyChanged);
        }
        public string OutputAlbumArtworkFileName
        {
            get => GetProperty(() => OutputAlbumArtworkFileName);
            set => SetProperty(() => OutputAlbumArtworkFileName, value, OnPropertyChanged);
        }
        
        public bool OutputPlaylistArtworkFile
        {
            get => GetProperty(() => OutputPlaylistArtworkFile);
            set => SetProperty(() => OutputPlaylistArtworkFile, value, OnPropertyChanged);
        }
        public string OutputPlaylistArtworkFileName
        {
            get => GetProperty(() => OutputPlaylistArtworkFileName);
            set => SetProperty(() => OutputPlaylistArtworkFileName, value, OnPropertyChanged);
        }

        public UserSettings()
        {
            OutputSongFile = true;
            OutputArtistFile = true;
            OutputAlbumFile = true;
            OutputPlaylistFile = true;
            OutputAlbumArtworkFile = true;
            OutputPlaylistArtworkFile = true;
            OutputSongFileName = DEFAULT_SONG_FILE_NAME;
            OutputArtistFileName = DEFAULT_ARTIST_FILE_NAME;
            OutputAlbumFileName = DEFAULT_ALBUM_FILE_NAME;
            OutputPlaylistFileName = DEFAULT_PLAYLIST_FILE_NAME;
            OutputAlbumArtworkFileName = DEFAULT_ALBUM_ARTWORK_FILE_NAME;
            OutputPlaylistArtworkFileName = DEFAULT_PLAYLIST_ARTWORK_FILE_NAME;
            OutputDirectory = DEFAULT_OUTPUT_DIRECTORY;

            _isLoading = false;
        }

        public bool IsOutputEnabled(FileType fileType)
        {
            return fileType switch
            {
                FileType.Song            => OutputSongFile,
                FileType.Artist          => OutputArtistFile,
                FileType.Album           => OutputAlbumFile,
                FileType.AlbumArtwork    => OutputAlbumArtworkFile,
                FileType.PlaylistName    => OutputPlaylistFile,
                FileType.PlaylistArtwork => OutputPlaylistArtworkFile,
                _                        => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }

        public string GetFullPath(FileType fileType)
        {
            var fileName = fileType switch
            {
                FileType.Song            => OutputSongFileName,
                FileType.Artist          => OutputArtistFileName,
                FileType.Album           => OutputAlbumFileName,
                FileType.AlbumArtwork    => OutputAlbumArtworkFileName,
                FileType.PlaylistName    => OutputPlaylistFileName,
                FileType.PlaylistArtwork => OutputPlaylistArtworkFileName,
                _                        => throw new ArgumentOutOfRangeException(nameof(fileType))
            };

            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            var filePath = Path.Join(OutputDirectory, fileName);

            return Path.GetFullPath(filePath);
        }
        
        public void OnSave()
        {
            IsDirty = false;
        }

        private void OnPropertyChanged()
        {
            if (_isLoading) return;

            IsDirty = true;
        }
    }
}
