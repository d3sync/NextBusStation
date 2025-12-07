# Settings Quick Reference

## Accessing Settings

Tap the **Settings** tab at the bottom of the app.

## Key Settings to Configure

### For Daily Commute

1. **Notification Defaults** section:
   - Set **Default Start Time** to your commute start (e.g., 17:30)
   - Set **Default End Time** to your commute end (e.g., 18:30)
   - Set **Default Proximity Radius** to match your walking distance (e.g., 400m)
   - Set **Default Check Interval** to 5 minutes (300 seconds)
   - Set **Default Minutes Threshold** to how early you want warnings (e.g., 10 min)

2. **Notification Behavior**:
   - Enable **Vibrate** for silent alerts
   - Enable **Sound** if you want audio notifications
   - Set **Priority** to High for important notifications

### For Battery Life

1. **Performance & Battery**:
   - Set **Battery Mode** to "Aggressive"
   - This will:
     - Increase check intervals
     - Reduce GPS polling
     - Lower API frequency

2. **API Settings**:
   - Increase **Refresh Interval** to 60s (less frequent updates)
   - Increase **Cache Expiration** to 120 min (reuse data longer)

### For Best Experience

1. **Appearance**:
   - Enable **Use English Descriptions** (clearer for non-Greek speakers)
   - Enable **Show Stop Codes** (helpful for identification)
   - Set **Theme** to your preference

2. **Map Settings**:
   - Set **Max Nearby Stops** to 15-20 (good balance)
   - Set **Search Radius** to 1000m (shows stops you might walk to)

## Common Configurations

### "Quick Glance" Profile
```
Check Interval: 120s (2 min)
Refresh Interval: 20s
Minutes Threshold: 5 min
Priority: High
Battery Mode: Performance
```
Perfect for: When you need real-time updates and battery isn't a concern.

### "Battery Saver" Profile
```
Check Interval: 600s (10 min)
Refresh Interval: 90s
Minutes Threshold: 15 min
Priority: Default
Battery Mode: Aggressive
WiFi Only: Enabled
```
Perfect for: Long days when you need to preserve battery.

### "Work Commute" Profile (Default)
```
Start Time: 17:40
End Time: 18:25
Check Interval: 300s (5 min)
Proximity: 500m
Minutes Threshold: 10 min
Days: Mon-Fri
```
Perfect for: Your PowerShell script replacement!

## Reset Options

### Reset a Category
- Tap the **Reset** button next to any category header
- Only that category's settings return to defaults
- Other settings unchanged

### Reset All Settings
- Tap **Reset All** button at bottom
- **Warning**: This resets EVERYTHING to factory defaults
- Cannot be undone
- Use sparingly!

## Tips

1. **Start with Defaults**: The default values are optimized for most use cases
2. **Adjust Gradually**: Change one setting at a time to see its effect
3. **Monitor Battery**: Check Settings ? Battery ? App Usage after config changes
4. **Test Notifications**: Create a test schedule to verify settings work as expected
5. **Sync Across Devices**: Export/import coming soon (planned feature)

## Setting Descriptions

### Data Types

- **Integer (slider)**: Drag slider or see current value below
- **Boolean (switch)**: Toggle on/off
- **String (text)**: Type value directly
- **Time**: Use time picker to select

### Validation

- Settings enforce min/max ranges automatically
- Invalid values won't save
- Helpful descriptions explain each setting

## Troubleshooting

**Q: Settings don't seem to apply?**
- Tap **Reload** button
- Restart monitoring service
- Check that setting value is within valid range

**Q: Lost my custom configuration?**
- Unfortunately, resets cannot be undone
- Future version will include backup/restore

**Q: Want different defaults for different stops?**
- Settings apply to NEW schedules only
- Existing schedules retain their configuration
- Edit individual schedules from the Schedules tab

## Advanced Usage

### For Developers

Access settings programmatically:

```csharp
// In any ViewModel or Service:
var settingsService = Handler.MauiContext.Services
    .GetService<SettingsService>();

// Read
int radius = await settingsService.GetValueAsync(
    SettingsKeys.DefaultProximityRadius, 500);

// Write
await settingsService.SetValueAsync(
    SettingsKeys.DefaultProximityRadius, 750);

// Use convenience methods
int radius = settingsService.GetDefaultProximityRadius();
```

### Database Location

Settings stored at:
```
{AppDataDirectory}/nextbusstation.db3
```

Table: `app_settings`

### Backup Strategy (Manual)

1. Export database file from device
2. Store somewhere safe
3. Restore by copying back

## Summary

The settings system gives you complete control over:

- ?? Notification behavior
- ?? Battery usage  
- ?? Network usage
- ?? Appearance
- ? Performance
- ?? Transit defaults

Configure once, enjoy forever!
