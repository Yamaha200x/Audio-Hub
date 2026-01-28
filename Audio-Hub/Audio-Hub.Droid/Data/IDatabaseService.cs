using Audio_Hub.Droid.Data.Models;

namespace Audio_Hub.Droid.Data;

/// <summary>
/// Database operations for music library persistence.
/// Frontend: Use this to cache metadata and manage playlists/history.
/// </summary>
public interface IDatabaseService
{
    // Initialization
    Task InitializeAsync();

    // Track operations
    Task<int> SaveTrackAsync(Track track);
    Task<List<Track>> GetAllTracksAsync();
    Task<Track?> GetTrackByPathAsync(string filePath);
    Task<Track?> GetTrackByIdAsync(int trackId); 
    Task<List<Track>> GetFavoriteTracksAsync();
    Task UpdatePlayCountAsync(int trackId);
    Task<int> DeleteTrackAsync(Track track);

    // Playlist operations
    Task<int> CreatePlaylistAsync(Playlist playlist);
    Task<List<Playlist>> GetAllPlaylistsAsync();
    Task<Playlist?> GetPlaylistAsync(int playlistId);
    Task<int> UpdatePlaylistAsync(Playlist playlist);
    Task<int> DeletePlaylistAsync(int playlistId);
    
    // Playlist-Track operations
    Task AddTrackToPlaylistAsync(int playlistId, int trackId);
    Task<List<Track>> GetPlaylistTracksAsync(int playlistId);
    Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId);
    
    // Play history
    Task<int> AddPlayHistoryAsync(PlayHistory history);
    Task<List<Track>> GetRecentlyPlayedAsync(int limit = 50);
    Task<Dictionary<int, int>> GetMostPlayedTracksAsync(int limit = 20);

    // Album art
    Task<int> SaveAlbumArtAsync(AlbumArt albumArt);
    Task<AlbumArt?> GetAlbumArtAsync(string albumKey);

    // Utility
    Task<int> GetLibrarySizeAsync();
    Task ClearAllDataAsync();

    Task SaveQueueAsync(List<QueueItem> queue);
    Task<List<QueueItem>> GetQueueAsync();
    Task ClearQueueAsync();
    Task<QueueItem?> GetCurrentTrackFromQueueAsync();
    Task UpdateCurrentTrackPositionAsync(int queueItemId, int positionMs);

    // Settings operations
    Task<AppSettings> GetSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
    Task<T> GetSettingAsync<T>(string key, T defaultValue);
    Task SetSettingAsync<T>(string key, T value);
}
