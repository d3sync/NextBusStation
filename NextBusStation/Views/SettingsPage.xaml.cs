using NextBusStation.Models;
using NextBusStation.ViewModels;

namespace NextBusStation.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;
    
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
    
    private async void OnSettingChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is AppSettings setting)
        {
            setting.Value = e.NewTextValue;
            await _viewModel.SaveSettingCommand.ExecuteAsync(setting);
        }
    }
    
    private async void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch switchControl && switchControl.BindingContext is AppSettings setting)
        {
            setting.Value = e.Value.ToString();
            await _viewModel.SaveSettingCommand.ExecuteAsync(setting);
        }
    }
    
    private async void OnTimePickerChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Time" && sender is TimePicker timePicker && timePicker.BindingContext is AppSettings setting)
        {
            var time = timePicker.Time;
            setting.Value = time.HasValue ? $"{time.Value.Hours:D2}:{time.Value.Minutes:D2}" : "00:00";
            await _viewModel.SaveSettingCommand.ExecuteAsync(setting);
        }
    }
    
    private async void OnSliderChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider && slider.BindingContext is AppSettings setting)
        {
            setting.Value = ((int)e.NewValue).ToString();
            await _viewModel.SaveSettingCommand.ExecuteAsync(setting);
        }
    }
}
