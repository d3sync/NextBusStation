using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace NextBusStation.Services;

public class NotificationService
{
    public async Task<bool> RequestPermissionAsync()
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
        
        return await LocalNotificationCenter.Current.AreNotificationsEnabled();
    }
    
    public async Task ShowBusArrivalNotificationAsync(string stopName, List<(string lineId, string destination, int minutes)> arrivals)
    {
        if (arrivals == null || !arrivals.Any())
            return;
        
        var title = $"Bus Arrivals - {stopName}";
        var message = string.Join("\n", arrivals.Select(a => 
            $"Line {a.lineId} to {a.destination} in {a.minutes} min"));
        
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
        
        await LocalNotificationCenter.Current.Show(notification);
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
