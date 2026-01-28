using Audio_Hub.Droid.Services;
using Audio_Hub.Droid.Platforms.Android;
using Audio_Hub.Droid.Data; // Add this for IDatabaseService

namespace Audio_Hub.Droid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseSharedMauiApp();
            
            // Backend Services - available via dependency injection
            builder.Services.AddSingleton<IAudioPlayerService, AudioPlayerService>();
            builder.Services.AddSingleton<IAudioMetadataService, AudioMetadataService>();
            builder.Services.AddSingleton<IMusicLibraryService, MusicLibraryService>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<ILyricsService, LyricsService>();
            
            // Database (single registration only)
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            
            // Settings and Queue
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IQueueService, QueueService>();
            
            // Pages
            builder.Services.AddTransient<MainPage>();

            var app = builder.Build();
            
            // Initialize database after build (fire and forget - won't block app startup)
            _ = Task.Run(async () =>
            {
                var dbService = app.Services.GetRequiredService<IDatabaseService>();
                await dbService.InitializeAsync();
                
                // Restore queue after database is ready
                var queueService = app.Services.GetRequiredService<IQueueService>();
                await queueService.RestoreQueueStateAsync();
            });

            return app;
        }
    }
}
