using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextBusStation.Models;
using NextBusStation.Services;

namespace NextBusStation.ViewModels;

public partial class EditScheduleViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    
    [ObservableProperty]
    private NotificationSchedule? _schedule;
    
    [ObservableProperty]
    private bool _isNew;
    
    [ObservableProperty]
    private TimeSpan _startTime;
    
    [ObservableProperty]
    private TimeSpan _endTime;
    
    [ObservableProperty]
    private int _proximityRadius;
    
    [ObservableProperty]
    private int _checkIntervalMinutes;
    
    [ObservableProperty]
    private int _minMinutesThreshold;
    
    [ObservableProperty]
    private bool _mondayEnabled;
    
    [ObservableProperty]
    private bool _tuesdayEnabled;
    
    [ObservableProperty]
    private bool _wednesdayEnabled;
    
    [ObservableProperty]
    private bool _thursdayEnabled;
    
    [ObservableProperty]
    private bool _fridayEnabled;
    
    [ObservableProperty]
    private bool _saturdayEnabled;
    
    [ObservableProperty]
    private bool _sundayEnabled;
    
    [ObservableProperty]
    private bool _isSaving;
    
    public EditScheduleViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Schedule", out var scheduleObj) && scheduleObj is NotificationSchedule schedule)
        {
            Schedule = schedule;
            IsNew = query.TryGetValue("IsNew", out var isNewObj) && isNewObj is bool isNew && isNew;
            
            StartTime = schedule.StartTime;
            EndTime = schedule.EndTime;
            ProximityRadius = (int)schedule.ProximityRadius;
            CheckIntervalMinutes = schedule.CheckIntervalSeconds / 60;
            MinMinutesThreshold = schedule.MinMinutesThreshold;
            MondayEnabled = schedule.MondayEnabled;
            TuesdayEnabled = schedule.TuesdayEnabled;
            WednesdayEnabled = schedule.WednesdayEnabled;
            ThursdayEnabled = schedule.ThursdayEnabled;
            FridayEnabled = schedule.FridayEnabled;
            SaturdayEnabled = schedule.SaturdayEnabled;
            SundayEnabled = schedule.SundayEnabled;
            
            System.Diagnostics.Debug.WriteLine($"?? EditScheduleViewModel loaded:");
            System.Diagnostics.Debug.WriteLine($"   Stop: {schedule.StopName} ({schedule.StopCode})");
            System.Diagnostics.Debug.WriteLine($"   IsNew: {IsNew}");
            System.Diagnostics.Debug.WriteLine($"   Time: {StartTime:hh\\:mm} - {EndTime:hh\\:mm}");
        }
    }
    
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Schedule == null)
        {
            System.Diagnostics.Debug.WriteLine("? Cannot save: Schedule is null");
            await Shell.Current.DisplayAlert("Error", "Schedule data is missing", "OK");
            return;
        }
        
        // Validate that at least one day is enabled
        if (!MondayEnabled && !TuesdayEnabled && !WednesdayEnabled && !ThursdayEnabled && 
            !FridayEnabled && !SaturdayEnabled && !SundayEnabled)
        {
            await Shell.Current.DisplayAlert("Validation Error", "Please select at least one day", "OK");
            return;
        }
        
        // Validate time range
        if (EndTime <= StartTime)
        {
            await Shell.Current.DisplayAlert("Validation Error", "End time must be after start time", "OK");
            return;
        }
        
        IsSaving = true;
        
        try
        {
            System.Diagnostics.Debug.WriteLine($"?? Saving schedule for {Schedule.StopName}...");
            
            Schedule.StartTime = StartTime;
            Schedule.EndTime = EndTime;
            Schedule.ProximityRadius = ProximityRadius;
            Schedule.CheckIntervalSeconds = CheckIntervalMinutes * 60;
            Schedule.MinMinutesThreshold = MinMinutesThreshold;
            Schedule.MondayEnabled = MondayEnabled;
            Schedule.TuesdayEnabled = TuesdayEnabled;
            Schedule.WednesdayEnabled = WednesdayEnabled;
            Schedule.ThursdayEnabled = ThursdayEnabled;
            Schedule.FridayEnabled = FridayEnabled;
            Schedule.SaturdayEnabled = SaturdayEnabled;
            Schedule.SundayEnabled = SundayEnabled;
            
            System.Diagnostics.Debug.WriteLine($"   Time: {StartTime:hh\\:mm} - {EndTime:hh\\:mm}");
            System.Diagnostics.Debug.WriteLine($"   Proximity: {ProximityRadius}m");
            System.Diagnostics.Debug.WriteLine($"   Check interval: {CheckIntervalMinutes} min");
            System.Diagnostics.Debug.WriteLine($"   Alert threshold: ?{MinMinutesThreshold} min");
            System.Diagnostics.Debug.WriteLine($"   Days: M:{MondayEnabled} T:{TuesdayEnabled} W:{WednesdayEnabled} Th:{ThursdayEnabled} F:{FridayEnabled} Sa:{SaturdayEnabled} Su:{SundayEnabled}");
            
            var result = await _databaseService.SaveScheduleAsync(Schedule);
            
            System.Diagnostics.Debug.WriteLine($"? Schedule saved successfully (Result: {result})");
            System.Diagnostics.Debug.WriteLine($"   Schedule ID: {Schedule.Id}");
            
            // Show success message
            await Shell.Current.DisplayAlert(
                "Success", 
                $"Schedule for {Schedule.StopName} saved successfully!", 
                "OK");
            
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error saving schedule:");
            System.Diagnostics.Debug.WriteLine($"   Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            
            await Shell.Current.DisplayAlert(
                "Error", 
                $"Failed to save schedule: {ex.Message}", 
                "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }
    
    [RelayCommand]
    public async Task CancelAsync()
    {
        var hasChanges = 
            Schedule?.StartTime != StartTime ||
            Schedule?.EndTime != EndTime ||
            Schedule?.ProximityRadius != ProximityRadius ||
            Schedule?.CheckIntervalSeconds != CheckIntervalMinutes * 60 ||
            Schedule?.MinMinutesThreshold != MinMinutesThreshold;
        
        if (hasChanges)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Unsaved Changes",
                "You have unsaved changes. Are you sure you want to cancel?",
                "Yes, Cancel",
                "No, Continue Editing");
            
            if (!confirm)
                return;
        }
        
        await Shell.Current.GoToAsync("..");
    }
    
    [RelayCommand]
    public void SetWeekdays()
    {
        MondayEnabled = true;
        TuesdayEnabled = true;
        WednesdayEnabled = true;
        ThursdayEnabled = true;
        FridayEnabled = true;
        SaturdayEnabled = false;
        SundayEnabled = false;
        
        System.Diagnostics.Debug.WriteLine("?? Set to weekdays only");
    }
    
    [RelayCommand]
    public void SetEveryday()
    {
        MondayEnabled = true;
        TuesdayEnabled = true;
        WednesdayEnabled = true;
        ThursdayEnabled = true;
        FridayEnabled = true;
        SaturdayEnabled = true;
        SundayEnabled = true;
        
        System.Diagnostics.Debug.WriteLine("?? Set to every day");
    }
}
