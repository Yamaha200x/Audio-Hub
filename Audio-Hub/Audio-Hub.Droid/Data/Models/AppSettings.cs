using SQLite;

namespace Audio_Hub.Droid.Data.Models;

/// <summary>
/// App preferences and settings stored in database.
/// Frontend: Load on startup, update when user changes settings.
/// </summary>
[Table("app_settings")]
public class AppSettings
{
    [PrimaryKey]
    public int Id { get; set; } = 1;  // Always use ID=1, single row table

    // Playback settings
    public float Volume { get; set; } = 1.0f;  // 0.0 to 1.0
    public string RepeatMode { get; set; } = "None";  // None, One, All
    public bool ShuffleEnabled { get; set; } = false;
    public bool EqualizerEnabled { get; set; } = false;
    
    // Library settings
    public DateTime? LastLibraryScan { get; set; }
    public bool AutoScanOnStartup { get; set; } = false;
    public string SortBy { get; set; } = "Artist";  // Artist, Album, Title, DateAdded
    public bool SortAscending { get; set; } = true;
    
    // UI settings
    public string Theme { get; set; } = "System";  // Light, Dark, System
    public bool ShowAlbumArt { get; set; } = true;
    public bool ShowLyrics { get; set; } = true;
    
    // Audio quality settings
    public string AudioQuality { get; set; } = "High";  // Low, Medium, High
    public bool GaplessPlayback { get; set; } = false;
    public int CrossfadeDurationMs { get; set; } = 0;  // 0 = disabled
    
    // Storage settings
    public long MaxCacheSize { get; set; } = 500_000_000;  // 500MB in bytes
    public bool DownloadOverWifiOnly { get; set; } = true;
    
    // Notification settings
    public bool ShowNotificationControls { get; set; } = true;
    public bool VibrateOnTrackChange { get; set; } = false;
    
    public DateTime LastModified { get; set; }
}
