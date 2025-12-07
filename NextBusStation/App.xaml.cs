using Microsoft.Extensions.DependencyInjection;
using NextBusStation.Services;

namespace NextBusStation
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            
            // Initialize monitoring on startup
            window.Created += async (s, e) =>
            {
                await InitializeMonitoringAsync();
            };
            
            return window;
        }

        private async Task InitializeMonitoringAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🚀 Initializing bus monitoring service...");
                
                var serviceProvider = this.Handler?.MauiContext?.Services;
                if (serviceProvider == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ ServiceProvider is null");
                    return;
                }

                var databaseService = serviceProvider.GetService<DatabaseService>();
                var monitoringService = serviceProvider.GetService<BusMonitoringService>();
                var settingsService = serviceProvider.GetService<SettingsService>();

                if (databaseService == null || monitoringService == null || settingsService == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Required services not available");
                    return;
                }

                await databaseService.InitializeAsync();
                await settingsService.InitializeDefaultSettingsAsync();
                
                var autoStart = settingsService.GetAutoStartMonitoring();
                System.Diagnostics.Debug.WriteLine($"📋 Auto-start monitoring setting: {autoStart}");

                if (!autoStart)
                {
                    System.Diagnostics.Debug.WriteLine("⏸️ Auto-start disabled in settings");
                    return;
                }

                var activeSchedules = await databaseService.GetActiveSchedulesAsync();
                System.Diagnostics.Debug.WriteLine($"📋 Found {activeSchedules.Count} active schedule(s)");

                if (activeSchedules.Any())
                {
                    System.Diagnostics.Debug.WriteLine("✅ Starting monitoring service...");
                    await monitoringService.StartMonitoringAsync();
                    System.Diagnostics.Debug.WriteLine("✅ Monitoring service started successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ℹ️ No active schedules - monitoring not started");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error initializing monitoring: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            }
        }
    }
}