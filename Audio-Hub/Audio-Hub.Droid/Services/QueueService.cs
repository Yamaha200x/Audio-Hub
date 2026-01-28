using Audio_Hub.Droid.Data;
using Audio_Hub.Droid.Data.Models;

namespace Audio_Hub.Droid.Services;

/// <summary>
/// Queue management with shuffle and repeat support.
/// Persists queue state to database.
/// </summary>
public class QueueService : IQueueService
{
    private readonly IDatabaseService _databaseService;
    private readonly ISettingsService _settingsService;
    private List<QueueItem> _currentQueue = new();
    private int _currentIndex = 0;

    public QueueService(IDatabaseService databaseService, ISettingsService settingsService)
    {
        _databaseService = databaseService;
        _settingsService = settingsService;
    }

    
    public async Task AddToQueueAsync(int trackId)
    {
        var queueItem = new QueueItem
        {
            TrackId = trackId,
            Position = _currentQueue.Count,
            IsCurrentTrack = false,
            AddedToQueue = DateTime.Now
        };
        
        _currentQueue.Add(queueItem);
        await SaveQueueStateAsync();
    }

    public async Task AddToQueueNextAsync(int trackId)
    {
        var queueItem = new QueueItem
        {
            TrackId = trackId,
            Position = _currentIndex + 1,
            IsCurrentTrack = false,
            AddedToQueue = DateTime.Now
        };
        
        _currentQueue.Insert(_currentIndex + 1, queueItem);
        
        // Reorder positions
        for (int i = 0; i < _currentQueue.Count; i++)
        {
            _currentQueue[i].Position = i;
        }
        
        await SaveQueueStateAsync();
    }

    public async Task<List<Track>> GetQueueAsync()
    {
        var tracks = new List<Track>();
        
        foreach (var queueItem in _currentQueue)
        {
            var track = await _databaseService.GetTrackByIdAsync(queueItem.TrackId);
            if (track != null)
            {
                tracks.Add(track);
            }
        }
        
        return tracks;
    }

    public async Task ClearQueueAsync()
    {
        _currentQueue.Clear();
        _currentIndex = 0;
        await _databaseService.ClearQueueAsync();
    }

    public async Task RemoveFromQueueAsync(int queueItemId)
    {
        _currentQueue.RemoveAll(q => q.Id == queueItemId);
        
        // Reorder positions
        for (int i = 0; i < _currentQueue.Count; i++)
        {
            _currentQueue[i].Position = i;
        }
        
        await SaveQueueStateAsync();
    }

    public async Task ReorderQueueAsync(int fromPosition, int toPosition)
    {
        if (fromPosition < 0 || fromPosition >= _currentQueue.Count ||
            toPosition < 0 || toPosition >= _currentQueue.Count)
            return;

        var item = _currentQueue[fromPosition];
        _currentQueue.RemoveAt(fromPosition);
        _currentQueue.Insert(toPosition, item);
        
        // Update positions
        for (int i = 0; i < _currentQueue.Count; i++)
        {
            _currentQueue[i].Position = i;
        }
        
        await SaveQueueStateAsync();
    }


    public async Task<Track?> GetCurrentTrackAsync()
    {
        if (_currentIndex < 0 || _currentIndex >= _currentQueue.Count)
            return null;
        
        var queueItem = _currentQueue[_currentIndex];
        return await _databaseService.GetTrackByIdAsync(queueItem.TrackId);
    }

    public async Task<Track?> GetNextTrackAsync()
    {
        var repeatMode = await _settingsService.GetRepeatModeAsync();
        
        if (repeatMode == "One")
        {
            return await GetCurrentTrackAsync();
        }
        
        if (_currentIndex + 1 < _currentQueue.Count)
        {
            var nextItem = _currentQueue[_currentIndex + 1];
            return await _databaseService.GetTrackByIdAsync(nextItem.TrackId);
        }
        
        if (repeatMode == "All" && _currentQueue.Count > 0)
        {
            var firstItem = _currentQueue[0];
            return await _databaseService.GetTrackByIdAsync(firstItem.TrackId);
        }
        
        return null;
    }

    public async Task<Track?> GetPreviousTrackAsync()
    {
        var repeatMode = await _settingsService.GetRepeatModeAsync();
        
        if (repeatMode == "One")
        {
            return await GetCurrentTrackAsync();
        }
        
        if (_currentIndex - 1 >= 0)
        {
            var prevItem = _currentQueue[_currentIndex - 1];
            return await _databaseService.GetTrackByIdAsync(prevItem.TrackId);
        }
        
        if (repeatMode == "All" && _currentQueue.Count > 0)
        {
            var lastItem = _currentQueue[^1];
            return await _databaseService.GetTrackByIdAsync(lastItem.TrackId);
        }
        
        return null;
    }

    public async Task MoveToNextAsync()
    {
        var nextTrack = await GetNextTrackAsync();
        if (nextTrack != null)
        {
            _currentIndex++;
            if (_currentIndex >= _currentQueue.Count)
            {
                _currentIndex = 0; // Loop back
            }
            await UpdateCurrentTrackMarker();
        }
    }

    public async Task MoveToPreviousAsync()
    {
        var prevTrack = await GetPreviousTrackAsync();
        if (prevTrack != null)
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _currentQueue.Count - 1; // Loop to end
            }
            await UpdateCurrentTrackMarker();
        }
    }


    public async Task SetShuffleAsync(bool enabled)
    {
        await _settingsService.SetShuffleEnabledAsync(enabled);
        
        if (enabled && _currentQueue.Count > 1)
        {
            // Keep current track, shuffle the rest
            var currentTrack = _currentQueue[_currentIndex];
            var random = new Random();
            _currentQueue = _currentQueue.OrderBy(x => random.Next()).ToList();
            _currentIndex = _currentQueue.IndexOf(currentTrack);
            
            // Update positions
            for (int i = 0; i < _currentQueue.Count; i++)
            {
                _currentQueue[i].Position = i;
            }
            
            await SaveQueueStateAsync();
        }
    }

    public Task SetRepeatModeAsync(string mode)
    {
        return _settingsService.SetRepeatModeAsync(mode);
    }

    public Task<bool> IsShuffleEnabledAsync()
    {
        return _settingsService.GetShuffleEnabledAsync();
    }

    public Task<string> GetRepeatModeAsync()
    {
        return _settingsService.GetRepeatModeAsync();
    }


    public async Task SaveQueueStateAsync()
    {
        await _databaseService.SaveQueueAsync(_currentQueue);
    }

    public async Task RestoreQueueStateAsync()
    {
        _currentQueue = await _databaseService.GetQueueAsync();
        
        // Find current track
        var currentItem = _currentQueue.FirstOrDefault(q => q.IsCurrentTrack);
        if (currentItem != null)
        {
            _currentIndex = _currentQueue.IndexOf(currentItem);
        }
    }


    private async Task UpdateCurrentTrackMarker()
    {
        // Mark all as not current
        foreach (var item in _currentQueue)
        {
            item.IsCurrentTrack = false;
        }
        
        // Mark current track
        if (_currentIndex >= 0 && _currentIndex < _currentQueue.Count)
        {
            _currentQueue[_currentIndex].IsCurrentTrack = true;
        }
        
        await SaveQueueStateAsync();
    }
}
