using System.Timers;
using NextBusStation.Models;
using Timer = System.Timers.Timer;

namespace NextBusStation.Services;

public class BusMonitoringService
{
    private readonly OasaApiService _oasaService;
    private readonly DatabaseService _databaseService;
    private readonly LocationService _locationService;
    private readonly NotificationService _notificationService;
    private readonly SettingsService _settingsService;
    private Timer? _monitoringTimer;
    private bool _isMonitoring;
    
    public BusMonitoringService(
        OasaApiService oasaService,
        DatabaseService databaseService,
        LocationService locationService,
        NotificationService notificationService,
        SettingsService settingsService)
    {
        _oasaService = oasaService;
        _databaseService = databaseService;
        _locationService = locationService;
        _notificationService = notificationService;
        _settingsService = settingsService;
    }
    
    public async Task StartMonitoringAsync()
    {
        if (_isMonitoring)
            return;
        
        var hasPermission = await _notificationService.RequestPermissionAsync();
        if (!hasPermission)
        {
            System.Diagnostics.Debug.WriteLine("Notification permission denied");
            return;
        }
        
        var hasLocationPermission = await _locationService.CheckAndRequestLocationPermissionAsync();
        if (!hasLocationPermission)
        {
            System.Diagnostics.Debug.WriteLine("Location permission denied");
            return;
        }
        
        _isMonitoring = true;
        
        _monitoringTimer = new Timer(30000);
        _monitoringTimer.Elapsed += OnMonitoringTick;
        _monitoringTimer.AutoReset = true;
        _monitoringTimer.Start();
        
        await CheckSchedulesAsync();
        
        System.Diagnostics.Debug.WriteLine("Bus monitoring started");
    }
    
    public void StopMonitoring()
    {
        _isMonitoring = false;
        _monitoringTimer?.Stop();
        _monitoringTimer?.Dispose();
        _monitoringTimer = null;
        
        System.Diagnostics.Debug.WriteLine("Bus monitoring stopped");
    }
    
    private async void OnMonitoringTick(object? sender, ElapsedEventArgs e)
    {
        await CheckSchedulesAsync();
    }
    
    private async Task CheckSchedulesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("?? [Monitoring] Checking schedules...");
            
            var activeSchedules = await _databaseService.GetActiveSchedulesAsync();
            
            if (!activeSchedules.Any())
            {
                System.Diagnostics.Debug.WriteLine("?? [Monitoring] No active schedules");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"? [Monitoring] Found {activeSchedules.Count} active schedule(s)");
            foreach (var sched in activeSchedules)
            {
                System.Diagnostics.Debug.WriteLine($"   ?? {sched.StopName} ({sched.StopCode}) - Active: {sched.IsActiveNow}");
            }
            
            var currentLocation = await _locationService.GetCurrentLocationAsync();
            
            if (currentLocation == null)
            {
                System.Diagnostics.Debug.WriteLine("? [Monitoring] Could not get current location");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"?? [Monitoring] Current location: {currentLocation.Latitude:F6}, {currentLocation.Longitude:F6}");
            
            foreach (var schedule in activeSchedules)
            {
                await CheckScheduleAsync(schedule, currentLocation);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? [Monitoring] Error in monitoring: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
        }
    }
    
    private async Task CheckScheduleAsync(NotificationSchedule schedule, Location currentLocation)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] Checking: {schedule.StopName}");
            
            if (schedule.LastNotificationSent.HasValue)
            {
                var timeSinceLastNotification = DateTime.Now - schedule.LastNotificationSent.Value;
                if (timeSinceLastNotification.TotalSeconds < schedule.CheckIntervalSeconds)
                {
                    System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] Skipping {schedule.StopName} - too soon since last notification ({timeSinceLastNotification.TotalSeconds:F0}s < {schedule.CheckIntervalSeconds}s)");
                    return;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] No previous notification for {schedule.StopName}");
            }
            
            var stop = await _databaseService.GetStopAsync(schedule.StopCode);
            if (stop == null)
            {
                System.Diagnostics.Debug.WriteLine($"? [Schedule Check] Stop {schedule.StopCode} not found in database");
                System.Diagnostics.Debug.WriteLine($"   ?? This usually means the stop wasn't saved when creating the schedule.");
                System.Diagnostics.Debug.WriteLine($"   ?? Try deleting and recreating the schedule, or go to the stop details page first.");
                return;
            }
            
            if (stop.StopLat == 0 && stop.StopLng == 0)
            {
                System.Diagnostics.Debug.WriteLine($"? [Schedule Check] Stop {schedule.StopCode} has no coordinates (lat/lng = 0,0)");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] Stop location: {stop.StopLat:F6}, {stop.StopLng:F6}");
            
            var distance = CalculateDistance(
                currentLocation.Latitude, 
                currentLocation.Longitude, 
                stop.StopLat, 
                stop.StopLng);
            
            System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] Distance to {schedule.StopName}: {distance:F0}m (limit: {schedule.ProximityRadius}m)");
            
            if (distance > schedule.ProximityRadius)
            {
                System.Diagnostics.Debug.WriteLine($"? [Schedule Check] Too far from {schedule.StopName}: {distance:F0}m > {schedule.ProximityRadius}m");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"? [Schedule Check] Within range of {schedule.StopName}: {distance:F0}m");
            System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] Fetching arrivals for stop {schedule.StopCode}...");
            
            var arrivals = await _oasaService.GetStopArrivalsAsync(schedule.StopCode);
            var routes = await _oasaService.GetRoutesForStopAsync(schedule.StopCode);
            
            System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] Received {arrivals.Count} arrivals and {routes.Count} routes");
            
            var upcomingBuses = new List<(string lineId, string destination, int minutes)>();
            
            foreach (var arrival in arrivals)
            {
                var minutes = arrival.MinutesUntilArrival;
                System.Diagnostics.Debug.WriteLine($"   ?? Arrival: RouteCode={arrival.RouteCode}, Minutes={minutes}");
                
                if (minutes > 0 && minutes <= schedule.MinMinutesThreshold)
                {
                    var route = routes.FirstOrDefault(r => r.RouteCode == arrival.RouteCode);
                    if (route != null)
                    {
                        var lineId = route.LineID ?? "?";
                        var destination = route.RouteDescrEng ?? route.RouteDescr ?? "Unknown";
                        upcomingBuses.Add((lineId, destination, minutes));
                        System.Diagnostics.Debug.WriteLine($"   ? Added to notification: Line {lineId} to {destination} in {minutes} min");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"   ?? No route info found for RouteCode: {arrival.RouteCode}");
                    }
                }
                else if (minutes <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"   ?? Skipped: already departed or arriving now");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"   ?? Skipped: {minutes} min > threshold {schedule.MinMinutesThreshold}");
                }
            }
            
            if (upcomingBuses.Any())
            {
                // Get max buses setting from configuration
                await _settingsService.InitializeDefaultSettingsAsync();
                var maxBuses = _settingsService.GetMaxBusesInNotification();
                
                // Limit to configured number of buses, sorted by arrival time
                var topBuses = upcomingBuses.OrderBy(b => b.minutes).Take(maxBuses).ToList();
                
                System.Diagnostics.Debug.WriteLine($"?? [Notification] Sending notification for {schedule.StopName} with {topBuses.Count} bus(es) (out of {upcomingBuses.Count} total, max={maxBuses})");
                
                await _notificationService.ShowBusArrivalNotificationAsync(
                    schedule.StopName,
                    topBuses);
                
                await _databaseService.UpdateLastNotificationTimeAsync(schedule.Id);
                System.Diagnostics.Debug.WriteLine($"? [Notification] Notification sent and timestamp updated");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"?? [Schedule Check] No buses within threshold ({schedule.MinMinutesThreshold} min) for {schedule.StopName}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? [Schedule Check] Error checking schedule for {schedule.StopName}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
        }
    }
    
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadius = 6371000;
        
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadius * c;
    }
    
    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
    
    public bool IsMonitoring => _isMonitoring;
}
