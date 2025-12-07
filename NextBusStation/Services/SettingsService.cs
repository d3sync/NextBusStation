using NextBusStation.Models;
using SQLite;

namespace NextBusStation.Services;

public class SettingsService
{
    private readonly DatabaseService _databaseService;
    private Dictionary<string, string> _cachedSettings = new();
    
    public SettingsService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    public async Task InitializeDefaultSettingsAsync()
    {
        await _databaseService.InitializeAsync();
        
        var defaultSettings = new List<AppSettings>
        {
            new() { Key = SettingsKeys.DefaultProximityRadius, Value = "500", Category = SettingsCategories.NotificationDefaults, 
                DisplayName = "Default Proximity Radius", Description = "Default radius in meters for new schedules", 
                DataType = "int", MinValue = "100", MaxValue = "2000", DefaultValue = "500" },
            
            new() { Key = SettingsKeys.DefaultCheckInterval, Value = "300", Category = SettingsCategories.NotificationDefaults,
                DisplayName = "Default Check Interval", Description = "How often to check for buses (seconds)",
                DataType = "int", MinValue = "60", MaxValue = "1800", DefaultValue = "300" },
            
            new() { Key = SettingsKeys.DefaultMinutesThreshold, Value = "10", Category = SettingsCategories.NotificationDefaults,
                DisplayName = "Default Minutes Threshold", Description = "Notify when bus is within X minutes",
                DataType = "int", MinValue = "5", MaxValue = "30", DefaultValue = "10" },
            
            new() { Key = SettingsKeys.DefaultStartTime, Value = "17:40", Category = SettingsCategories.NotificationDefaults,
                DisplayName = "Default Start Time", Description = "Default monitoring start time",
                DataType = "time", DefaultValue = "17:40" },
            
            new() { Key = SettingsKeys.DefaultEndTime, Value = "18:25", Category = SettingsCategories.NotificationDefaults,
                DisplayName = "Default End Time", Description = "Default monitoring end time",
                DataType = "time", DefaultValue = "18:25" },
            
            new() { Key = SettingsKeys.MaxNearbyStops, Value = "20", Category = SettingsCategories.MapSettings,
                DisplayName = "Max Nearby Stops", Description = "Maximum number of stops to show",
                DataType = "int", MinValue = "5", MaxValue = "50", DefaultValue = "20" },
            
            new() { Key = SettingsKeys.SearchRadius, Value = "1000", Category = SettingsCategories.MapSettings,
                DisplayName = "Search Radius", Description = "Search radius for nearby stops (meters)",
                DataType = "int", MinValue = "500", MaxValue = "5000", DefaultValue = "1000" },
            
            new() { Key = SettingsKeys.RefreshIntervalSeconds, Value = "30", Category = SettingsCategories.ApiSettings,
                DisplayName = "Refresh Interval", Description = "Auto-refresh interval on stop details page (seconds)",
                DataType = "int", MinValue = "15", MaxValue = "120", DefaultValue = "30" },
            
            new() { Key = SettingsKeys.ApiTimeoutSeconds, Value = "30", Category = SettingsCategories.ApiSettings,
                DisplayName = "API Timeout", Description = "HTTP request timeout (seconds)",
                DataType = "int", MinValue = "10", MaxValue = "60", DefaultValue = "30" },
            
            new() { Key = SettingsKeys.CacheExpirationMinutes, Value = "60", Category = SettingsCategories.ApiSettings,
                DisplayName = "Cache Expiration", Description = "How long to cache stop data (minutes)",
                DataType = "int", MinValue = "15", MaxValue = "240", DefaultValue = "60" },
            
            new() { Key = SettingsKeys.NotificationVibrate, Value = "true", Category = SettingsCategories.NotificationBehavior,
                DisplayName = "Vibrate on Notification", Description = "Vibrate when notification arrives",
                DataType = "bool", DefaultValue = "true" },
            
            new() { Key = SettingsKeys.NotificationSound, Value = "true", Category = SettingsCategories.NotificationBehavior,
                DisplayName = "Notification Sound", Description = "Play sound on notification",
                DataType = "bool", DefaultValue = "true" },
            
            new() { Key = SettingsKeys.NotificationPriority, Value = "High", Category = SettingsCategories.NotificationBehavior,
                DisplayName = "Notification Priority", Description = "Android notification priority",
                DataType = "string", DefaultValue = "High" },
            
            new() { Key = SettingsKeys.UseEnglishDescriptions, Value = "true", Category = SettingsCategories.Appearance,
                DisplayName = "Use English Descriptions", Description = "Show English stop/route names when available",
                DataType = "bool", DefaultValue = "true" },
            
            new() { Key = SettingsKeys.ThemePreference, Value = "System", Category = SettingsCategories.Appearance,
                DisplayName = "Theme", Description = "App theme (Light/Dark/System)",
                DataType = "string", DefaultValue = "System" },
            
            new() { Key = SettingsKeys.ShowStopCodes, Value = "true", Category = SettingsCategories.Appearance,
                DisplayName = "Show Stop Codes", Description = "Display stop codes in lists",
                DataType = "bool", DefaultValue = "true" },
            
            new() { Key = SettingsKeys.AutoStartMonitoring, Value = "true", Category = SettingsCategories.Performance,
                DisplayName = "Auto-Start Monitoring", Description = "Automatically start monitoring when schedules are enabled",
                DataType = "bool", DefaultValue = "true" },
            
            new() { Key = SettingsKeys.BatteryOptimizationMode, Value = "Balanced", Category = SettingsCategories.Performance,
                DisplayName = "Battery Mode", Description = "Performance vs battery (Aggressive/Balanced/Performance)",
                DataType = "string", DefaultValue = "Balanced" },
            
            new() { Key = SettingsKeys.WifiOnlyUpdates, Value = "false", Category = SettingsCategories.Performance,
                DisplayName = "WiFi Only Updates", Description = "Only check for updates on WiFi",
                DataType = "bool", DefaultValue = "false" },
            
            new() { Key = SettingsKeys.DebugMode, Value = "false", Category = SettingsCategories.Advanced,
                DisplayName = "Debug Mode", Description = "Enable debug logging",
                DataType = "bool", DefaultValue = "false" },
            
            new() { Key = SettingsKeys.LogLevel, Value = "Info", Category = SettingsCategories.Advanced,
                DisplayName = "Log Level", Description = "Logging detail level",
                DataType = "string", DefaultValue = "Info" }
        };
        
        foreach (var setting in defaultSettings)
        {
            var existing = await GetSettingAsync(setting.Key);
            if (existing == null)
            {
                await SaveSettingAsync(setting);
            }
        }
        
        await LoadCacheAsync();
    }
    
    private async Task LoadCacheAsync()
    {
        var allSettings = await GetAllSettingsAsync();
        _cachedSettings = allSettings.ToDictionary(s => s.Key, s => s.Value);
    }
    
    public async Task<AppSettings?> GetSettingAsync(string key)
    {
        await _databaseService.InitializeAsync();
        var db = await _databaseService.GetDatabaseConnectionAsync();
        
        return await db.Table<AppSettings>()
            .Where(s => s.Key == key)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<AppSettings>> GetAllSettingsAsync()
    {
        await _databaseService.InitializeAsync();
        var db = await _databaseService.GetDatabaseConnectionAsync();
        
        return await db.Table<AppSettings>().ToListAsync();
    }
    
    public async Task<List<AppSettings>> GetSettingsByCategoryAsync(string category)
    {
        await _databaseService.InitializeAsync();
        var db = await _databaseService.GetDatabaseConnectionAsync();
        
        return await db.Table<AppSettings>()
            .Where(s => s.Category == category)
            .ToListAsync();
    }
    
    public async Task SaveSettingAsync(AppSettings setting)
    {
        await _databaseService.InitializeAsync();
        var db = await _databaseService.GetDatabaseConnectionAsync();
        
        setting.UpdatedAt = DateTime.Now;
        
        var existing = await GetSettingAsync(setting.Key);
        if (existing != null)
        {
            setting.Id = existing.Id;
            await db.UpdateAsync(setting);
        }
        else
        {
            await db.InsertAsync(setting);
        }
        
        _cachedSettings[setting.Key] = setting.Value;
    }
    
    public async Task<T> GetValueAsync<T>(string key, T defaultValue)
    {
        if (_cachedSettings.TryGetValue(key, out var cachedValue))
        {
            return ConvertValue<T>(cachedValue, defaultValue);
        }
        
        var setting = await GetSettingAsync(key);
        if (setting != null)
        {
            _cachedSettings[key] = setting.Value;
            return ConvertValue<T>(setting.Value, defaultValue);
        }
        
        return defaultValue;
    }
    
    public async Task SetValueAsync<T>(string key, T value)
    {
        var stringValue = value?.ToString() ?? string.Empty;
        
        var setting = await GetSettingAsync(key);
        if (setting == null)
        {
            setting = new AppSettings
            {
                Key = key,
                Value = stringValue,
                Category = SettingsCategories.Advanced,
                DisplayName = key,
                Description = "Custom setting"
            };
        }
        else
        {
            setting.Value = stringValue;
        }
        
        await SaveSettingAsync(setting);
    }
    
    private T ConvertValue<T>(string value, T defaultValue)
    {
        try
        {
            if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(value);
            if (typeof(T) == typeof(double))
                return (T)(object)double.Parse(value);
            if (typeof(T) == typeof(bool))
                return (T)(object)bool.Parse(value);
            if (typeof(T) == typeof(TimeSpan))
                return (T)(object)TimeSpan.Parse(value);
            
            return (T)(object)value;
        }
        catch
        {
            return defaultValue;
        }
    }
    
    public async Task ResetToDefaultsAsync()
    {
        var allSettings = await GetAllSettingsAsync();
        
        foreach (var setting in allSettings)
        {
            if (!string.IsNullOrEmpty(setting.DefaultValue))
            {
                setting.Value = setting.DefaultValue;
                await SaveSettingAsync(setting);
            }
        }
    }
    
    public async Task ResetCategoryToDefaultsAsync(string category)
    {
        var settings = await GetSettingsByCategoryAsync(category);
        
        foreach (var setting in settings)
        {
            if (!string.IsNullOrEmpty(setting.DefaultValue))
            {
                setting.Value = setting.DefaultValue;
                await SaveSettingAsync(setting);
            }
        }
    }
    
    public int GetDefaultProximityRadius() => 
        _cachedSettings.TryGetValue(SettingsKeys.DefaultProximityRadius, out var val) ? int.Parse(val) : 500;
    
    public int GetDefaultCheckInterval() => 
        _cachedSettings.TryGetValue(SettingsKeys.DefaultCheckInterval, out var val) ? int.Parse(val) : 300;
    
    public int GetDefaultMinutesThreshold() => 
        _cachedSettings.TryGetValue(SettingsKeys.DefaultMinutesThreshold, out var val) ? int.Parse(val) : 10;
    
    public TimeSpan GetDefaultStartTime() => 
        _cachedSettings.TryGetValue(SettingsKeys.DefaultStartTime, out var val) ? TimeSpan.Parse(val) : new TimeSpan(17, 40, 0);
    
    public TimeSpan GetDefaultEndTime() => 
        _cachedSettings.TryGetValue(SettingsKeys.DefaultEndTime, out var val) ? TimeSpan.Parse(val) : new TimeSpan(18, 25, 0);
    
    public int GetMaxNearbyStops() => 
        _cachedSettings.TryGetValue(SettingsKeys.MaxNearbyStops, out var val) ? int.Parse(val) : 20;
    
    public int GetSearchRadius() => 
        _cachedSettings.TryGetValue(SettingsKeys.SearchRadius, out var val) ? int.Parse(val) : 1000;
    
    public bool GetAutoStartMonitoring() => 
        _cachedSettings.TryGetValue(SettingsKeys.AutoStartMonitoring, out var val) ? bool.Parse(val) : true;
}
