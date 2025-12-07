# Quick Settings Examples - Copy & Paste Values

## Preset Configurations

### ?? "Aggressive Finder" - Find ALL nearby stops
```
Go to Settings ? Map Settings:
- Search Radius: 2000
- Max Nearby Stops: 50

Then tap Nearby ? Find Stops
```

### ? "Fast Refresh" - Real-time updates
```
Go to Settings ? API Settings:
- Refresh Interval: 15
- API Timeout: 45

Go to Settings ? Notification Defaults:
- Check Interval: 120
```

### ?? "Battery Saver" - Minimal power
```
Go to Settings ? Notification Defaults:
- Check Interval: 600

Go to Settings ? Performance:
- Battery Mode: Aggressive
- WiFi Only Updates: ON

Go to Settings ? API Settings:
- Refresh Interval: 60
```

### ?? "Work Commute" - Your use case
```
Go to Settings ? Notification Defaults:
- Start Time: 17:40
- End Time: 18:25
- Proximity Radius: 500
- Check Interval: 300
- Minutes Threshold: 10
```

### ?? "Quick Test" - Test notifications fast
```
Go to Settings ? Notification Defaults:
- Check Interval: 60  (check every 1 min)
- Minutes Threshold: 30  (notify for buses ?30 min away)
- Proximity Radius: 2000  (notify within 2km)
```

---

## Individual Setting Values

### Finding More/Fewer Stops

**Show more stops** (if list is too short):
```
Map Settings ? Max Nearby Stops ? 50
Map Settings ? Search Radius ? 2000
```

**Show fewer stops** (if list is too long):
```
Map Settings ? Max Nearby Stops ? 10
Map Settings ? Search Radius ? 500
```

### Notification Timing

**Earlier warnings**:
```
Notification Defaults ? Minutes Threshold ? 20
```

**Last-minute warnings**:
```
Notification Defaults ? Minutes Threshold ? 5
```

**Check more often**:
```
Notification Defaults ? Check Interval ? 120  (2 min)
```

**Check less often** (save battery):
```
Notification Defaults ? Check Interval ? 600  (10 min)
```

### Location Sensitivity

**Tighter radius** (only notify very close):
```
Notification Defaults ? Proximity Radius ? 300
```

**Wider radius** (notify further away):
```
Notification Defaults ? Proximity Radius ? 1000
```

### API Performance

**Faster timeout** (fail quickly on slow network):
```
API Settings ? API Timeout ? 15
```

**Longer timeout** (more patient on slow network):
```
API Settings ? API Timeout ? 60
```

**Faster refresh**:
```
API Settings ? Refresh Interval ? 15
```

**Slower refresh** (save data):
```
API Settings ? Refresh Interval ? 60
```

### Appearance

**Always English**:
```
Appearance ? Use English Descriptions ? ON
```

**Show Greek names**:
```
Appearance ? Use English Descriptions ? OFF
```

**Hide stop codes**:
```
Appearance ? Show Stop Codes ? OFF
```

**Dark theme**:
```
Appearance ? Theme ? Dark
```

**Light theme**:
```
Appearance ? Theme ? Light
```

**Follow system**:
```
Appearance ? Theme ? System
```

### Notifications

**Silent notifications**:
```
Notification Behavior ? Vibrate ? OFF
Notification Behavior ? Sound ? OFF
```

**Loud notifications**:
```
Notification Behavior ? Vibrate ? ON
Notification Behavior ? Sound ? ON
Notification Behavior ? Priority ? High
```

---

## Reset Shortcuts

### Reset just notification settings
```
Settings ? Notification Defaults category
Tap "Reset" button next to category name
```

### Reset just map settings
```
Settings ? Map Settings category
Tap "Reset" button next to category name
```

### Reset everything
```
Settings ? Scroll to bottom
Tap "?? Reset All" button
Confirm
```

---

## Testing Checklist

### Before creating schedules, configure:

- [ ] Default Start Time: Your commute start time
- [ ] Default End Time: Your commute end time  
- [ ] Default Proximity: How close you need to be (500m default is good)
- [ ] Default Check Interval: 300s (5 min) is balanced
- [ ] Default Minutes Threshold: 10 min is good warning time

### For better stop finding:

- [ ] Search Radius: Start at 1000, increase if needed
- [ ] Max Nearby Stops: Start at 20, increase if needed

### For debugging:

- [ ] Debug Mode: ON
- [ ] Log Level: Debug
- [ ] Check Visual Studio Output window

---

## Common Value Ranges

| Setting | Min | Max | Good Values |
|---------|-----|-----|-------------|
| Search Radius | 500m | 5000m | 1000m-2000m |
| Max Stops | 5 | 50 | 15-30 |
| Proximity | 100m | 2000m | 400m-600m |
| Check Interval | 60s | 1800s | 180s-600s |
| Minutes Threshold | 5min | 30min | 8-15min |
| API Timeout | 10s | 60s | 20-45s |
| Refresh Interval | 15s | 120s | 25-45s |

---

**All changes save automatically!** Just edit and go. ?
