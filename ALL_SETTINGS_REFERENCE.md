# All Available Settings - Complete Reference

## Quick Access Chart

| Setting Key | Category | Data Type | Default | Range/Options | Purpose |
|-------------|----------|-----------|---------|---------------|---------|
| **DefaultProximityRadius** | Notification Defaults | int | 500 | 100-2000 | Default radius in meters for new schedules |
| **DefaultCheckInterval** | Notification Defaults | int | 300 | 60-1800 | How often to check for buses (seconds) |
| **DefaultMinutesThreshold** | Notification Defaults | int | 10 | 5-30 | Notify when bus is within X minutes |
| **DefaultStartTime** | Notification Defaults | time | 17:40 | any | Default monitoring start time |
| **DefaultEndTime** | Notification Defaults | time | 18:25 | any | Default monitoring end time |
| **MaxNearbyStops** | Map Settings | int | 20 | 5-50 | Maximum stops to display |
| **SearchRadius** | Map Settings | int | 1000 | 500-5000 | Search radius for nearby stops (meters) |
| **RefreshIntervalSeconds** | API Settings | int | 30 | 15-120 | Auto-refresh interval on details page (seconds) |
| **ApiTimeoutSeconds** | API Settings | int | 30 | 10-60 | HTTP request timeout (seconds) |
| **CacheExpirationMinutes** | API Settings | int | 60 | 15-240 | How long to cache stop data (minutes) |
| **NotificationVibrate** | Notification Behavior | bool | true | on/off | Vibrate when notification arrives |
| **NotificationSound** | Notification Behavior | bool | true | on/off | Play sound on notification |
| **NotificationPriority** | Notification Behavior | string | High | Low/Default/High | Android notification priority |
| **UseEnglishDescriptions** | Appearance | bool | true | on/off | Show English stop/route names when available |
| **ThemePreference** | Appearance | string | System | Light/Dark/System | App theme |
| **ShowStopCodes** | Appearance | bool | true | on/off | Display stop codes in lists |
| **BatteryOptimizationMode** | Performance | string | Balanced | Aggressive/Balanced/Performance | Performance vs battery tradeoff |
| **WifiOnlyUpdates** | Performance | bool | false | on/off | Only check for updates on WiFi |
| **DebugMode** | Advanced | bool | false | on/off | Enable debug logging |
| **LogLevel** | Advanced | string | Info | Trace/Debug/Info/Warning/Error | Logging detail level |

## Detailed Descriptions

### Notification Defaults Category

#### DefaultProximityRadius
- **What it is**: The default distance (in meters) from a bus stop within which you must be to receive notifications
- **When to change**: 
  - Increase (750-1000m) if you walk slowly or want earlier warnings
  - Decrease (300-400m) if you want notifications only when very close
- **Example**: Set to 400m if you're a fast walker and only want alerts when you're almost at the stop

#### DefaultCheckInterval
- **What it is**: How often the app checks the OASA API for new bus arrivals (in seconds)
- **When to change**:
  - Decrease (120-180s) for more real-time updates (uses more battery)
  - Increase (600-900s) to save battery
- **Example**: Set to 600s (10 min) on battery saver mode; 120s (2 min) when you need real-time tracking

#### DefaultMinutesThreshold
- **What it is**: Only notify when a bus is arriving within this many minutes
- **When to change**:
  - Increase (15-20min) if you want early warnings and can wait comfortably
  - Decrease (5-7min) if you want last-minute notifications
- **Example**: Set to 8min if that's how long it takes you to walk to the stop

#### DefaultStartTime / DefaultEndTime
- **What they are**: The time window during which monitoring is active
- **When to change**: Match your actual commute times
- **Example**: 
  - Morning commute: 07:30-08:15
  - Evening commute: 17:40-18:25
  - Flexible schedule: 06:00-22:00

### Map Settings Category

#### MaxNearbyStops
- **What it is**: Maximum number of stops to show in the nearby stops list
- **When to change**:
  - Increase (30-50) if you're in a dense area with many options
  - Decrease (10-15) to reduce clutter
- **Example**: Set to 10 if you only care about your regular stops

#### SearchRadius
- **What it is**: How far to search for bus stops from your current location (meters)
- **When to change**:
  - Increase (2000-5000m) to see stops you might drive/bike to
  - Decrease (500-800m) to only see walking distance stops
- **Example**: Set to 1500m if you're willing to walk up to 20 minutes

### API Settings Category

#### RefreshIntervalSeconds
- **What it is**: How often the stop details page auto-refreshes arrivals
- **When to change**:
  - Decrease (15-20s) when actively waiting for a bus
  - Increase (60-90s) to save battery/data
- **Example**: Set to 20s when you're at the stop watching for your bus

#### ApiTimeoutSeconds
- **What it is**: How long to wait for OASA API before giving up
- **When to change**:
  - Increase (45-60s) if you have slow/unreliable internet
  - Decrease (15-20s) if you want faster failure on bad connections
- **Example**: Set to 15s if you'd rather see "timeout" quickly than wait

#### CacheExpirationMinutes
- **What it is**: How long to reuse cached stop/route data before fetching fresh
- **When to change**:
  - Increase (120-240min) to reduce API calls (stop data rarely changes)
  - Decrease (30min) if you want the freshest possible data
- **Example**: Set to 180min (3 hours) - stop locations don't change often!

### Notification Behavior Category

#### NotificationVibrate
- **What it is**: Whether to vibrate your phone when a notification arrives
- **When to change**: Disable if you find vibration annoying; keep on for silent alerts
- **Example**: Enable for meetings/quiet environments where you can't have sound

#### NotificationSound
- **What it is**: Whether to play a sound with notifications
- **When to change**: Disable in quiet environments; enable if you might miss vibration
- **Example**: Disable at work, enable at home

#### NotificationPriority
- **What it is**: Android notification importance level
- **When to change**:
  - **High**: Pops up on screen, bypasses Do Not Disturb
  - **Default**: Shows in notification shade
  - **Low**: Minimal/silent notification
- **Example**: Use High for critical commute times, Default otherwise

### Appearance Category

#### UseEnglishDescriptions
- **What it is**: Prefer English names for stops/routes when available
- **When to change**: Disable if you prefer Greek text
- **Example**: Enable if you don't read Greek

#### ThemePreference
- **What it is**: App color scheme
- **When to change**: Personal preference
- **Options**:
  - **Light**: Always light theme
  - **Dark**: Always dark theme
  - **System**: Follow phone's theme setting (recommended)

#### ShowStopCodes
- **What it is**: Display OASA stop codes (e.g., "030019") in lists
- **When to change**: Disable if codes seem cluttered; keep on for identification
- **Example**: Enable to quickly identify stops by their official code

### Performance & Battery Category

#### BatteryOptimizationMode
- **What it is**: Preset that adjusts multiple settings for battery vs performance
- **When to change**: Based on current battery level and urgency
- **Options**:
  - **Aggressive**: Maximum battery savings
    - Check interval: 600s (10 min)
    - API timeout: 20s
    - Reduced GPS polling
    - ~1% battery per day
  - **Balanced**: Good compromise (default)
    - Check interval: 300s (5 min)
    - Standard settings
    - ~2% battery per day
  - **Performance**: Real-time updates
    - Check interval: 120s (2 min)
    - Longer timeouts
    - Aggressive refresh
    - ~4% battery per day

#### WifiOnlyUpdates
- **What it is**: Only fetch API updates when connected to WiFi
- **When to change**: Enable to save mobile data; disable for full functionality
- **Example**: Enable if you have a limited data plan

### Advanced Category

#### DebugMode
- **What it is**: Enable detailed debug logging
- **When to change**: Enable when troubleshooting issues; keep off normally
- **Side effects**: Slight performance impact, larger log files

#### LogLevel
- **What it is**: How detailed the logs should be
- **When to change**: Lower (Trace/Debug) for troubleshooting; higher (Warning/Error) for production
- **Options**: Trace > Debug > Info > Warning > Error (from most to least verbose)

## Recommended Configurations

### Configuration 1: "Morning Rush"
```
Purpose: Get to work on time with minimal battery usage
DefaultProximityRadius: 300m (close to stop only)
DefaultCheckInterval: 180s (3 min - frequent enough)
DefaultMinutesThreshold: 5min (last-minute warnings)
DefaultStartTime: 07:30
DefaultEndTime: 08:15
RefreshIntervalSeconds: 25s
BatteryOptimizationMode: Balanced
NotificationPriority: High
```

### Configuration 2: "Evening Commute" (Your Use Case)
```
Purpose: Match your PowerShell script exactly
DefaultProximityRadius: 500m
DefaultCheckInterval: 300s (5 min)
DefaultMinutesThreshold: 10min
DefaultStartTime: 17:40
DefaultEndTime: 18:25
RefreshIntervalSeconds: 30s
BatteryOptimizationMode: Balanced
NotificationPriority: High
NotificationVibrate: true
```

### Configuration 3: "Battery Saver"
```
Purpose: All-day monitoring with minimal battery drain
DefaultProximityRadius: 600m (larger radius, fewer checks)
DefaultCheckInterval: 600s (10 min)
DefaultMinutesThreshold: 15min
RefreshIntervalSeconds: 90s
BatteryOptimizationMode: Aggressive
ApiTimeoutSeconds: 20s
CacheExpirationMinutes: 180min
WifiOnlyUpdates: true
```

### Configuration 4: "Real-Time Tracker"
```
Purpose: Maximum accuracy, battery not a concern
DefaultProximityRadius: 400m
DefaultCheckInterval: 120s (2 min)
DefaultMinutesThreshold: 8min
RefreshIntervalSeconds: 15s
BatteryOptimizationMode: Performance
ApiTimeoutSeconds: 45s
NotificationPriority: High
```

### Configuration 5: "Weekend Explorer"
```
Purpose: Discovering new routes without strict timing
DefaultProximityRadius: 800m
DefaultCheckInterval: 600s
DefaultMinutesThreshold: 15min
DefaultStartTime: 09:00
DefaultEndTime: 20:00
MaxNearbyStops: 30
SearchRadius: 2000m
BatteryOptimizationMode: Balanced
```

## How to Apply a Configuration

### Method 1: Manual (via UI)
1. Open **Settings** tab
2. Navigate to each category
3. Change values to match desired configuration
4. Values save automatically

### Method 2: Code (for developers)
```csharp
// Apply "Evening Commute" preset
public async Task ApplyEveningCommutePresetAsync()
{
    await _settingsService.SetValueAsync(SettingsKeys.DefaultProximityRadius, 500);
    await _settingsService.SetValueAsync(SettingsKeys.DefaultCheckInterval, 300);
    await _settingsService.SetValueAsync(SettingsKeys.DefaultMinutesThreshold, 10);
    await _settingsService.SetValueAsync(SettingsKeys.DefaultStartTime, "17:40");
    await _settingsService.SetValueAsync(SettingsKeys.DefaultEndTime, "18:25");
    await _settingsService.SetValueAsync(SettingsKeys.RefreshIntervalSeconds, 30);
    await _settingsService.SetValueAsync(SettingsKeys.BatteryOptimizationMode, "Balanced");
    await _settingsService.SetValueAsync(SettingsKeys.NotificationPriority, "High");
    await _settingsService.SetValueAsync(SettingsKeys.NotificationVibrate, true);
}
```

## Tips for Optimization

1. **Start Conservative**: Begin with Balanced mode and adjust based on results
2. **Monitor Battery**: Check Settings ? Battery ? App Usage after a day
3. **Adjust Gradually**: Change one setting at a time to isolate effects
4. **Test Thoroughly**: Create a test schedule before committing to configuration
5. **Use Presets**: Try each battery mode to find your sweet spot
6. **Consider Context**: Different configs for weekday commute vs weekend exploration
7. **Reset When Lost**: Use category/global reset if you get confused

## Troubleshooting by Setting

| Problem | Likely Setting | Solution |
|---------|---------------|----------|
| Too many notifications | MinutesThreshold too high | Decrease to 5-8 min |
| Missing notifications | ProximityRadius too small | Increase to 600-800m |
| Battery draining fast | CheckInterval too low | Increase to 600s or use Aggressive mode |
| Notifications too late | MinutesThreshold too low | Increase to 12-15 min |
| Slow app performance | RefreshInterval too low | Increase to 45-60s |
| Missing nearby stops | SearchRadius too small | Increase to 1500-2000m |
| Too cluttered UI | MaxNearbyStops too high | Decrease to 10-15 |
| API timeouts | ApiTimeout too low | Increase to 45-60s |
| Stale data | CacheExpiration too high | Decrease to 30-45 min |

Your app is now fully configurable! ??
