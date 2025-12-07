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
    private Timer? _monitoringTimer;
    private bool _isMonitoring;
    
    public BusMonitoringService(
        OasaApiService oasaService,
        DatabaseService databaseService,
        LocationService locationService,
        NotificationService notificationService)
    {
        _oasaService = oasaService;
        _databaseService = databaseService;
        _locationService = locationService;
        _notificationService = notificationService;
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
            var activeSchedules = await _databaseService.GetActiveSchedulesAsync();
            
            if (!activeSchedules.Any())
            {
                System.Diagnostics.Debug.WriteLine("No active schedules");
                return;
            }
            
            var currentLocation = await _locationService.GetCurrentLocationAsync();
            
            if (currentLocation == null)
            {
                System.Diagnostics.Debug.WriteLine("Could not get current location");
                return;
            }
            
            foreach (var schedule in activeSchedules)
            {
                await CheckScheduleAsync(schedule, currentLocation);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in monitoring: {ex.Message}");
        }
    }
    
    private async Task CheckScheduleAsync(NotificationSchedule schedule, Location currentLocation)
    {
        try
        {
            if (schedule.LastNotificationSent.HasValue)
            {
                var timeSinceLastNotification = DateTime.Now - schedule.LastNotificationSent.Value;
                if (timeSinceLastNotification.TotalSeconds < schedule.CheckIntervalSeconds)
                {
                    System.Diagnostics.Debug.WriteLine($"Skipping {schedule.StopName} - too soon since last notification");
                    return;
                }
            }
            
            var stop = await _databaseService.GetStopAsync(schedule.StopCode);
            if (stop == null)
            {
                System.Diagnostics.Debug.WriteLine($"Stop {schedule.StopCode} not found in database");
                return;
            }
            
            var distance = CalculateDistance(
                currentLocation.Latitude, 
                currentLocation.Longitude, 
                stop.StopLat, 
                stop.StopLng);
            
            if (distance > schedule.ProximityRadius)
            {
                System.Diagnostics.Debug.WriteLine($"Too far from {schedule.StopName}: {distance:F0}m (limit: {schedule.ProximityRadius}m)");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"Within range of {schedule.StopName}: {distance:F0}m");
            
            var arrivals = await _oasaService.GetStopArrivalsAsync(schedule.StopCode);
            var routes = await _oasaService.GetRoutesForStopAsync(schedule.StopCode);
            
            var upcomingBuses = new List<(string lineId, string destination, int minutes)>();
            
            foreach (var arrival in arrivals)
            {
                var minutes = arrival.MinutesUntilArrival;
                
                if (minutes > 0 && minutes <= schedule.MinMinutesThreshold)
                {
                    var route = routes.FirstOrDefault(r => r.RouteCode == arrival.RouteCode);
                    if (route != null)
                    {
                        var lineId = route.LineID ?? "?";
                        var destination = route.RouteDescrEng ?? route.RouteDescr ?? "Unknown";
                        upcomingBuses.Add((lineId, destination, minutes));
                    }
                }
            }
            
            if (upcomingBuses.Any())
            {
                System.Diagnostics.Debug.WriteLine($"Sending notification for {schedule.StopName} with {upcomingBuses.Count} buses");
                
                await _notificationService.ShowBusArrivalNotificationAsync(
                    schedule.StopName,
                    upcomingBuses.OrderBy(b => b.minutes).ToList());
                
                await _databaseService.UpdateLastNotificationTimeAsync(schedule.Id);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No buses within threshold for {schedule.StopName}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking schedule for {schedule.StopName}: {ex.Message}");
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
