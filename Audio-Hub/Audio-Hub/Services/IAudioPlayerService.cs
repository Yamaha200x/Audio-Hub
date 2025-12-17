namespace Audio_Hub.Droid.Services;  // Use Audio_Hub with underscore

public interface IAudioPlayerService
{
    Task PlayAsync(string filePath);
    Task PauseAsync();
    Task StopAsync();
    Task<bool> IsPlayingAsync();
    event EventHandler<string> PlaybackStateChanged;
}
