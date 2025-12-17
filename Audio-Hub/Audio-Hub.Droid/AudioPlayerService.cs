using Android.Content;
using Audio_Hub.Droid.Services;  // Changed from Audio_Hub to Audio_Hub
using Microsoft.Maui.ApplicationModel;
using Android.OS;

namespace Audio_Hub.Droid.Platforms.Android;  // Use Audio_Hub (with underscore)

public class AudioPlayerService : IAudioPlayerService
{
    public event EventHandler<string>? PlaybackStateChanged;

    public Task PlayAsync(string filePath)
    {
        var context = Platform.CurrentActivity;
        if (context == null) return Task.CompletedTask;
        
        var intent = new Intent(context, typeof(MusicPlaybackService));
        intent.SetAction("PLAY");
        intent.PutExtra("audioPath", filePath);
        
        // Check Android version for StartForegroundService
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            context.StartForegroundService(intent);
        }
        else
        {
            context.StartService(intent);
        }
        
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

    public Task<bool> IsPlayingAsync()
    {
        // TODO: Implement state tracking
        return Task.FromResult(false);
    }
}
