namespace Audio_Hub.Droid.Services;

/// <summary>
/// Extracts song info (title, artist, album art) from audio files.
/// Frontend: Use to display track information in your UI.
/// </summary>
public interface IAudioMetadataService
{
    /// <summary>Synchronous metadata extraction - blocks until complete</summary>
    AudioMetadata GetMetadata(string filePath);

    /// <summary>Async metadata extraction - recommended for UI code</summary>
    Task<AudioMetadata> GetMetadataAsync(string filePath);

    /// <summary>Extracts only album artwork. Returns null if none exists.</summary>
    byte[]? GetAlbumArt(string filePath);
}

/// <summary>Song metadata container</summary>
public class AudioMetadata
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string? Genre { get; set; }
    public uint? Year { get; set; }
    public TimeSpan Duration { get; set; }  // Use Duration.TotalSeconds for progress bars
    public int? TrackNumber { get; set; }
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;  // Use this to play the file
    public byte[]? AlbumArt { get; set; }  // Convert to ImageSource for display
}
