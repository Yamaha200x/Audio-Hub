using Audio_Hub.Droid.Services;

namespace Audio_Hub.Droid.Platforms.Android;

/// <summary>
/// Scans common Android music folders: /Music, /Download, /sdcard variations.
/// Caches results in memory for fast subsequent access.
/// </summary>
public class MusicLibraryService : IMusicLibraryService
{
    private readonly IAudioMetadataService _metadataService;
    private List<AudioMetadata> _cachedLibrary = new();

    public MusicLibraryService(IAudioMetadataService metadataService)
    {
        _metadataService = metadataService;
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

        // Extract metadata for each file
        for (int i = 0; i < distinctFiles.Count; i++)
        {
            var file = distinctFiles[i];
            try
            {
                var metadata = await _metadataService.GetMetadataAsync(file);
                library.Add(metadata);

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

        _cachedLibrary = library;
        return library;
    }

    public Task<List<AudioMetadata>> GetAllSongsAsync()
    {
        return Task.FromResult(_cachedLibrary);
    }

    public Task<List<string>> GetAllArtistsAsync()
    {
        var artists = _cachedLibrary
            .Select(s => s.Artist)
            .Distinct()
            .OrderBy(a => a)
            .ToList();
        return Task.FromResult(artists);
    }

    public Task<List<string>> GetAllAlbumsAsync()
    {
        var albums = _cachedLibrary
            .Select(s => s.Album)
            .Distinct()
            .OrderBy(a => a)
            .ToList();
        return Task.FromResult(albums);
    }

    public Task<List<AudioMetadata>> GetSongsByArtistAsync(string artist)
    {
        var songs = _cachedLibrary
            .Where(s => s.Artist == artist)
            .OrderBy(s => s.Album)
            .ThenBy(s => s.TrackNumber)
            .ToList();
        return Task.FromResult(songs);
    }

    public Task<List<AudioMetadata>> GetSongsByAlbumAsync(string album)
    {
        var songs = _cachedLibrary
            .Where(s => s.Album == album)
            .OrderBy(s => s.TrackNumber)
            .ToList();
        return Task.FromResult(songs);
    }
}
