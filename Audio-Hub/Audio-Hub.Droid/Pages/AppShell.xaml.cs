using Audio_Hub.Droid.Pages;

namespace Audio_Hub.Droid.Pages;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(LibraryPage), typeof(LibraryPage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
    }
}
