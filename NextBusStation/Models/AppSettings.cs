using SQLite;

namespace NextBusStation.Models;

[Table("app_settings")]
public class AppSettings
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Key { get; set; } = string.Empty;
    
    public string Value { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty;
    
    public string DisplayName { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string DataType { get; set; } = "string";
    
    public string? MinValue { get; set; }
    
    public string? MaxValue { get; set; }
    
    public string? DefaultValue { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public static class SettingsKeys
{
    public const string DefaultProximityRadius = "DefaultProximityRadius";
    public const string DefaultCheckInterval = "DefaultCheckInterval";
    public const string DefaultMinutesThreshold = "DefaultMinutesThreshold";
    public const string MaxBusesInNotification = "MaxBusesInNotification";
    public const string DefaultStartTime = "DefaultStartTime";
    public const string DefaultEndTime = "DefaultEndTime";
    
    public const string MapDefaultZoomLevel = "MapDefaultZoomLevel";
    public const string MaxNearbyStops = "MaxNearbyStops";
    public const string SearchRadius = "SearchRadius";
    
    public const string RefreshIntervalSeconds = "RefreshIntervalSeconds";
    public const string ApiTimeoutSeconds = "ApiTimeoutSeconds";
    public const string CacheExpirationMinutes = "CacheExpirationMinutes";
    
    public const string NotificationVibrate = "NotificationVibrate";
    public const string NotificationSound = "NotificationSound";
    public const string NotificationLedColor = "NotificationLedColor";
    public const string NotificationPriority = "NotificationPriority";
    
    public const string LanguagePreference = "LanguagePreference";
    public const string ThemePreference = "ThemePreference";
    public const string UseEnglishDescriptions = "UseEnglishDescriptions";
    
    public const string BackgroundMonitoringEnabled = "BackgroundMonitoringEnabled";
    public const string AutoStartMonitoring = "AutoStartMonitoring";
    public const string BatteryOptimizationMode = "BatteryOptimizationMode";
    public const string WifiOnlyUpdates = "WifiOnlyUpdates";
    
    public const string DebugMode = "DebugMode";
    public const string ShowDebugFeatures = "ShowDebugFeatures";
    public const string LogLevel = "LogLevel";
    public const string ShowStopCodes = "ShowStopCodes";
}

public static class SettingsCategories
{
    public const string NotificationDefaults = "Notification Defaults";
    public const string MapSettings = "Map Settings";
    public const string ApiSettings = "API Settings";
    public const string NotificationBehavior = "Notification Behavior";
    public const string Appearance = "Appearance";
    public const string Performance = "Performance & Battery";
    public const string Advanced = "Advanced";
}
