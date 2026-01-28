using Microsoft.Extensions.Logging;
using Audio_Hub.Droid.Services;  // Changed from Audio_Hub to Audio_Hub
using Microsoft.Extensions.DependencyInjection;

namespace Audio_Hub.Droid; 
public static class MauiProgramExtensions
{
	public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
	{
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("CenturyGothic.ttf", "CenturyGothic");
			});

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder;
    }
}
