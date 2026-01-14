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
            
            // Backend Services - available via dependency injection
            // Frontend: Add these as constructor parameters in your pages/ViewModels
            builder.Services.AddSingleton<IAudioPlayerService, AudioPlayerService>();
            builder.Services.AddSingleton<IAudioMetadataService, AudioMetadataService>();
            builder.Services.AddSingleton<IMusicLibraryService, MusicLibraryService>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<ILyricsService, LyricsService>();
            
            // Pages
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
