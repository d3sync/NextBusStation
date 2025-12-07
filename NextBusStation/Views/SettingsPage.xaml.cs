using NextBusStation.Models;
using NextBusStation.ViewModels;

namespace NextBusStation.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;
    private readonly Dictionary<string, System.Timers.Timer> _debounceTimers = new();
    
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSettingsCommand.ExecuteAsync(null);
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Clean up all timers
        foreach (var timer in _debounceTimers.Values)
        {
            timer?.Stop();
            timer?.Dispose();
        }
        _debounceTimers.Clear();
    }
    
    private void OnSettingChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is AppSettings setting)
        {
            setting.Value = e.NewTextValue;
            DebounceSave(setting.Key, setting);
        }
    }
    
    private void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch switchControl && switchControl.BindingContext is AppSettings setting)
        {
            setting.Value = e.Value.ToString();
            // Switches should save immediately (not debounced)
            _ = _viewModel.SaveSettingCommand.ExecuteAsync(setting);
        }
    }
    
    private void OnTimePickerChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Time" && sender is TimePicker timePicker && timePicker.BindingContext is AppSettings setting)
        {
            var time = timePicker.Time;
            setting.Value = time.HasValue ? $"{time.Value.Hours:D2}:{time.Value.Minutes:D2}" : "00:00";
            // Time pickers should save immediately (not debounced)
            _ = _viewModel.SaveSettingCommand.ExecuteAsync(setting);
        }
    }
    
    private void OnSliderChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider && slider.BindingContext is AppSettings setting)
        {
            setting.Value = ((int)e.NewValue).ToString();
            DebounceSave(setting.Key, setting);
        }
    }
    
    private void DebounceSave(string key, AppSettings setting)
    {
        // Cancel existing timer for this setting
        if (_debounceTimers.TryGetValue(key, out var existingTimer))
        {
            existingTimer.Stop();
            existingTimer.Dispose();
        }
        
        // Create new debounce timer (1 second delay)
        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += async (s, e) =>
        {
            timer.Stop();
            timer.Dispose();
            _debounceTimers.Remove(key);
            
            // Execute save on main thread
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await _viewModel.SaveSettingCommand.ExecuteAsync(setting);
            });
        };
        
        _debounceTimers[key] = timer;
        timer.Start();
    }
}
