using Audio_Hub.Droid.Data.Models;

namespace Audio_Hub.Droid.Services;
/// <summary>
/// Manages play queue and playback order.
/// Frontend: Use to add songs to queue, get next track, handle shuffle/repeat.
/// </summary>
public interface IQueueService
{
    // Queue management
    Task AddToQueueAsync(int trackId);
    Task AddToQueueNextAsync(int trackId);  // Add as next track
    Task<List<Track>> GetQueueAsync();
    Task ClearQueueAsync();
    Task RemoveFromQueueAsync(int queueItemId);
    Task ReorderQueueAsync(int fromPosition, int toPosition);
    
    // Playback navigation
    Task<Track?> GetCurrentTrackAsync();
    Task<Track?> GetNextTrackAsync();
    Task<Track?> GetPreviousTrackAsync();
    Task MoveToNextAsync();
    Task MoveToPreviousAsync();
    
    // Playback modes
    Task SetShuffleAsync(bool enabled);
    Task SetRepeatModeAsync(string mode);  // None, One, All
    Task<bool> IsShuffleEnabledAsync();
    Task<string> GetRepeatModeAsync();
    
    // Persistence
    Task SaveQueueStateAsync();
    Task RestoreQueueStateAsync();
}
