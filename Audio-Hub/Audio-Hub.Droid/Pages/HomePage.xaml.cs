using Audio_Hub.Droid.Services; 

namespace Audio_Hub.Droid.Pages;

public partial class HomePage : ContentPage
{
    private readonly IMusicLibraryService _libraryService;
    private readonly IAudioPlayerService _playerService;

    public HomePage()
    {
        InitializeComponent();
    }

    // Constructor mit Dependency Injection
    public HomePage(IMusicLibraryService libraryService, IAudioPlayerService playerService)
    {
        InitializeComponent();
        _libraryService = libraryService;
        _playerService = playerService;

        LoadRecentlyPlayed();
    }

    private async void LoadRecentlyPlayed()
    {
        // Hier sp�ter die zuletzt geh�rten Songs/Alben laden
    }

    // Event Handler f�r Album/Playlist Klick
    private async void OnAlbumTapped(object sender, EventArgs e)
    {
        var frame = sender as Border;
        if (frame?.BindingContext is AudioMetadata audio)
        {
            await _playerService.PlayAsync(audio.FilePath);
        }
    }

    // Event Handler f�r Mix Klick
    private async void OnMixTapped(object sender, EventArgs e)
    {
        // Navigation zur Mix-Detail Seite
    }
}