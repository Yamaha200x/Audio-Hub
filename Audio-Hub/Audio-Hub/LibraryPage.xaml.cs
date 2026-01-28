using Audio_Hub.Droid.Services;

namespace Audio_Hub;

public partial class LibraryPage : ContentPage
{
    private readonly IMusicLibraryService _libraryService;
    private readonly IAudioPlayerService _playerService;

    private string _currentFilter = "Folders";

    public LibraryPage()
    {
        InitializeComponent();
        ShowView(_currentFilter);
    }

    // Constructor mit Dependency Injection
    public LibraryPage(IMusicLibraryService libraryService, IAudioPlayerService playerService)
    {
        InitializeComponent();
        _libraryService = libraryService;
        _playerService = playerService;

        ShowView(_currentFilter);
        LoadData();
    }

    private async void LoadData()
    {
        try
        {
            var albums = await _libraryService.GetAllAlbumsAsync();
            var artists = await _libraryService.GetAllArtistsAsync();
            var songs = await _libraryService.GetAllSongsAsync();

            // Hier später die Daten an die Views binden
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fehler", $"Fehler beim Laden: {ex.Message}", "OK");
        }
    }

    // Event Handler für Filter-Chip Änderung
    private void OnFilterChanged(object sender, TappedEventArgs e)
    {
        if (e.Parameter is string filterName)
        {
            _currentFilter = filterName;

            ResetAllChips();
            HighlightActiveChip(filterName);
            ShowView(filterName);
        }
    }

    private void ResetAllChips()
    {
        var inactiveColor = Color.FromArgb("#1A1A1A");
        var transparent = Colors.Transparent;

        FoldersChip.BackgroundColor = inactiveColor;
        FoldersChip.BorderColor = transparent;

        PlaylistsChip.BackgroundColor = inactiveColor;
        PlaylistsChip.BorderColor = transparent;

        ArtistsChip.BackgroundColor = inactiveColor;
        ArtistsChip.BorderColor = transparent;

        AlbumsChip.BackgroundColor = inactiveColor;
        AlbumsChip.BorderColor = transparent;

        PodcastsChip.BackgroundColor = inactiveColor;
        PodcastsChip.BorderColor = transparent;
    }

    private void HighlightActiveChip(string filterName)
    {
        var activeColor = Color.FromArgb("#1E5D60");
        var borderColor = Color.FromArgb("#00C2CB");

        switch (filterName)
        {
            case "Folders":
                FoldersChip.BackgroundColor = activeColor;
                FoldersChip.BorderColor = borderColor;
                break;
            case "Playlists":
                PlaylistsChip.BackgroundColor = activeColor;
                PlaylistsChip.BorderColor = borderColor;
                break;
            case "Artists":
                ArtistsChip.BackgroundColor = activeColor;
                ArtistsChip.BorderColor = borderColor;
                break;
            case "Albums":
                AlbumsChip.BackgroundColor = activeColor;
                AlbumsChip.BorderColor = borderColor;
                break;
            case "Podcasts":
                PodcastsChip.BackgroundColor = activeColor;
                PodcastsChip.BorderColor = borderColor;
                break;
        }
    }

    private void ShowView(string filterName)
    {
        // Alle Views verstecken
        FoldersView.IsVisible = false;
        PlaylistsView.IsVisible = false;
        ArtistsView.IsVisible = false;
        AlbumsView.IsVisible = false;
        PodcastsView.IsVisible = false;

        switch (filterName)
        {
            case "Folders":
                FoldersView.IsVisible = true;
                break;
            case "Playlists":
                PlaylistsView.IsVisible = true;
                break;
            case "Artists":
                ArtistsView.IsVisible = true;
                break;
            case "Albums":
                AlbumsView.IsVisible = true;
                break;
            case "Podcasts":
                PodcastsView.IsVisible = true;
                break;
        }

        // Scroll nach oben
        ContentScrollView.ScrollToAsync(0, 0, false);
    }

    // Event Handler für Suchbutton
    private async void OnSearchClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Suche", "Suchfunktion wird geöffnet", "OK");
    }

    // Event Handler für "Neue Playlist" Button
    private async void OnNewPlaylistClicked(object sender, TappedEventArgs e)
    {
        string playlistName = await DisplayPromptAsync(
            "Neue Playlist",
            "Gib einen Namen für die Playlist ein:",
            placeholder: "Meine Playlist");

        if (!string.IsNullOrEmpty(playlistName))
        {
            // TODO: Playlist in Datenbank erstellen
            await DisplayAlert("Erfolg", $"Playlist '{playlistName}' wurde erstellt", "OK");
        }
    }

    // Event Handler für "Meine Favoriten" Button
    private async void OnFavoritesClicked(object sender, TappedEventArgs e)
    {
        // TODO: Navigation zur Favoriten-Detailseite
        await DisplayAlert("Favoriten", "Deine Favoriten werden geladen...", "OK");
    }

    // Event Handler für Item-Klick (Folder, Playlist, Artist, Album, Podcast)
    private async void OnItemTapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        if (frame?.BindingContext != null)
        {
            // TODO: Navigation zur entsprechenden Detail-Seite
        }
    }

    // Event Handler für More-Button (drei Punkte)
    private async void OnMoreOptionsClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button == null) return;

        string action = await DisplayActionSheet(
            "Optionen",
            "Abbrechen",
            null,
            "Abspielen",
            "Als Nächstes abspielen",
            "Zur Warteschlange hinzufügen",
            "Zur Playlist hinzufügen",
            "Zu Favoriten hinzufügen",
            "Teilen",
            "Details anzeigen");

        switch (action)
        {
            case "Abspielen":
                // TODO: Sofort abspielen
                break;
            case "Als Nächstes abspielen":
                // TODO: Zur Queue hinzufügen (nächster Song)
                break;
            case "Zur Warteschlange hinzufügen":
                // TODO: Zur Queue hinzufügen (am Ende)
                break;
            case "Zur Playlist hinzufügen":
                // TODO: Playlist-Auswahl Dialog
                break;
            case "Zu Favoriten hinzufügen":
                // TODO: Zu Favoriten hinzufügen
                break;
            case "Teilen":
                // TODO: Share-Dialog öffnen
                break;
            case "Details anzeigen":
                // TODO: Detail-Seite öffnen
                break;
        }
    }
}