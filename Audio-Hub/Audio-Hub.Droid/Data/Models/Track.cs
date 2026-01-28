using SQLite;

namespace Audio_Hub.Droid.Data.Models;

/// <summary>
/// Cached song metadata stored in database.
/// Avoids re-scanning files on every app launch.
/// </summary>
[Table("tracks")]
public class Track
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, NotNull]
    public string FilePath { get; set; } = string.Empty;  // Unique file location

    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string? Genre { get; set; }
    public int? Year { get; set; }
    public int DurationMs { get; set; }  // Duration in milliseconds
    public int? TrackNumber { get; set; }
    public long FileSize { get; set; }
    
    public bool IsFavorite { get; set; }  // Quick favorite flag
    public int PlayCount { get; set; }    // How many times played
    public DateTime? LastPlayed { get; set; }  // When last played
    
    public DateTime DateAdded { get; set; }  // When added to library
    public DateTime DateModified { get; set; }  // Last file modification
    
    // Album art stored separately for performance
    public bool HasAlbumArt { get; set; }
}
