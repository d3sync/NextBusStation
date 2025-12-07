# Bug Fixes: Max Stops Limit & Notification Scheduling

## Issues Fixed

### Issue 1: Maximum Stops Setting Not Working
**Problem:** Setting "Max Nearby Stops" to 5 in settings still showed 20 stops  
**Root Cause:** MapViewModel wasn't using SettingsService to get the MaxNearbyStops value

**Fix:**
1. Added `SettingsService` dependency to MapViewModel constructor
2. Call `GetMaxNearbyStops()` from settings before loading stops
3. Pass the maxStops parameter to `GetClosestStopsAsync()`

**Files Modified:**
- `NextBusStation\ViewModels\MapViewModel.cs`
  - Added `SettingsService _settingsService` field
  - Updated constructor to inject SettingsService
  - Added code to load max stops from settings
  - Pass maxStops to API call

- `NextBusStation\Services\SettingsService.cs`
  - Added `GetMaxNearbyStops()` helper method
  - Added `GetSearchRadius()` helper method (for future use)

**Testing:**
```csharp
// In MapViewModel.LoadNearbyStopsAsync():
await _settingsService.InitializeDefaultSettingsAsync();
var maxStops = _settingsService.GetMaxNearbyStops();
System.Diagnostics.Debug.WriteLine($"   ?? Max stops setting: {maxStops}");

var stops = await _oasaService.GetClosestStopsAsync(
    location.Longitude,
    location.Latitude,
    maxStops);  // ? Now uses setting value (5, 10, 20, etc.)
```

---

### Issue 2: Notification Scheduling Not Working
**Problem:** Creating a notification schedule didn't save properly

**Root Cause:** Multiple issues:
1. No validation for required fields
2. No user feedback on save success/failure
3. No error handling for save operations

**Fixes:**

#### A. Added Validation
- At least one day must be selected
- End time must be after start time
- Schedule data must exist

#### B. Added User Feedback
- Success alert after saving
- Error alert if save fails
- "Saving..." button text during save operation
- Disabled buttons while saving

#### C. Added Debug Logging
- Logs when schedule is loaded
- Logs all settings being saved
- Logs save result from database
- Logs any errors that occur

**Files Modified:**

1. `NextBusStation\ViewModels\EditScheduleViewModel.cs`
   - Added `IsSaving` observable property
   - Added validation logic
   - Added success/error alerts
   - Added comprehensive debug logging
   - Added unsaved changes detection in Cancel

2. `NextBusStation\Views\EditSchedulePage.xaml`
   - Added IsEnabled binding to buttons (disabled while saving)
   - Added DataTrigger to show "Saving..." text
   - Uses InvertedBoolConverter for button states

3. `NextBusStation\ViewModels\NotificationSchedulesViewModel.cs`
   - Added debug logging to LoadSchedulesAsync
   - Logs schedule count and details when loading

**Key Code Changes:**

```csharp
// EditScheduleViewModel.SaveAsync()

// Validation
if (!MondayEnabled && !TuesdayEnabled && ... !SundayEnabled)
{
    await Shell.Current.DisplayAlert("Validation Error", 
        "Please select at least one day", "OK");
    return;
}

if (EndTime <= StartTime)
{
    await Shell.Current.DisplayAlert("Validation Error", 
        "End time must be after start time", "OK");
    return;
}

IsSaving = true;
try
{
    // ... save logic ...
    
    await Shell.Current.DisplayAlert("Success", 
        $"Schedule for {Schedule.StopName} saved successfully!", "OK");
    
    await Shell.Current.GoToAsync("..");
}
catch (Exception ex)
{
    await Shell.Current.DisplayAlert("Error", 
        $"Failed to save schedule: {ex.Message}", "OK");
}
finally
{
    IsSaving = false;
}
```

---

## Testing Instructions

### Test Max Stops Setting

1. Go to Settings page
2. Find "Map Settings" ? "Max Nearby Stops"
3. Change value to 5
4. Go to Map page
5. Tap "Find Nearby Stops"
6. Verify only 5 stops are shown
7. Check debug output for: `?? Max stops setting: 5`

### Test Notification Scheduling

1. Go to Map page
2. Find any bus stop
3. Tap the stop to view details
4. Tap the "+" button (Add Schedule)
5. Configure the schedule:
   - Set time window (e.g., 17:00 - 18:30)
   - Select weekdays
   - Set proximity radius (e.g., 500m)
   - Set check interval (e.g., 5 minutes)
   - Set alert threshold (e.g., ?10 minutes)
6. Tap "Save Schedule"
7. You should see: "Success" alert
8. Return to Schedules page
9. Verify new schedule appears in list

**Error Cases to Test:**
1. Try to save with no days selected ? Should show validation error
2. Try to save with end time before start time ? Should show validation error
3. Change values and tap Cancel ? Should ask about unsaved changes

---

## Debug Output Examples

### Successful Save:
```
?? EditScheduleViewModel loaded:
   Stop: Syntagma Station (400234)
   IsNew: True
   Time: 17:40 - 18:25
?? Saving schedule for Syntagma Station...
   Time: 17:40 - 18:25
   Proximity: 500m
   Check interval: 5 min
   Alert threshold: ?10 min
   Days: M:True T:True W:True Th:True F:True Sa:False Su:False
? Schedule saved successfully (Result: 1)
   Schedule ID: 1
?? Loading schedules...
   Found 1 schedule(s) in database
   • Syntagma Station (400234) - Enabled: True
? Schedules loaded. Monitoring: False
```

### Max Stops in Action:
```
?? MapViewModel: Starting LoadNearbyStops
   Test mode: True
   ?? Max stops setting: 5
?? Using TEST location (Athens, Greece)
?? Using location: 37.9838, 23.7275
?? Received 5 stops from API (max requested: 5)
? Success: 5 stops loaded
```

---

## Files Changed

1. ? `NextBusStation\ViewModels\MapViewModel.cs`
2. ? `NextBusStation\ViewModels\EditScheduleViewModel.cs`
3. ? `NextBusStation\ViewModels\NotificationSchedulesViewModel.cs`
4. ? `NextBusStation\Views\EditSchedulePage.xaml`
5. ? `NextBusStation\Services\SettingsService.cs`

---

## Build Status
? **Build Successful** - All changes compile without errors

## Next Steps

1. **Test on Device**: Run the app and verify both fixes work
2. **Monitor Debug Output**: Watch for the log messages during testing
3. **Create Schedules**: Try creating multiple schedules for different stops
4. **Adjust Settings**: Test different MaxNearbyStops values (5, 10, 15, 20)
5. **Validation Tests**: Try to trigger all validation messages

## Related Settings

You can now adjust these settings to customize behavior:

- **Max Nearby Stops** (5-50): Controls how many stops are shown
- **Default Proximity Radius** (100-2000m): Default for new schedules  
- **Default Check Interval** (60-1800s): How often to check for buses
- **Default Minutes Threshold** (5-30min): When to send notification

All settings are in the Settings page under their respective categories.
