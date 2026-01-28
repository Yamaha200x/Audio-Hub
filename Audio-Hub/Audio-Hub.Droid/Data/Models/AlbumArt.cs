using SQLite;

namespace Audio_Hub.Droid.Data.Models;

/// <summary>
/// Album artwork stored separately to keep Track table lightweight.
/// Multiple tracks can share same album art.
/// </summary>
[Table("album_art")]
public class AlbumArt
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, Unique, NotNull]
    public string AlbumKey { get; set; } = string.Empty;  // "Artist - Album"

    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public DateTime CachedDate { get; set; }
}
