namespace Audio_Hub;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Hier starten wir mit dem Ladebildschirm
        MainPage = new MainPage();
    }
}