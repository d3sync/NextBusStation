using Android.App;
using Android.Runtime;

namespace NextBusStation
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            try
            {
                CreateNotificationChannel();
                return MauiProgram.CreateMauiApp();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating MauiApp: {ex}");
                throw;
            }
        }

        private void CreateNotificationChannel()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    "bus_arrivals",
                    "Bus Arrivals",
                    NotificationImportance.High)
                {
                    Description = "Notifications for upcoming bus arrivals"
                };
                
                channel.EnableVibration(true);
                channel.EnableLights(true);
                
                var notificationManager = GetSystemService(NotificationService) as NotificationManager;
                notificationManager?.CreateNotificationChannel(channel);
                
                System.Diagnostics.Debug.WriteLine("📢 Notification channel 'bus_arrivals' created");
            }
        }
    }
}
