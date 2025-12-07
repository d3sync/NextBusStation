using NextBusStation.ViewModels;
using NextBusStation.Models;

namespace NextBusStation.Views;

public partial class NotificationSchedulesPage : ContentPage
{
    private readonly NotificationSchedulesViewModel _viewModel;
    
    public NotificationSchedulesPage(NotificationSchedulesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await _viewModel.LoadSchedulesCommand.ExecuteAsync(null);
        
        // Auto-start monitoring if setting is enabled and there are enabled schedules
        var autoStart = await _viewModel.GetAutoStartSettingAsync();
        
        if (autoStart && !_viewModel.IsMonitoring && _viewModel.Schedules.Any(s => s.IsEnabled))
        {
            System.Diagnostics.Debug.WriteLine("?? Auto-starting background monitoring (setting enabled)...");
            await _viewModel.ToggleMonitoringCommand.ExecuteAsync(null);
        }
    }
    
    private async void OnScheduleToggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch switchControl && switchControl.BindingContext is NotificationSchedule schedule)
        {
            System.Diagnostics.Debug.WriteLine($"?? Schedule toggled: {schedule.StopName} ? {e.Value}");
            
            schedule.IsEnabled = e.Value;
            await _viewModel.ToggleScheduleCommand.ExecuteAsync(schedule);
            
            // Check auto-start setting
            var autoStart = await _viewModel.GetAutoStartSettingAsync();
            
            if (autoStart)
            {
                // Auto-start monitoring if this is the first enabled schedule
                if (e.Value && !_viewModel.IsMonitoring)
                {
                    System.Diagnostics.Debug.WriteLine("?? Auto-starting monitoring (schedule enabled)...");
                    await _viewModel.ToggleMonitoringCommand.ExecuteAsync(null);
                }
                // Auto-stop monitoring if no schedules are enabled
                else if (!e.Value && !_viewModel.Schedules.Any(s => s.IsEnabled) && _viewModel.IsMonitoring)
                {
                    System.Diagnostics.Debug.WriteLine("?? Auto-stopping monitoring (no enabled schedules)...");
                    await _viewModel.ToggleMonitoringCommand.ExecuteAsync(null);
                }
            }
        }
    }
    
    private async void OnEditSwipeItemInvoked(object sender, EventArgs e)
    {
        // sender is SwipeItemView
        if (sender is Element element && element.BindingContext is NotificationSchedule schedule)
        {
            System.Diagnostics.Debug.WriteLine($"✏️ Edit swipe invoked for: {schedule.StopName}");
            await _viewModel.EditScheduleCommand.ExecuteAsync(schedule);
        }
    }

    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        // sender is SwipeItemView
        if (sender is Element element && element.BindingContext is NotificationSchedule schedule)
        {
            System.Diagnostics.Debug.WriteLine($"🗑️ Delete swipe invoked for: {schedule.StopName}");
            await _viewModel.DeleteScheduleCommand.ExecuteAsync(schedule);
        }
    }
}
