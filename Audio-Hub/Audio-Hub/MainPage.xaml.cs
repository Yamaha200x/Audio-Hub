namespace Audio_Hub;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Startet die Initialisierung, sobald die Seite geladen ist
        StartAppSequence();
    }

    private async void StartAppSequence()
    {
        // Einmaliges Laden am Anfang (z.B. 2 Sekunden)
        await Task.Delay(2000);

        // Jetzt wechseln wir DAUERHAFT zur AppShell
        // Dadurch wird die TabBar unten sichtbar und der Ladebildschirm gelöscht
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.MainPage = new AppShell();
        });
    }
}