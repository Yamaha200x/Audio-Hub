using SQLite;

namespace Audio_Hub.Droid.Data.Models;

/// <summary>
/// Junction table: Links playlists to tracks (many-to-many)
/// </summary>
[Table("playlist_tracks")]
public class PlaylistTrack
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, NotNull]
    public int PlaylistId { get; set; }

    [Indexed, NotNull]
    public int TrackId { get; set; }

    public int Position { get; set; }  // Track order in playlist
    public DateTime AddedDate { get; set; }
}
