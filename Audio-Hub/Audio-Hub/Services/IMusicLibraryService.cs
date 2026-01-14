namespace Audio_Hub.Droid.Services;

/// <summary>
/// Scans device and manages music library.
/// Frontend: Call ScanLibraryAsync() on startup, then use Get methods to populate lists.
/// </summary>
public interface IMusicLibraryService
{
    /// <summary>Scans device for audio files. Takes 30-60s for large libraries. Show loading screen.</summary>
    Task<List<AudioMetadata>> ScanLibraryAsync(IProgress<ScanProgress>? progress = null);

    /// <summary>Returns cached songs (fast). Empty if not scanned yet.</summary>
    Task<List<AudioMetadata>> GetAllSongsAsync();

    /// <summary>Returns unique artists, alphabetically sorted</summary>
    Task<List<string>> GetAllArtistsAsync();

    /// <summary>Returns unique albums, alphabetically sorted</summary>
    Task<List<string>> GetAllAlbumsAsync();

    /// <summary>Returns all songs by artist, sorted by album/track</summary>
    Task<List<AudioMetadata>> GetSongsByArtistAsync(string artist);

    /// <summary>Returns all songs from album, sorted by track number</summary>
    Task<List<AudioMetadata>> GetSongsByAlbumAsync(string album);
}

/// <summary>Library scan progress for UI updates</summary>
public class ScanProgress
{
    public int FilesScanned { get; set; }
    public int TotalFiles { get; set; }
    public string CurrentFile { get; set; } = string.Empty;
    public double PercentComplete => TotalFiles > 0 ? (double)FilesScanned / TotalFiles * 100 : 0;
}
