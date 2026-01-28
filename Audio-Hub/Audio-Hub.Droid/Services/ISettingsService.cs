using Audio_Hub.Droid.Data.Models;
namespace Audio_Hub.Droid.Services;

/// <summary>
/// Manages app settings and preferences.
/// Frontend: Use to save/load user preferences.
/// </summary>
public interface ISettingsService
{
    // General settings
    Task<AppSettings> GetAllSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
    
    // Quick access to common settings
    Task<float> GetVolumeAsync();
    Task SetVolumeAsync(float volume);
    
    Task<string> GetThemeAsync();
    Task SetThemeAsync(string theme);
    
    Task<bool> GetShuffleEnabledAsync();
    Task SetShuffleEnabledAsync(bool enabled);
    
    Task<string> GetRepeatModeAsync();
    Task SetRepeatModeAsync(string mode);
    
    Task<string> GetSortByAsync();
    Task SetSortByAsync(string sortBy);
    
    // Reset to defaults
    Task ResetToDefaultsAsync();
}
