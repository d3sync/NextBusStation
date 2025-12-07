using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace NextBusStation.Services;

public class NotificationService
{
    public async Task<bool> RequestPermissionAsync()
    {
        System.Diagnostics.Debug.WriteLine("?? [Permission] Checking notification permissions...");
        
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            System.Diagnostics.Debug.WriteLine("?? [Permission] Requesting notification permission...");
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
        
        var hasPermission = await LocalNotificationCenter.Current.AreNotificationsEnabled();
        System.Diagnostics.Debug.WriteLine($"?? [Permission] Notification permission: {(hasPermission ? "GRANTED" : "DENIED")}");
        
        return hasPermission;
    }
    
    public async Task ShowBusArrivalNotificationAsync(string stopName, List<(string lineId, string destination, int minutes)> arrivals)
    {
        if (arrivals == null || !arrivals.Any())
        {
            System.Diagnostics.Debug.WriteLine("?? [Notification] No arrivals to notify");
            return;
        }
        
        System.Diagnostics.Debug.WriteLine($"?? [Notification] Creating notification for {stopName} with {arrivals.Count} arrival(s)");
        
        var title = arrivals.Count == 1 
            ? $"Bus Arriving - {stopName}"
            : $"{arrivals.Count} Buses Arriving - {stopName}";
        
        var messageLines = arrivals.Select(a => 
            $"?? Line {a.lineId} ? {a.destination} in {a.minutes} min");
        
        var message = string.Join("\n", messageLines);
        
        System.Diagnostics.Debug.WriteLine($"?? [Notification] Title: {title}");
        System.Diagnostics.Debug.WriteLine($"?? [Notification] Message:\n{message}");
        
        var notification = new NotificationRequest
        {
            NotificationId = stopName.GetHashCode(),
            Title = title,
            Description = message,
            BadgeNumber = arrivals.Count,
            CategoryType = NotificationCategoryType.Status,
            Android = new AndroidOptions
            {
                Priority = AndroidPriority.High,
                ChannelId = "bus_arrivals",
                AutoCancel = true,
                Ongoing = false,
                TimeoutAfter = TimeSpan.FromMinutes(15)
            }
        };
        
        System.Diagnostics.Debug.WriteLine($"?? [Notification] Sending notification with ID: {notification.NotificationId}");
        
        await LocalNotificationCenter.Current.Show(notification);
        
        System.Diagnostics.Debug.WriteLine($"? [Notification] Notification sent successfully");
    }
    
    public async Task ShowTestNotificationAsync()
    {
        System.Diagnostics.Debug.WriteLine("?? [Test] Sending test notification...");
        
        var notification = new NotificationRequest
        {
            NotificationId = 999999,
            Title = "Test Notification",
            Description = "This is a test notification from NextBusStation app",
            BadgeNumber = 1,
            CategoryType = NotificationCategoryType.Status,
            Android = new AndroidOptions
            {
                Priority = AndroidPriority.High,
                ChannelId = "bus_arrivals",
                AutoCancel = true
            }
        };
        
        await LocalNotificationCenter.Current.Show(notification);
        System.Diagnostics.Debug.WriteLine("? [Test] Test notification sent");
    }
    
    public void ClearNotifications()
    {
        LocalNotificationCenter.Current.Clear();
    }
    
    public void CancelNotification(int notificationId)
    {
        LocalNotificationCenter.Current.Cancel(notificationId);
    }
}
