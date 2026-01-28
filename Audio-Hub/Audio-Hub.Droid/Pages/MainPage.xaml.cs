using Audio_Hub.Droid.Pages;
namespace Audio_Hub;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        StartAppSequence();
    }

    private async void StartAppSequence()
    {
        await Task.Delay(2000);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.MainPage = new AppShell();
        });
    }
}