using SQLite;

namespace Audio_Hub.Droid.Data.Models;

/// <summary>
/// Tracks listening history for statistics and "recently played"
/// </summary>
[Table("play_history")]
public class PlayHistory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, NotNull]
    public int TrackId { get; set; }

    [Indexed]
    public DateTime PlayedAt { get; set; }

    public int DurationPlayedMs { get; set; }  // How long user listened
    public bool Completed { get; set; }  // True if played to end
}
