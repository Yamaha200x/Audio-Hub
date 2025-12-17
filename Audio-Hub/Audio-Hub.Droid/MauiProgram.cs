using Audio_Hub.Droid.Services;
using Audio_Hub.Droid.Platforms.Android;

namespace Audio_Hub.Droid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseSharedMauiApp();
            
            builder.Services.AddSingleton<IAudioPlayerService, AudioPlayerService>();
                        builder.Services.AddSingleton<MainPage>();


            return builder.Build();
        }
    }
}
