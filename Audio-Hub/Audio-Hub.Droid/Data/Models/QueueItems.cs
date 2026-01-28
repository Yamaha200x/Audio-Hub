using SQLite;

namespace Audio_Hub.Droid.Data.Models;

/// <summary>
/// Persistent play queue - saves current queue when app closes.
/// Frontend: Restore queue on app startup for seamless continuation.
/// </summary>
[Table("queue_items")]
public class QueueItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, NotNull]
    public int TrackId { get; set; }

    public int Position { get; set; }  // Order in queue (0 = next up)
    public bool IsCurrentTrack { get; set; }  // True for the playing track
    public int CurrentPositionMs { get; set; }  // Playback position if paused mid-song
    
    public DateTime AddedToQueue { get; set; }
}
