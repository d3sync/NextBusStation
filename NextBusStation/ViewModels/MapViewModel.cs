using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextBusStation.Models;
using NextBusStation.Services;
using System.Collections.ObjectModel;

namespace NextBusStation.ViewModels;

public partial class MapViewModel : ObservableObject
{
    private readonly OasaApiService _oasaService;
    private readonly LocationService _locationService;
    private readonly DatabaseService _databaseService;
    private readonly SettingsService _settingsService;
    
    [ObservableProperty]
    private ObservableCollection<BusStop> _nearbyStops = new();
    
    [ObservableProperty]
    private Location? _currentLocation;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    [ObservableProperty]
    private bool _useTestLocation;
    
    [ObservableProperty]
    private bool _showMap = false;
    
    [ObservableProperty]
    private bool _showDebugFeatures = false;
    
    public MapViewModel(OasaApiService oasaService, LocationService locationService, DatabaseService databaseService, SettingsService settingsService)
    {
        _oasaService = oasaService;
        _locationService = locationService;
        _databaseService = databaseService;
        _settingsService = settingsService;
    }
    
    [RelayCommand]
    public async Task LoadNearbyStopsAsync()
    {
        if (IsLoading)
            return;
        
        IsLoading = true;
        StatusMessage = "Getting location...";
        
        System.Diagnostics.Debug.WriteLine("=================================================");
        System.Diagnostics.Debug.WriteLine("??? MapViewModel: Starting LoadNearbyStops");
        System.Diagnostics.Debug.WriteLine($"   Test mode: {UseTestLocation}");
        
        try
        {
            // Get max stops from settings
            await _settingsService.InitializeDefaultSettingsAsync();
            var maxStops = _settingsService.GetMaxNearbyStops();
            ShowDebugFeatures = _settingsService.GetShowDebugFeatures();
            System.Diagnostics.Debug.WriteLine($"   ?? Max stops setting: {maxStops}");
            System.Diagnostics.Debug.WriteLine($"   ?? Show debug features: {ShowDebugFeatures}");
            
            Location? location;
            
            if (UseTestLocation)
            {
                System.Diagnostics.Debug.WriteLine("?? Using TEST location (Athens, Greece)");
                location = _locationService.GetAthensTestLocation();
                StatusMessage = "Using test location (Athens)";
            }
            else
            {
                var hasPermission = await _locationService.CheckAndRequestLocationPermissionAsync();
                if (!hasPermission)
                {
                    StatusMessage = "Location permission denied";
                    System.Diagnostics.Debug.WriteLine("? Permission denied");
                    return;
                }
                
                location = await _locationService.GetCurrentLocationAsync();
            }
            
            if (location == null)
            {
                StatusMessage = "Could not get current location. Try test mode?";
                System.Diagnostics.Debug.WriteLine("? Location is null");
                return;
            }
            
            CurrentLocation = location;
            StatusMessage = "Loading nearby stops...";
            
            System.Diagnostics.Debug.WriteLine($"?? Using location: {location.Latitude}, {location.Longitude}");
            
            var stops = await _oasaService.GetClosestStopsAsync(
                location.Longitude,
                location.Latitude,
                maxStops);
            
            System.Diagnostics.Debug.WriteLine($"?? Received {stops.Count} stops from API (max requested: {maxStops})");
            
            NearbyStops.Clear();
            foreach (var stop in stops)
            {
                NearbyStops.Add(stop);
            }
            
            StatusMessage = $"Found {stops.Count} nearby stops (max: {maxStops})";
            System.Diagnostics.Debug.WriteLine($"? Success: {stops.Count} stops loaded");
            System.Diagnostics.Debug.WriteLine("=================================================");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"? ERROR in LoadNearbyStops:");
            System.Diagnostics.Debug.WriteLine($"   Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Type: {ex.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            System.Diagnostics.Debug.WriteLine("=================================================");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    public async Task ToggleTestModeAsync()
    {
        UseTestLocation = !UseTestLocation;
        StatusMessage = UseTestLocation 
            ? "TEST MODE: Will use Athens location" 
            : "LIVE MODE: Will use your GPS";
        
        System.Diagnostics.Debug.WriteLine($"?? Test mode toggled: {UseTestLocation}");
        
        if (UseTestLocation)
        {
            await LoadNearbyStopsAsync();
        }
    }
    
    [RelayCommand]
    public async Task SelectStopAsync(BusStop stop)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Stop", stop }
        };
        
        await Shell.Current.GoToAsync("stopdetails", navigationParameter);
    }
    
    [RelayCommand]
    public async Task ToggleFavoriteAsync(string stopCode)
    {
        await _databaseService.ToggleFavoriteAsync(stopCode);
        await LoadNearbyStopsAsync();
    }
    
    [RelayCommand]
    public void ToggleMapView()
    {
        ShowMap = !ShowMap;
    }
    
    [RelayCommand]
    public async Task RefreshDebugSettingAsync()
    {
        try
        {
            await _settingsService.InitializeDefaultSettingsAsync();
            ShowDebugFeatures = _settingsService.GetShowDebugFeatures();
            System.Diagnostics.Debug.WriteLine($"?? [MapViewModel] Debug features: {ShowDebugFeatures}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? [MapViewModel] Error refreshing debug setting: {ex.Message}");
        }
    }
}
