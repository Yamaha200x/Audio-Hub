using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;

namespace Audio_Hub.Droid.Platforms.Android;

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
        var audioPath = intent?.GetStringExtra("audioPath");

        switch (action)
        {
            case "PLAY":
                if (!string.IsNullOrEmpty(audioPath))
                    PlayAudio(audioPath);
                break;
            case "PAUSE":
                _mediaPlayer?.Pause();
                break;
            case "STOP":
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
                break;
        }

        return StartCommandResult.Sticky;
    }

    private void PlayAudio(string audioPath)
    {
        try
        {
            _mediaPlayer?.Reset();
            _mediaPlayer?.SetDataSource(audioPath);
            _mediaPlayer?.Prepare();
            _mediaPlayer?.Start();

            var notification = new NotificationCompat.Builder(this, ChannelId)
                .SetContentTitle("Playing Music")
                .SetContentText(audioPath)
                .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
                .SetOngoing(true)
                .Build();

            if (notification != null)
                StartForeground(NotificationId, notification);
        }
        catch (Exception)
        {
            // Log error or handle appropriately
        }
    }

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnDestroy()
    {
        _mediaPlayer?.Release();
        _mediaPlayer = null;
        base.OnDestroy();
    }
}
