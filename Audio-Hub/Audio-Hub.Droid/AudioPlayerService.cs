using global::Android.Content;
using global::Android.OS;
using Audio_Hub.Droid.Services;
using Microsoft.Maui.ApplicationModel;

namespace Audio_Hub.Droid.Platforms.Android;

/// <summary>
/// Android audio player implementation using foreground service.
/// Communicates with MusicPlaybackService via Intents.
/// </summary>
public class AudioPlayerService : IAudioPlayerService
{
    public event EventHandler<string>? PlaybackStateChanged;
    public event EventHandler<int>? PositionChanged;
    public event EventHandler<AudioMetadata>? CurrentTrackChanged;

    public Task PlayAsync(string filePath)
    {
        var context = Platform.CurrentActivity;
        if (context == null) return Task.CompletedTask;
        
        var intent = new Intent(context, typeof(MusicPlaybackService));
        intent.SetAction("PLAY");
        intent.PutExtra("audioPath", filePath);
        
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            context.StartForegroundService(intent);
        else
            context.StartService(intent);
        
        PlaybackStateChanged?.Invoke(this, "Playing");
        return Task.CompletedTask;
    }

    public Task PauseAsync()
    {
        var context = Platform.CurrentActivity;
        if (context == null) return Task.CompletedTask;
        
        var intent = new Intent(context, typeof(MusicPlaybackService));
        intent.SetAction("PAUSE");
        context.StartService(intent);
        
        PlaybackStateChanged?.Invoke(this, "Paused");
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        var context = Platform.CurrentActivity;
        if (context == null) return Task.CompletedTask;
        
        var intent = new Intent(context, typeof(MusicPlaybackService));
        intent.SetAction("STOP");
        context.StartService(intent);
        
        PlaybackStateChanged?.Invoke(this, "Stopped");
        return Task.CompletedTask;
    }

    public Task ResumeAsync()
    {
        return PlayAsync(string.Empty); // Service will resume current track
    }

    public Task SeekToAsync(int positionMs)
    {
        var context = Platform.CurrentActivity;
        if (context == null) return Task.CompletedTask;
        
        var intent = new Intent(context, typeof(MusicPlaybackService));
        intent.SetAction("SEEK");
        intent.PutExtra("position", positionMs);
        context.StartService(intent);
        
        return Task.CompletedTask;
    }

    public Task<bool> IsPlayingAsync()
    {
        // TODO: Implement state tracking via broadcast receiver
        return Task.FromResult(false);
    }

    public Task<int> GetCurrentPositionAsync()
    {
        // TODO: Implement via broadcast receiver from service
        return Task.FromResult(0);
    }

    public Task<int> GetDurationAsync()
    {
        // TODO: Implement via broadcast receiver from service
        return Task.FromResult(0);
    }

    public Task SetVolumeAsync(float volume)
    {
        var context = Platform.CurrentActivity;
        if (context == null) return Task.CompletedTask;
        
        var intent = new Intent(context, typeof(MusicPlaybackService));
        intent.SetAction("SET_VOLUME");
        intent.PutExtra("volume", volume);
        context.StartService(intent);
        
        return Task.CompletedTask;
    }
}
