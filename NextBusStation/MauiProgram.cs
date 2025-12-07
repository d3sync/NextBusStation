using MauiIcons.FontAwesome;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using NextBusStation.Services;
using NextBusStation.ViewModels;
using NextBusStation.Views;
using Plugin.LocalNotification;

namespace NextBusStation
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseLocalNotification()
                .UseFontAwesomeMauiIcons()
                .UseMaterialMauiIcons();

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<LocationService>();
            builder.Services.AddSingleton<OasaApiService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<BusMonitoringService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddHttpClient();
            
            builder.Services.AddTransient<MapViewModel>();
            builder.Services.AddTransient<StopDetailsViewModel>();
            builder.Services.AddTransient<NotificationSchedulesViewModel>();
            builder.Services.AddTransient<EditScheduleViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            
            builder.Services.AddTransient<MapPage>();
            builder.Services.AddTransient<StopDetailsPage>();
            builder.Services.AddTransient<NotificationSchedulesPage>();
            builder.Services.AddTransient<EditSchedulePage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
