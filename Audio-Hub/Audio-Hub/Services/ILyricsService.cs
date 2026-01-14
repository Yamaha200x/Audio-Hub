namespace Audio_Hub.Droid.Services;

/// <summary>
/// Fetches lyrics from LRCLIB API (free, no API key needed).
/// Frontend: Returns null if lyrics not found - handle gracefully.
/// </summary>
public interface ILyricsService
{
    /// <summary>Fetches lyrics by track/artist. Album and duration improve accuracy.</summary>
    Task<Lyrics?> GetLyricsAsync(string trackName, string artistName, string? albumName = null, int? duration = null);

    /// <summary>Convenience method using AudioMetadata object</summary>
    Task<Lyrics?> GetLyricsByMetadataAsync(AudioMetadata metadata);
}

/// <summary>Lyrics container - use PlainLyrics for display or SyncedLyrics for karaoke</summary>
public class Lyrics
{
    public string PlainLyrics { get; set; } = string.Empty;  // Multi-line text
    public string? SyncedLyrics { get; set; }  // LRC format: [mm:ss.xx]Lyric text
    public string Language { get; set; } = "en";
    public int? Duration { get; set; }
}
