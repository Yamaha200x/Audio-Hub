using Audio_Hub.Droid.Services;
using Audio_Hub.Droid.Data;
using Audio_Hub.Droid.Data.Models;

namespace Audio_Hub.Droid.Platforms.Android;

/// <summary>
/// Scans common Android music folders: /Music, /Download, /sdcard variations.
/// Caches results in database for fast subsequent access.
/// </summary>
public class MusicLibraryService : IMusicLibraryService
{
    private readonly IAudioMetadataService _metadataService;
    private readonly IDatabaseService _databaseService;

    public MusicLibraryService(
        IAudioMetadataService metadataService,
        IDatabaseService databaseService)
    {
        _metadataService = metadataService;
        _databaseService = databaseService;
    }

    public async Task<List<AudioMetadata>> ScanLibraryAsync(IProgress<ScanProgress>? progress = null)
    {
        var library = new List<AudioMetadata>();
        
        // Standard Android music directories
        var musicPaths = new[]
        {
            "/storage/emulated/0/Music",
            "/storage/emulated/0/Download",
            "/sdcard/Music",
            "/sdcard/Download"
        };

        var audioFiles = new List<string>();
        
        // Recursively find all audio files
        foreach (var path in musicPaths)
        {
            if (Directory.Exists(path))
            {
                audioFiles.AddRange(Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories));
                audioFiles.AddRange(Directory.GetFiles(path, "*.m4a", SearchOption.AllDirectories));
                audioFiles.AddRange(Directory.GetFiles(path, "*.flac", SearchOption.AllDirectories));
                audioFiles.AddRange(Directory.GetFiles(path, "*.wav", SearchOption.AllDirectories));
                audioFiles.AddRange(Directory.GetFiles(path, "*.ogg", SearchOption.AllDirectories));
            }
        }

        var distinctFiles = audioFiles.Distinct().ToList();
        var totalFiles = distinctFiles.Count;

        // Extract metadata for each file and save to database
        for (int i = 0; i < distinctFiles.Count; i++)
        {
            var file = distinctFiles[i];
            try
            {
                var metadata = await _metadataService.GetMetadataAsync(file);
                library.Add(metadata);

                // Save track to database
                var track = new Track
                {
                    FilePath = metadata.FilePath,
                    Title = metadata.Title,
                    Artist = metadata.Artist,
                    Album = metadata.Album,
                    Genre = metadata.Genre,
                    Year = (int?)metadata.Year,
                    DurationMs = (int)metadata.Duration.TotalMilliseconds,
                    TrackNumber = metadata.TrackNumber,
                    FileSize = metadata.FileSize,
                    DateAdded = DateTime.Now,
                    DateModified = DateTime.Now,
                    HasAlbumArt = metadata.AlbumArt != null
                };
                
                await _databaseService.SaveTrackAsync(track);
                
                // Save album art if exists
                if (metadata.AlbumArt != null)
                {
                    var albumKey = $"{metadata.Artist} - {metadata.Album}";
                    await _databaseService.SaveAlbumArtAsync(new AlbumArt
                    {
                        AlbumKey = albumKey,
                        ImageData = metadata.AlbumArt
                    });
                }

                progress?.Report(new ScanProgress
                {
                    FilesScanned = i + 1,
                    TotalFiles = totalFiles,
                    CurrentFile = Path.GetFileName(file)
                });
            }
            catch (Exception ex)
            {
                global::Android.Util.Log.Error("MusicLibrary", $"Error scanning {file}: {ex.Message}");
            }
        }

        return library;
    }

    public async Task<List<AudioMetadata>> GetAllSongsAsync()
    {
        var tracks = await _databaseService.GetAllTracksAsync();
        return tracks.Select(MapToAudioMetadata).ToList();
    }

    public async Task<List<string>> GetAllArtistsAsync()
    {
        var tracks = await _databaseService.GetAllTracksAsync();
        var artists = tracks
            .Select(t => t.Artist)
            .Distinct()
            .OrderBy(a => a)
            .ToList();
        return artists;
    }

    public async Task<List<string>> GetAllAlbumsAsync()
    {
        var tracks = await _databaseService.GetAllTracksAsync();
        var albums = tracks
            .Select(t => t.Album)
            .Distinct()
            .OrderBy(a => a)
            .ToList();
        return albums;
    }

    public async Task<List<AudioMetadata>> GetSongsByArtistAsync(string artist)
    {
        var tracks = await _databaseService.GetAllTracksAsync();
        var songs = tracks
            .Where(t => t.Artist == artist)
            .OrderBy(t => t.Album)
            .ThenBy(t => t.TrackNumber)
            .Select(MapToAudioMetadata)
            .ToList();
        return songs;
    }

    public async Task<List<AudioMetadata>> GetSongsByAlbumAsync(string album)
    {
        var tracks = await _databaseService.GetAllTracksAsync();
        var songs = tracks
            .Where(t => t.Album == album)
            .OrderBy(t => t.TrackNumber)
            .Select(MapToAudioMetadata)
            .ToList();
        return songs;
    }

    private AudioMetadata MapToAudioMetadata(Track track)
    {
        return new AudioMetadata
        {
            Title = track.Title,
            Artist = track.Artist,
            Album = track.Album,
            Genre = track.Genre,
            Year = (uint?)track.Year,
            Duration = TimeSpan.FromMilliseconds(track.DurationMs),
            TrackNumber = track.TrackNumber,
            FileSize = track.FileSize,
            FilePath = track.FilePath,
            AlbumArt = null  // Load separately if needed for performance
        };
    }
}
