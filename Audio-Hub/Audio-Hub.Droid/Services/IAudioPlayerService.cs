namespace Audio_Hub.Droid.Services;

/// <summary>
/// Core audio playback service.
/// Frontend: Subscribe to events to sync UI with playback state.
/// </summary>
public interface IAudioPlayerService
{
    // Playback controls
    Task PlayAsync(string filePath);  // Start playing from beginning
    Task PauseAsync();                // Pause at current position
    Task StopAsync();                 // Stop and reset to 0:00
    Task ResumeAsync();               // Resume from paused position
    Task SeekToAsync(int positionMs); // Jump to position (milliseconds)

    // State queries
    Task<bool> IsPlayingAsync();        // True if playing (not paused/stopped)
    Task<int> GetCurrentPositionAsync(); // Current position in milliseconds
    Task<int> GetDurationAsync();        // Total duration in milliseconds
    Task SetVolumeAsync(float volume);   // Volume 0.0 to 1.0

    // Events - subscribe to keep UI synchronized
    event EventHandler<string> PlaybackStateChanged;      // "Playing", "Paused", "Stopped"
    event EventHandler<int> PositionChanged;              // Fires every second with position (ms)
    event EventHandler<AudioMetadata>? CurrentTrackChanged; // Fires when new track starts
}
