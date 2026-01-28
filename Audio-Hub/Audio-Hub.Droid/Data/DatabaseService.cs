using SQLite;
using Audio_Hub.Droid.Data.Models;

namespace Audio_Hub.Droid.Data;

/// <summary>
/// SQLite implementation for local database storage.
/// Uses sqlite-net-pcl for async operations.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        // Store database in app's local data folder
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "audiohub.db3");
    }

    public async Task InitializeAsync()
    {
        if (_database != null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);
        
        // Create tables if they don't exist
        await _database.CreateTableAsync<Track>();
        await _database.CreateTableAsync<Playlist>();
        await _database.CreateTableAsync<PlaylistTrack>();
        await _database.CreateTableAsync<PlayHistory>();
        await _database.CreateTableAsync<AlbumArt>();
        
        // Enable foreign key support
        await _database.ExecuteAsync("PRAGMA foreign_keys = ON;");

        await _database.CreateTableAsync<QueueItem>();
    await _database.CreateTableAsync<AppSettings>();
    
    // Initialize default settings if none exist
    var settings = await GetSettingsAsync();
    if (settings == null)
    {
        await SaveSettingsAsync(new AppSettings());
    }
    }



public async Task SaveQueueAsync(List<QueueItem> queue)
{
    var db = await GetDatabaseAsync();
    
    // Clear existing queue
    await ClearQueueAsync();
    
    // Insert new queue
    foreach (var item in queue)
    {
        item.AddedToQueue = DateTime.Now;
        await db.InsertAsync(item);
    }
}

public async Task<List<QueueItem>> GetQueueAsync()
{
    var db = await GetDatabaseAsync();
    return await db.Table<QueueItem>()
        .OrderBy(q => q.Position)
        .ToListAsync();
}

public async Task ClearQueueAsync()
{
    var db = await GetDatabaseAsync();
    await db.DeleteAllAsync<QueueItem>();
}

public async Task<QueueItem?> GetCurrentTrackFromQueueAsync()
{
    var db = await GetDatabaseAsync();
    return await db.Table<QueueItem>()
        .Where(q => q.IsCurrentTrack)
        .FirstOrDefaultAsync();
}

public async Task UpdateCurrentTrackPositionAsync(int queueItemId, int positionMs)
{
    var db = await GetDatabaseAsync();
    var queueItem = await db.Table<QueueItem>()
        .Where(q => q.Id == queueItemId)
        .FirstOrDefaultAsync();
    
    if (queueItem != null)
    {
        queueItem.CurrentPositionMs = positionMs;
        await db.UpdateAsync(queueItem);
    }
}

// ==================== Settings Operations ====================

public async Task<AppSettings> GetSettingsAsync()
{
    var db = await GetDatabaseAsync();
    var settings = await db.Table<AppSettings>()
        .Where(s => s.Id == 1)
        .FirstOrDefaultAsync();
    
    // Return default settings if none exist
    return settings ?? new AppSettings();
}

public async Task SaveSettingsAsync(AppSettings settings)
{
    var db = await GetDatabaseAsync();
    settings.Id = 1;  // Always use ID 1
    settings.LastModified = DateTime.Now;
    
    var existing = await db.Table<AppSettings>()
        .Where(s => s.Id == 1)
        .FirstOrDefaultAsync();
    
    if (existing != null)
        await db.UpdateAsync(settings);
    else
        await db.InsertAsync(settings);
}

// Convenience methods for individual settings
public async Task<T> GetSettingAsync<T>(string key, T defaultValue)
{
    var settings = await GetSettingsAsync();
    var property = typeof(AppSettings).GetProperty(key);
    
    if (property != null)
    {
        var value = property.GetValue(settings);
        return value != null ? (T)value : defaultValue;
    }
    
    return defaultValue;
}

public async Task SetSettingAsync<T>(string key, T value)
{
    var settings = await GetSettingsAsync();
    var property = typeof(AppSettings).GetProperty(key);
    
    if (property != null)
    {
        property.SetValue(settings, value);
        await SaveSettingsAsync(settings);
    }
}
    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database == null)
            await InitializeAsync();
        return _database!;
    }

    // Track operations
    public async Task<int> SaveTrackAsync(Track track)
    {
        var db = await GetDatabaseAsync();
        
        if (track.Id != 0)
            return await db.UpdateAsync(track);
        else
            return await db.InsertAsync(track);
    }

    public async Task<List<Track>> GetAllTracksAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Track>().ToListAsync();
    }

    public async Task<Track?> GetTrackByPathAsync(string filePath)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Track>()
            .Where(t => t.FilePath == filePath)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Track>> GetFavoriteTracksAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Track>()
            .Where(t => t.IsFavorite)
            .OrderBy(t => t.Artist)
            .ThenBy(t => t.Album)
            .ThenBy(t => t.TrackNumber)
            .ToListAsync();
    }

    public async Task UpdatePlayCountAsync(int trackId)
    {
        var db = await GetDatabaseAsync();
        var track = await db.Table<Track>()
            .Where(t => t.Id == trackId)
            .FirstOrDefaultAsync();
        
        if (track != null)
        {
            track.PlayCount++;
            track.LastPlayed = DateTime.Now;
            await db.UpdateAsync(track);
        }
    }

    public async Task<int> DeleteTrackAsync(Track track)
    {
        var db = await GetDatabaseAsync();
        return await db.DeleteAsync(track);
    }

    // Playlist operations
    public async Task<int> CreatePlaylistAsync(Playlist playlist)
    {
        var db = await GetDatabaseAsync();
        playlist.CreatedDate = DateTime.Now;
        playlist.ModifiedDate = DateTime.Now;
        return await db.InsertAsync(playlist);
    }

    public async Task<List<Playlist>> GetAllPlaylistsAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Playlist>()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Playlist?> GetPlaylistAsync(int playlistId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Playlist>()
            .Where(p => p.Id == playlistId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> UpdatePlaylistAsync(Playlist playlist)
    {
        var db = await GetDatabaseAsync();
        playlist.ModifiedDate = DateTime.Now;
        return await db.UpdateAsync(playlist);
    }

    public async Task<int> DeletePlaylistAsync(int playlistId)
    {
        var db = await GetDatabaseAsync();
        
        // Delete playlist tracks first (cascade delete)
        await db.ExecuteAsync("DELETE FROM playlist_tracks WHERE PlaylistId = ?", playlistId);
        
        return await db.ExecuteAsync("DELETE FROM playlists WHERE Id = ?", playlistId);
    }

    // Playlist-Track operations
    public async Task AddTrackToPlaylistAsync(int playlistId, int trackId)
    {
        var db = await GetDatabaseAsync();
        
        // Get current max position
        var maxPosition = await db.Table<PlaylistTrack>()
            .Where(pt => pt.PlaylistId == playlistId)
            .CountAsync();

        var playlistTrack = new PlaylistTrack
        {
            PlaylistId = playlistId,
            TrackId = trackId,
            Position = maxPosition,
            AddedDate = DateTime.Now
        };
        
        await db.InsertAsync(playlistTrack);
        
        // Update playlist track count
        await db.ExecuteAsync(
            "UPDATE playlists SET TrackCount = TrackCount + 1, ModifiedDate = ? WHERE Id = ?",
            DateTime.Now, playlistId);
    }

    public async Task<List<Track>> GetPlaylistTracksAsync(int playlistId)
    {
        var db = await GetDatabaseAsync();
        
        var query = @"
            SELECT t.* FROM tracks t
            INNER JOIN playlist_tracks pt ON t.Id = pt.TrackId
            WHERE pt.PlaylistId = ?
            ORDER BY pt.Position";
        
        return await db.QueryAsync<Track>(query, playlistId);
    }

    public async Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId)
    {
        var db = await GetDatabaseAsync();
        
        await db.ExecuteAsync(
            "DELETE FROM playlist_tracks WHERE PlaylistId = ? AND TrackId = ?",
            playlistId, trackId);
        
        // Update playlist track count
        await db.ExecuteAsync(
            "UPDATE playlists SET TrackCount = TrackCount - 1, ModifiedDate = ? WHERE Id = ?",
            DateTime.Now, playlistId);
    }

    // Play history
    public async Task<int> AddPlayHistoryAsync(PlayHistory history)
    {
        var db = await GetDatabaseAsync();
        history.PlayedAt = DateTime.Now;
        return await db.InsertAsync(history);
    }

    public async Task<List<Track>> GetRecentlyPlayedAsync(int limit = 50)
    {
        var db = await GetDatabaseAsync();
        
        var query = @"
            SELECT DISTINCT t.* FROM tracks t
            INNER JOIN play_history ph ON t.Id = ph.TrackId
            ORDER BY ph.PlayedAt DESC
            LIMIT ?";
        
        return await db.QueryAsync<Track>(query, limit);
    }

    public async Task<Dictionary<int, int>> GetMostPlayedTracksAsync(int limit = 20)
{
    var db = await GetDatabaseAsync();
    
    var tracks = await db.Table<Track>()
        .OrderByDescending(t => t.PlayCount)
        .Take(limit)
        .ToListAsync();
    
    return tracks.ToDictionary(t => t.Id, t => t.PlayCount);
}


    // Album art
    public async Task<int> SaveAlbumArtAsync(AlbumArt albumArt)
    {
        var db = await GetDatabaseAsync();
        albumArt.CachedDate = DateTime.Now;
        
        var existing = await GetAlbumArtAsync(albumArt.AlbumKey);
        if (existing != null)
        {
            albumArt.Id = existing.Id;
            return await db.UpdateAsync(albumArt);
        }
        
        return await db.InsertAsync(albumArt);
    }

    public async Task<AlbumArt?> GetAlbumArtAsync(string albumKey)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<AlbumArt>()
            .Where(a => a.AlbumKey == albumKey)
            .FirstOrDefaultAsync();
    }

    // Utility
    public async Task<int> GetLibrarySizeAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Track>().CountAsync();
    }

    public async Task<Track?> GetTrackByIdAsync(int trackId)
{
    var db = await GetDatabaseAsync();
    return await db.Table<Track>()
        .Where(t => t.Id == trackId)
        .FirstOrDefaultAsync();
}


    public async Task ClearAllDataAsync()
    {
        var db = await GetDatabaseAsync();
        await db.DeleteAllAsync<PlayHistory>();
        await db.DeleteAllAsync<PlaylistTrack>();
        await db.DeleteAllAsync<Playlist>();
        await db.DeleteAllAsync<AlbumArt>();
        await db.DeleteAllAsync<Track>();
    }
}
