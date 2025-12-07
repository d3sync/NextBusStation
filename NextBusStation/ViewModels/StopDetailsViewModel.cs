using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextBusStation.Models;
using NextBusStation.Services;
using System.Collections.ObjectModel;

namespace NextBusStation.ViewModels;

public partial class StopDetailsViewModel : ObservableObject, IQueryAttributable
{
    private readonly OasaApiService _oasaService;
    private readonly DatabaseService _databaseService;
    private readonly SettingsService _settingsService;
    
    [ObservableProperty]
    private BusStop? _selectedStop;
    
    [ObservableProperty]
    private ObservableCollection<StopArrival> _arrivals = new();
    
    [ObservableProperty]
    private ObservableCollection<RouteInfo> _routes = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private bool _isFavorite;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    private System.Timers.Timer? _refreshTimer;
    
    public StopDetailsViewModel(OasaApiService oasaService, DatabaseService databaseService, SettingsService settingsService)
    {
        _oasaService = oasaService;
        _databaseService = databaseService;
        _settingsService = settingsService;
    }
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Stop", out var stopObj) && stopObj is BusStop stop)
        {
            SelectedStop = stop;
            IsFavorite = stop.IsFavorite;
            _ = LoadStopDetailsAsync();
        }
    }
    
    [RelayCommand]
    public async Task LoadStopDetailsAsync()
    {
        if (SelectedStop == null || IsLoading)
            return;
        
        IsLoading = true;
        StatusMessage = "Loading stop information...";
        
        try
        {
            var arrivalsTask = _oasaService.GetStopArrivalsAsync(SelectedStop.StopCode);
            var routesTask = _oasaService.GetRoutesForStopAsync(SelectedStop.StopCode);
            
            await Task.WhenAll(arrivalsTask, routesTask);
            
            var arrivals = await arrivalsTask;
            var routes = await routesTask;
            
            // Create lookup for route info by RouteCode (handles duplicates)
            var routeLookup = routes.ToLookup(r => r.RouteCode);
            
            Arrivals.Clear();
            foreach (var arrival in arrivals.OrderBy(a => a.MinutesUntilArrival))
            {
                // Enrich arrival with route info (take first matching route)
                var routeInfo = routeLookup[arrival.RouteCode].FirstOrDefault();
                if (routeInfo != null)
                {
                    arrival.LineID = routeInfo.LineID ?? "—";
                    arrival.RouteDescription = routeInfo.RouteDescrEng ?? routeInfo.RouteDescr ?? "Unknown Route";
                }
                else
                {
                    // Fallback if route not found in lookup
                    arrival.LineID = "—";
                    arrival.RouteDescription = $"Route {arrival.RouteCode}";
                    
                    System.Diagnostics.Debug.WriteLine($"?? No route info found for RouteCode: {arrival.RouteCode}");
                }
                
                Arrivals.Add(arrival);
            }
            
            Routes.Clear();
            foreach (var route in routes)
            {
                // Ensure LineID has a value for display
                if (string.IsNullOrEmpty(route.LineID))
                {
                    route.LineID = "—";
                }
                Routes.Add(route);
            }
            
            StatusMessage = arrivals.Any() 
                ? $"{arrivals.Count} arrivals found" 
                : "No upcoming arrivals";
            
            StartAutoRefresh();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading stop details: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    public async Task ToggleFavoriteAsync()
    {
        if (SelectedStop == null)
            return;
        
        await _databaseService.ToggleFavoriteAsync(SelectedStop.StopCode);
        IsFavorite = !IsFavorite;
    }
    
    [RelayCommand]
    public async Task CreateNotificationScheduleAsync()
    {
        if (SelectedStop == null)
            return;
        
        StopAutoRefresh();
        
        await _settingsService.InitializeDefaultSettingsAsync();
        
        var schedule = new NotificationSchedule
        {
            StopCode = SelectedStop.StopCode,
            StopName = SelectedStop.StopDescrEng ?? SelectedStop.StopDescr,
            StartTime = _settingsService.GetDefaultStartTime(),
            EndTime = _settingsService.GetDefaultEndTime(),
            ProximityRadius = _settingsService.GetDefaultProximityRadius(),
            CheckIntervalSeconds = _settingsService.GetDefaultCheckInterval(),
            MinMinutesThreshold = _settingsService.GetDefaultMinutesThreshold(),
            MondayEnabled = true,
            TuesdayEnabled = true,
            WednesdayEnabled = true,
            ThursdayEnabled = true,
            FridayEnabled = true
        };
        
        var navigationParameter = new Dictionary<string, object>
        {
            { "Schedule", schedule },
            { "IsNew", true }
        };
        
        await Shell.Current.GoToAsync("editschedule", navigationParameter);
    }
    
    private void StartAutoRefresh()
    {
        StopAutoRefresh();
        
        _refreshTimer = new System.Timers.Timer(30000);
        _refreshTimer.Elapsed += async (s, e) => await LoadStopDetailsAsync();
        _refreshTimer.AutoReset = true;
        _refreshTimer.Start();
    }
    
    private void StopAutoRefresh()
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        _refreshTimer = null;
    }
    
    [RelayCommand]
    public async Task GoBackAsync()
    {
        StopAutoRefresh();
        await Shell.Current.GoToAsync("..");
    }
}
