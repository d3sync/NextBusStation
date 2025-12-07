using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextBusStation.Models;
using NextBusStation.Services;
using System.Collections.ObjectModel;

namespace NextBusStation.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    
    [ObservableProperty]
    private ObservableCollection<SettingGroup> _settingGroups = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }
    
    [RelayCommand]
    public async Task LoadSettingsAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading settings...";
        
        try
        {
            await _settingsService.InitializeDefaultSettingsAsync();
            
            var allSettings = await _settingsService.GetAllSettingsAsync();
            var grouped = allSettings.GroupBy(s => s.Category)
                .OrderBy(g => g.Key)
                .Select(g => new SettingGroup
                {
                    Category = g.Key,
                    Settings = new ObservableCollection<AppSettings>(g.OrderBy(s => s.DisplayName))
                });
            
            SettingGroups.Clear();
            foreach (var group in grouped)
            {
                SettingGroups.Add(group);
            }
            
            StatusMessage = "Settings loaded";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    public async Task SaveSettingAsync(AppSettings setting)
    {
        try
        {
            await _settingsService.SaveSettingAsync(setting);
            StatusMessage = $"{setting.DisplayName} updated";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving: {ex.Message}";
        }
    }
    
    [RelayCommand]
    public async Task ResetCategoryAsync(string category)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Reset Category",
            $"Reset all settings in '{category}' to defaults?",
            "Reset",
            "Cancel");
        
        if (confirm)
        {
            await _settingsService.ResetCategoryToDefaultsAsync(category);
            await LoadSettingsAsync();
            StatusMessage = $"{category} reset to defaults";
        }
    }
    
    [RelayCommand]
    public async Task ResetAllAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Reset All Settings",
            "Reset ALL settings to factory defaults? This cannot be undone.",
            "Reset All",
            "Cancel");
        
        if (confirm)
        {
            await _settingsService.ResetToDefaultsAsync();
            await LoadSettingsAsync();
            StatusMessage = "All settings reset to defaults";
        }
    }
}

public class SettingGroup
{
    public string Category { get; set; } = string.Empty;
    public ObservableCollection<AppSettings> Settings { get; set; } = new();
}
