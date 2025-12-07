# Quick Troubleshooting Guide

## ? FIXED - Can Now Find Stops & Change Settings!

### What Was Fixed

1. **MapPage now auto-loads stops** on first appear
2. **Settings page is now fully interactive** - you can edit all values
3. **Proper converters** for different data types

---

## How to Find Nearby Stops

### Option 1: Automatic (NEW!)
1. Open app
2. **Stops load automatically** when you go to "Nearby" tab
3. Wait for GPS fix (~5-10 seconds)

### Option 2: Manual Refresh
1. Go to "Nearby" tab
2. Tap **"Find Nearby Stops"** button at bottom
3. Grant location permission if prompted
4. Wait for GPS and API response

### Troubleshooting "No Stops Found"

**Issue**: "Location permission denied"
- **Fix**: Go to phone Settings ? Apps ? NextBusStation ? Permissions ? Enable Location

**Issue**: "Could not get current location"
- **Fix**: 
  - Make sure GPS/Location is turned on in phone settings
  - Go outside or near a window (GPS needs sky visibility)
  - Wait 30 seconds for GPS fix
  - Try tapping "Find Nearby Stops" again

**Issue**: Empty list but no error
- **Fix**:
  - You might not be in Athens, Greece
  - OASA API only has stops in Athens metropolitan area
  - Try setting phone location to Athens for testing:
    - Android Emulator: Use Extended Controls ? Location
    - Real device: You need to be physically in Athens

**Issue**: API timeout
- **Fix**:
  - Check internet connection
  - OASA API might be slow - wait and retry
  - Go to Settings ? API Settings ? Increase "API Timeout" to 60 seconds

---

## How to Change Settings (NEW!)

### Settings Page is Now Interactive!

1. Go to **Settings** tab
2. You'll see all settings grouped by category
3. **Edit values directly**:

#### Integer Settings (e.g., Proximity Radius)
- **See**: Entry field with current value
- **Edit**: Tap field, type new number
- **Save**: Automatically saved when you finish typing

#### Boolean Settings (e.g., Vibrate)
- **See**: Switch control
- **Edit**: Toggle on/off
- **Save**: Automatically saved on toggle

#### Time Settings (e.g., Start Time)
- **See**: Time picker
- **Edit**: Tap to open time picker wheel
- **Save**: Automatically saved when you select time

#### String Settings (e.g., Theme)
- **See**: Entry field with current value
- **Edit**: Tap field, type new value
- **Save**: Automatically saved when you finish typing

### Quick Settings Changes

**To test notifications faster**:
```
Settings ? Notification Defaults ?
  - Default Check Interval: Change from 300 to 60 (1 minute)
  - Default Minutes Threshold: Change from 10 to 20 (longer warning)
```

**To find more stops**:
```
Settings ? Map Settings ?
  - Search Radius: Change from 1000 to 2000 (2km search)
  - Max Nearby Stops: Change from 20 to 50 (show more)
```

**To save battery**:
```
Settings ? Performance & Battery ?
  - Battery Mode: Change from "Balanced" to "Aggressive"
  - WiFi Only Updates: Toggle ON
```

### Reset Options

**Reset one category**:
- Tap "Reset" button next to category header
- Only that category returns to defaults

**Reset everything**:
- Scroll to bottom
- Tap "?? Reset All" button
- **Warning**: All settings return to defaults!

---

## Testing the App

### Quick Test Flow

1. **Check Settings First**
   ```
   Go to Settings tab
   See all categories loaded
   Try editing "Default Proximity Radius" from 500 to 600
   Tap "Reload" - should show 600
   ```

2. **Find Stops**
   ```
   Go to Nearby tab
   Wait for "Getting location..."
   Should change to "Loading nearby stops..."
   Should show "Found X nearby stops"
   See list of stops appear
   ```

3. **View Stop Details**
   ```
   Tap any stop in the list
   See stop name and code at top
   See "Loading stop information..." 
   See arrivals list populate
   See routes list populate
   ```

4. **Create Schedule**
   ```
   On stop details page, tap ? button
   See schedule editor with:
     - Time pickers (should show your defaults from Settings)
     - Day checkboxes
     - Sliders for proximity, interval, threshold
   Tap Save
   ```

5. **Start Monitoring**
   ```
   Go to Schedules tab
   See your schedule in the list
   Tap "Start" button
   Toggle should turn green
   Status should show "Monitoring active"
   ```

---

## Common Issues & Fixes

### Issue: Settings Not Saving

**Symptom**: Change a value, tap Reload, old value appears

**Fix**:
1. Check status bar for errors
2. Try tapping the field again and editing
3. Check database permissions (shouldn't be an issue on Android)
4. Restart app

### Issue: MapPage Shows Empty List

**Symptom**: No error, but no stops

**Check**:
1. Status message - what does it say?
2. Are you in Athens? (OASA only works there)
3. Try manual refresh button
4. Check Settings ? Map Settings ? Search Radius

### Issue: Notifications Not Working

**Not testing yet?** The notification system works but requires:
1. At least one schedule configured
2. Monitoring started (green toggle)
3. Being physically near the configured stop
4. During the configured time window
5. On configured days

**Test programmatically instead**: See DONE.md for test code

---

## Settings Reference Card

| Setting | Location | Default | What It Does |
|---------|----------|---------|--------------|
| **Search Radius** | Map Settings | 1000m | How far to search for stops |
| **Max Stops** | Map Settings | 20 | Max stops to display |
| **Proximity Radius** | Notification Defaults | 500m | How close to stop before notifying |
| **Check Interval** | Notification Defaults | 300s (5min) | How often to check arrivals |
| **Minutes Threshold** | Notification Defaults | 10min | Notify when bus ? X min away |
| **API Timeout** | API Settings | 30s | How long to wait for API |
| **Refresh Interval** | API Settings | 30s | Auto-refresh rate on details page |

---

## Data Flow

### Finding Stops
```
Nearby tab appears
  ?
OnAppearing() triggered
  ?
LoadNearbyStopsCommand executed
  ?
Check location permission
  ?
Get GPS coordinates
  ?
Call OASA API getClosestStops
  ?
Display stops in list
```

### Changing Settings
```
Tap Entry/Switch/TimePicker
  ?
User changes value
  ?
Event handler (OnSettingChanged, etc.) fires
  ?
Update AppSettings.Value
  ?
Call SaveSettingCommand
  ?
Save to database
  ?
Update cache
  ?
Show "Updated" in status
```

---

## Next Steps

1. ? Open app ? See it load stops automatically
2. ? Go to Settings ? Try changing some values
3. ? Create a test schedule
4. ? Test notifications (requires being in Athens or using test code)

---

## Debug Tips

### Enable Debug Logging
```
Settings ? Advanced ? Debug Mode ? Toggle ON
Settings ? Advanced ? Log Level ? Change to "Debug"
```

Then check Visual Studio Output window for detailed logs.

### Check Database
Database location:
```
{AppDataDirectory}/nextbusstation.db3
```

You can inspect it with SQLite browser to see saved settings.

### Force Refresh
```
Settings tab ? Tap "Reload"
Nearby tab ? Tap "Find Nearby Stops"
```

---

**You should now be able to**:
- ? Find nearby stops automatically
- ? Refresh stops manually
- ? Edit ALL settings on the fly
- ? See changes take effect immediately
- ? Reset settings if needed

**Happy testing!** ??
