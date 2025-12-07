using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextBusStation.Models;
using NextBusStation.Services;
using System.Collections.ObjectModel;

namespace NextBusStation.ViewModels;

public partial class NotificationSchedulesViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly BusMonitoringService _monitoringService;
    private readonly SettingsService _settingsService;
    
    [ObservableProperty]
    private ObservableCollection<NotificationSchedule> _schedules = new();
    
    [ObservableProperty]
    private bool _isMonitoring;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    public NotificationSchedulesViewModel(DatabaseService databaseService, BusMonitoringService monitoringService, SettingsService settingsService)
    {
        _databaseService = databaseService;
        _monitoringService = monitoringService;
        _settingsService = settingsService;
    }
    
    [RelayCommand]
    public async Task LoadSchedulesAsync()
    {
        IsLoading = true;
        
        try
        {
            System.Diagnostics.Debug.WriteLine("?? Loading schedules...");
            
            var schedules = await _databaseService.GetAllSchedulesAsync();
            
            System.Diagnostics.Debug.WriteLine($"   Found {schedules.Count} schedule(s) in database");
            
            Schedules.Clear();
            foreach (var schedule in schedules)
            {
                System.Diagnostics.Debug.WriteLine($"   • {schedule.StopName} ({schedule.StopCode}) - Enabled: {schedule.IsEnabled}");
                Schedules.Add(schedule);
            }
            
            IsMonitoring = _monitoringService.IsMonitoring;
            StatusMessage = Schedules.Any() 
                ? $"{Schedules.Count} schedule(s) configured" 
                : "No schedules configured";
            
            System.Diagnostics.Debug.WriteLine($"? Schedules loaded. Monitoring: {IsMonitoring}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error loading schedules: {ex.Message}");
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public async Task<bool> GetAutoStartSettingAsync()
    {
        await _settingsService.InitializeDefaultSettingsAsync();
        return _settingsService.GetAutoStartMonitoring();
    }
    
    [RelayCommand]
    public async Task AddScheduleAsync(BusStop stop)
    {
        var schedule = new NotificationSchedule
        {
            StopCode = stop.StopCode,
            StopName = stop.StopDescrEng ?? stop.StopDescr,
            StartTime = new TimeSpan(17, 40, 0),
            EndTime = new TimeSpan(18, 25, 0),
            ProximityRadius = 500,
            CheckIntervalSeconds = 300,
            MinMinutesThreshold = 10,
            MondayEnabled = true,
            TuesdayEnabled = true,
            WednesdayEnabled = true,
            ThursdayEnabled = true,
            FridayEnabled = true,
            SaturdayEnabled = false,
            SundayEnabled = false
        };
        
        var navigationParameter = new Dictionary<string, object>
        {
            { "Schedule", schedule },
            { "IsNew", true }
        };
        
        await Shell.Current.GoToAsync("editschedule", navigationParameter);
    }
    
    [RelayCommand]
    public async Task EditScheduleAsync(NotificationSchedule schedule)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Schedule", schedule },
            { "IsNew", false }
        };
        
        await Shell.Current.GoToAsync("editschedule", navigationParameter);
    }
    
    [RelayCommand]
    public async Task DeleteScheduleAsync(NotificationSchedule schedule)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Delete Schedule",
            $"Are you sure you want to delete the schedule for {schedule.StopName}?",
            "Delete",
            "Cancel");
        
        if (confirm)
        {
            await _databaseService.DeleteScheduleAsync(schedule);
            await LoadSchedulesAsync();
        }
    }
    
    [RelayCommand]
    public async Task ToggleScheduleAsync(NotificationSchedule schedule)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"?? Saving schedule toggle: {schedule.StopName} - Enabled: {schedule.IsEnabled}");
            
            await _databaseService.SaveScheduleAsync(schedule);
            
            System.Diagnostics.Debug.WriteLine($"? Schedule toggle saved");
            
            // Update monitoring status without full reload
            IsMonitoring = _monitoringService.IsMonitoring;
            StatusMessage = Schedules.Count(s => s.IsEnabled) > 0
                ? $"{Schedules.Count} schedule(s) - {Schedules.Count(s => s.IsEnabled)} enabled"
                : $"{Schedules.Count} schedule(s) configured";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error toggling schedule: {ex.Message}");
            
            // Revert the change in UI
            schedule.IsEnabled = !schedule.IsEnabled;
            
            await Shell.Current.DisplayAlert("Error", 
                $"Failed to update schedule: {ex.Message}", "OK");
        }
    }
    
    [RelayCommand]
    public async Task ToggleMonitoringAsync()
    {
        if (IsMonitoring)
        {
            _monitoringService.StopMonitoring();
            IsMonitoring = false;
            StatusMessage = "Monitoring stopped";
        }
        else
        {
            if (!Schedules.Any(s => s.IsEnabled))
            {
                await Shell.Current.DisplayAlert(
                    "No Active Schedules",
                    "Please enable at least one schedule before starting monitoring.",
                    "OK");
                return;
            }
            
            await _monitoringService.StartMonitoringAsync();
            IsMonitoring = true;
            StatusMessage = "Monitoring active";
        }
    }
}
