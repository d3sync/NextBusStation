# Configuration & Settings System

## Overview

I've implemented a comprehensive, database-backed settings system that allows fine-grained control over all aspects of the app. All settings are stored in SQLite and can be modified through the Settings UI.

## Settings Categories

### 1. **Notification Defaults**
These settings control the default values when creating new notification schedules:

| Setting | Default | Range | Description |
|---------|---------|-------|-------------|
| **Default Proximity Radius** | 500m | 100-2000m | Default radius for new schedules |
| **Default Check Interval** | 300s (5min) | 60-1800s | How often to check for buses |
| **Default Minutes Threshold** | 10 min | 5-30 min | Notify when bus within X minutes |
| **Default Start Time** | 17:40 | Any | Default monitoring start time |
| **Default End Time** | 18:25 | Any | Default monitoring end time |

### 2. **Map Settings**
Control map behavior and stop discovery:

| Setting | Default | Range | Description |
|---------|---------|-------|-------------|
| **Max Nearby Stops** | 20 | 5-50 | Number of stops to display |
| **Search Radius** | 1000m | 500-5000m | Radius for finding stops |

### 3. **API Settings**
Fine-tune API interaction and performance:

| Setting | Default | Range | Description |
|---------|---------|-------|-------------|
| **Refresh Interval** | 30s | 15-120s | Auto-refresh on details page |
| **API Timeout** | 30s | 10-60s | HTTP request timeout |
| **Cache Expiration** | 60 min | 15-240 min | How long to cache data |

### 4. **Notification Behavior**
Customize notification appearance and behavior:

| Setting | Default | Type | Description |
|---------|---------|------|-------------|
| **Vibrate on Notification** | True | bool | Vibrate when notified |
| **Notification Sound** | True | bool | Play sound |
| **Notification Priority** | High | string | Android priority level |

### 5. **Appearance**
UI customization:

| Setting | Default | Type | Description |
|---------|---------|------|-------------|
| **Use English Descriptions** | True | bool | Prefer English names |
| **Theme** | System | string | Light/Dark/System |
| **Show Stop Codes** | True | bool | Display stop codes |

### 6. **Performance & Battery**
Optimize for battery or performance:

| Setting | Default | Type | Description |
|---------|---------|------|-------------|
| **Battery Mode** | Balanced | string | Aggressive/Balanced/Performance |
| **WiFi Only Updates** | False | bool | Only update on WiFi |

### 7. **Advanced**
Developer and debugging options:

| Setting | Default | Type | Description |
|---------|---------|------|-------------|
| **Debug Mode** | False | bool | Enable debug logging |
| **Log Level** | Info | string | Logging verbosity |

## How Settings Work

### Database Structure

```sql
CREATE TABLE app_settings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Key TEXT NOT NULL UNIQUE,
    Value TEXT NOT NULL,
    Category TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    Description TEXT,
    DataType TEXT DEFAULT 'string',
    MinValue TEXT,
    MaxValue TEXT,
    DefaultValue TEXT,
    UpdatedAt TEXT
);
```

### Usage in Code

#### Reading Settings

```csharp
// In a ViewModel or Service
private readonly SettingsService _settingsService;

// Simple value retrieval
int radius = await _settingsService.GetValueAsync(
    SettingsKeys.DefaultProximityRadius, 500);

// Or use convenience methods
int radius = _settingsService.GetDefaultProximityRadius();
TimeSpan startTime = _settingsService.GetDefaultStartTime();
```

#### Writing Settings

```csharp
// Update a setting
await _settingsService.SetValueAsync(
    SettingsKeys.DefaultProximityRadius, 750);

// Or modify through the UI (automatic save)
var setting = await _settingsService.GetSettingAsync(
    SettingsKeys.DefaultProximityRadius);
setting.Value = "750";
await _settingsService.SaveSettingAsync(setting);
```

### Settings Service API

```csharp
public class SettingsService
{
    // Initialize with defaults
    Task InitializeDefaultSettingsAsync();
    
    // CRUD operations
    Task<AppSettings?> GetSettingAsync(string key);
    Task<List<AppSettings>> GetAllSettingsAsync();
    Task<List<AppSettings>> GetSettingsByCategoryAsync(string category);
    Task SaveSettingAsync(AppSettings setting);
    
    // Type-safe access
    Task<T> GetValueAsync<T>(string key, T defaultValue);
    Task SetValueAsync<T>(string key, T value);
    
    // Reset functionality
    Task ResetToDefaultsAsync();
    Task ResetCategoryToDefaultsAsync(string category);
    
    // Convenience methods for common settings
    int GetDefaultProximityRadius();
    int GetDefaultCheckInterval();
    int GetDefaultMinutesThreshold();
    TimeSpan GetDefaultStartTime();
    TimeSpan GetDefaultEndTime();
}
```

## UI Features

### Settings Page

The Settings page provides:

1. **Grouped Display**: Settings organized by category
2. **Type-Appropriate Controls**:
   - Text/Number inputs for strings and integers
   - Switches for booleans
   - Time pickers for times
   - Sliders for numeric ranges
3. **Live Validation**: Min/max enforcement
4. **Auto-Save**: Changes saved immediately
5. **Reset Options**:
   - Reset individual category
   - Reset all settings to factory defaults

### Adding Custom Settings

To add a new setting:

1. **Add constant to SettingsKeys**:
```csharp
public const string MyNewSetting = "MyNewSetting";
```

2. **Add to InitializeDefaultSettingsAsync**:
```csharp
new() { 
    Key = SettingsKeys.MyNewSetting, 
    Value = "default", 
    Category = SettingsCategories.Advanced,
    DisplayName = "My Setting", 
    Description = "What this setting does",
    DataType = "int", 
    MinValue = "0", 
    MaxValue = "100", 
    DefaultValue = "50" 
}
```

3. **Use in code**:
```csharp
int value = await _settingsService.GetValueAsync(
    SettingsKeys.MyNewSetting, 50);
```

## Integration with Notification Schedules

When creating a notification schedule, defaults are pulled from settings:

```csharp
var schedule = new NotificationSchedule
{
    StopCode = stop.StopCode,
    StopName = stop.Name,
    StartTime = _settingsService.GetDefaultStartTime(),
    EndTime = _settingsService.GetDefaultEndTime(),
    ProximityRadius = _settingsService.GetDefaultProximityRadius(),
    CheckIntervalSeconds = _settingsService.GetDefaultCheckInterval(),
    MinMinutesThreshold = _settingsService.GetDefaultMinutesThreshold()
};
```

Users can:
1. Set their preferred defaults once in Settings
2. Create schedules with one tap (uses defaults)
3. Customize individual schedules as needed

## Performance Considerations

- **Caching**: Settings are cached in-memory after first load
- **Minimal DB Hits**: Cached values used for reads
- **Batch Updates**: UI changes debounced where appropriate
- **Lazy Loading**: Settings loaded on-demand per category

## Battery Optimization Modes

### Aggressive
- Longer check intervals (10 min)
- Larger proximity radius (reduce GPS checks)
- Lower API timeout (fail faster)
- Disable auto-refresh

### Balanced (Default)
- Standard intervals (5 min)
- Standard radius (500m)
- Normal timeouts

### Performance
- Faster checks (2 min)
- Smaller radius (more precise)
- Longer timeouts (better reliability)
- Aggressive auto-refresh

## Settings Migration

When updating the app, new settings are automatically added:

```csharp
// On app start
await _settingsService.InitializeDefaultSettingsAsync();

// This checks for missing settings and adds them
// Existing settings are preserved
```

## Export/Import Settings (Future)

Planned features:
- Export settings to JSON file
- Import settings from file
- Sync settings across devices
- Backup/restore configuration

## Troubleshooting

### Settings Not Saving
- Check database permissions
- Verify SettingsService is registered as Singleton
- Check for exceptions in debug output

### Settings Not Appearing in UI
- Ensure category is valid
- Check DataType matches expected control
- Verify converters are registered in App.xaml

### Default Values Not Working
- Check DefaultValue is set in database
- Verify ResetToDefaultsAsync is called
- Check data type conversion logic

## Example: Custom Configuration Profile

Create a "Work Commute" preset:

```csharp
public async Task ApplyWorkCommutePresetAsync()
{
    await _settingsService.SetValueAsync(
        SettingsKeys.DefaultStartTime, "17:30");
    await _settingsService.SetValueAsync(
        SettingsKeys.DefaultEndTime, "18:30");
    await _settingsService.SetValueAsync(
        SettingsKeys.DefaultCheckInterval, 300); // 5 min
    await _settingsService.SetValueAsync(
        SettingsKeys.DefaultProximityRadius, 400); // 400m
    await _settingsService.SetValueAsync(
        SettingsKeys.DefaultMinutesThreshold, 8); // 8 min warning
}
```

## Summary

The settings system provides:

? **Centralized Configuration**: All app settings in one place  
? **User-Friendly UI**: Intuitive controls for all data types  
? **Type Safety**: Generic `GetValueAsync<T>` prevents errors  
? **Persistence**: SQLite-backed storage  
? **Performance**: In-memory caching  
? **Flexibility**: Easy to extend with new settings  
? **Reset Options**: Category and global reset  
? **Defaults Integration**: New schedules use configured defaults  

This makes the app highly customizable without requiring code changes!
