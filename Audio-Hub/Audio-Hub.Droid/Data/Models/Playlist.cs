using SQLite;

namespace Audio_Hub.Droid.Data.Models;

/// <summary>
/// User-created playlists
/// </summary>
[Table("playlists")]
public class Playlist
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    
    public int TrackCount { get; set; }  // Cached count for performance
}
