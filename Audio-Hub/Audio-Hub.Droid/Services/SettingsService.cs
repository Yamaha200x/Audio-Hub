using Audio_Hub.Droid.Data;
using Audio_Hub.Droid.Data.Models;

namespace Audio_Hub.Droid.Services;

/// <summary>
/// Settings persistence and management.
/// Caches settings in memory for fast access.
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly IDatabaseService _databaseService;
    private AppSettings? _cachedSettings;

    public SettingsService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    private async Task<AppSettings> GetCachedSettingsAsync()
    {
        if (_cachedSettings == null)
        {
            _cachedSettings = await _databaseService.GetSettingsAsync();
        }
        return _cachedSettings;
    }

    public async Task<AppSettings> GetAllSettingsAsync()
    {
        return await GetCachedSettingsAsync();
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        await _databaseService.SaveSettingsAsync(settings);
        _cachedSettings = settings;  // Update cache
    }

    // Volume
    public async Task<float> GetVolumeAsync()
    {
        var settings = await GetCachedSettingsAsync();
        return settings.Volume;
    }

    public async Task SetVolumeAsync(float volume)
    {
        var settings = await GetCachedSettingsAsync();
        settings.Volume = Math.Clamp(volume, 0.0f, 1.0f);
        await SaveSettingsAsync(settings);
    }

    // Theme
    public async Task<string> GetThemeAsync()
    {
        var settings = await GetCachedSettingsAsync();
        return settings.Theme;
    }

    public async Task SetThemeAsync(string theme)
    {
        var settings = await GetCachedSettingsAsync();
        settings.Theme = theme;
        await SaveSettingsAsync(settings);
    }

    // Shuffle
    public async Task<bool> GetShuffleEnabledAsync()
    {
        var settings = await GetCachedSettingsAsync();
        return settings.ShuffleEnabled;
    }

    public async Task SetShuffleEnabledAsync(bool enabled)
    {
        var settings = await GetCachedSettingsAsync();
        settings.ShuffleEnabled = enabled;
        await SaveSettingsAsync(settings);
    }

    // Repeat Mode
    public async Task<string> GetRepeatModeAsync()
    {
        var settings = await GetCachedSettingsAsync();
        return settings.RepeatMode;
    }

    public async Task SetRepeatModeAsync(string mode)
    {
        var settings = await GetCachedSettingsAsync();
        settings.RepeatMode = mode;
        await SaveSettingsAsync(settings);
    }

    // Sort By
    public async Task<string> GetSortByAsync()
    {
        var settings = await GetCachedSettingsAsync();
        return settings.SortBy;
    }

    public async Task SetSortByAsync(string sortBy)
    {
        var settings = await GetCachedSettingsAsync();
        settings.SortBy = sortBy;
        await SaveSettingsAsync(settings);
    }

    // Reset
    public async Task ResetToDefaultsAsync()
    {
        await SaveSettingsAsync(new AppSettings());
    }
}
