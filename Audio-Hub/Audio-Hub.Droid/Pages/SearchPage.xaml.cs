using Audio_Hub.Droid.Services;

namespace Audio_Hub.Droid.Pages;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // TODO: Implement search filtering
        var searchQuery = e.NewTextValue;
        
        // Filter your music library based on searchQuery
        System.Diagnostics.Debug.WriteLine($"Search query: {searchQuery}");
    }

    private void OnItemTapped(object sender, EventArgs e)
    {
        // TODO: Handle item tap (navigate to album/artist/playlist)
        System.Diagnostics.Debug.WriteLine("Item tapped");
    }
}
