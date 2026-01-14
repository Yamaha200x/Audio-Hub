using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;

namespace Audio_Hub.Droid.Platforms.Android;

/// <summary>
/// Background service for audio playback with notification controls.
/// Keeps audio playing when app is backgrounded.
/// </summary>
[Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeMediaPlayback)]
public class MusicPlaybackService : Service
{
    private MediaPlayer? _mediaPlayer;
    private const int NotificationId = 1000;
    public const string ChannelId = "music_playback_channel";

    public override void OnCreate()
    {
        base.OnCreate();
        _mediaPlayer = new MediaPlayer();
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        var action = intent?.Action;

        switch (action)
        {
            case "PLAY":
                var audioPath = intent?.GetStringExtra("audioPath");
                if (!string.IsNullOrEmpty(audioPath))
                    PlayAudio(audioPath);
                break;
                
            case "PAUSE":
                _mediaPlayer?.Pause();
                break;
                
            case "STOP":
                StopPlayback();
                break;
                
            case "SEEK":
                var position = intent?.GetIntExtra("position", 0) ?? 0;
                _mediaPlayer?.SeekTo(position);
                break;
                
            case "SET_VOLUME":
                var volume = intent?.GetFloatExtra("volume", 1.0f) ?? 1.0f;
                _mediaPlayer?.SetVolume(volume, volume);
                break;
        }

        return StartCommandResult.Sticky;
    }

    private void PlayAudio(string audioPath)
    {
        try
        {
            global::Android.Util.Log.Debug("MusicPlayback", $"Playing: {audioPath}");
            
            if (!System.IO.File.Exists(audioPath))
            {
                global::Android.Util.Log.Error("MusicPlayback", $"File not found: {audioPath}");
                return;
            }

            _mediaPlayer?.Reset();
            _mediaPlayer?.SetDataSource(audioPath);
            _mediaPlayer?.Prepare();
            _mediaPlayer?.Start();

            var notification = new NotificationCompat.Builder(this, ChannelId)
                .SetContentTitle("Playing Music")
                .SetContentText(System.IO.Path.GetFileName(audioPath))
                .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
                .SetOngoing(true)
                .Build();

            if (notification != null)
                StartForeground(NotificationId, notification);
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("MusicPlayback", $"Error: {ex.Message}");
        }
    }

    private void StopPlayback()
    {
        _mediaPlayer?.Stop();
        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
        {
#pragma warning disable CA1416
            StopForeground(StopForegroundFlags.Remove);
#pragma warning restore CA1416
        }
        else
        {
#pragma warning disable CA1422
            StopForeground(true);
#pragma warning restore CA1422
        }
        StopSelf();
    }

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnDestroy()
    {
        _mediaPlayer?.Release();
        _mediaPlayer = null;
        base.OnDestroy();
    }
}
