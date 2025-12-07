# ?? DEBUG MODE - Finding Stops with Detailed Logging

## ? What's New

### 1. Test Mode Toggle
You can now test with **hardcoded Athens coordinates** without needing GPS!

### 2. Comprehensive Debug Logging
Every step of the process is logged to Visual Studio Output window

### 3. Manual URL Testing
You can see and copy the exact URL being called

---

## How to Use Test Mode

### Option 1: Enable from Empty List
```
1. Open app ? Nearby tab
2. See "No nearby stops found"
3. Tap "?? Enable Test Mode (Athens)" button
4. Automatically loads stops from Athens!
```

### Option 2: Toggle Button
```
1. Go to Nearby tab
2. Tap "?? Test" button at bottom-right
3. See yellow banner: "TEST MODE ACTIVE"
4. Automatically loads stops
5. Tap "?? GPS" to switch back to real location
```

---

## Reading the Debug Output

### Where to See Logs

**Visual Studio**:
```
View ? Output ? Show output from: Debug
```

You'll see colored emojis making it easy to scan:
- ?? = Location service activity
- ?? = OASA API calls
- ?? = Coordinates
- ?? = URL being called
- ? = Success
- ? = Error
- ?? = Test mode
```

### Example Output (Success)

```
=================================================
?? MapViewModel: Starting LoadNearbyStops
   Test mode: True
?? Using TEST location (Athens, Greece)
?? LocationService: Using TEST location - Syntagma Square, Athens
   Lat: 37.9755, Lon: 23.7348
?? Using location: 37.9755, 23.7348
?? OasaApiService: GetClosestStops
   ?? Location: Lat=37.9755, Lon=23.7348
   ?? URL: http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.7348&p2=37.9755
   ?? Requesting...
   ?? Response length: 4523 chars
   ?? First 200 chars: [{"StopCode":"010001","StopID":"10001","StopDescr":"сумтацла","StopDescrEng":"SYNTAGMA"...
   ? Found 25 stops
   ?? SYNTAGMA (010001) - 150m away
   ?? MITROPOLEOS (010002) - 280m away
   ?? ERMOU (010003) - 320m away
?? Received 25 stops from API
? Success: 25 stops loaded
=================================================
```

### Example Output (GPS Mode)

```
=================================================
?? MapViewModel: Starting LoadNearbyStops
   Test mode: False
?? LocationService: Checking location permission...
   Current status: Granted
? LocationService: Permission already granted
?? LocationService: Requesting current location...
? LocationService: Got location - Lat: 40.7128, Lon: -74.0060
   Accuracy: 15m, Altitude: 10m
?? Using location: 40.7128, -74.0060
?? OasaApiService: GetClosestStops
   ?? Location: Lat=40.7128, Lon=-74.0060
   ?? URL: http://telematics.oasa.gr/api/?act=getClosestStops&p1=-74.006&p2=40.7128
   ?? Requesting...
   ?? Response length: 2 chars
   ?? First 200 chars: []
   ?? No stops returned from API
?? Received 0 stops from API
? Success: 0 stops loaded
=================================================
```

---

## Manual URL Testing

### Step 1: Get the URL from Logs

When you tap "Find Stops", look for this line:
```
?? URL: http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.7348&p2=37.9755
```

### Step 2: Test in Browser

**Copy that URL and paste in browser**, you should see JSON:

```json
[
  {
    "StopCode":"010001",
    "StopID":"10001",
    "StopDescr":"сумтацла",
    "StopDescrEng":"SYNTAGMA",
    "StopStreet":"алакиас",
    "StopStreetEng":null,
    "StopHeading":"95",
    "StopLat":"37.9753285",
    "StopLng":"23.7348187",
    "distance":"0.000234567"
  },
  ...
]
```

**If you get empty array `[]`**: 
- Your coordinates are not in Athens
- Enable Test Mode to use Athens coordinates

**If you get error**:
- OASA API might be down
- Check internet connection

### Step 3: Test with Different Coordinates

**Syntagma Square (Athens center)**:
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.7348&p2=37.9755
```

**Piraeus Port**:
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.6428&p2=37.9375
```

**Your stop 030019 area (approximate)**:
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.76&p2=37.98
```

---

## Troubleshooting with Logs

### Issue: "Location permission denied"

Look for:
```
?? LocationService: Checking location permission...
   Current status: Denied
? LocationService: Permission denied on iOS (cannot re-request)
```

**Fix**: 
- Android: App will request again
- iOS: Go to Settings ? App ? Enable Location
- **OR use Test Mode!**

### Issue: "Could not get current location"

Look for:
```
?? LocationService: Requesting current location...
? LocationService: Location is null
```

**Fix**: 
- Check GPS is enabled
- Wait longer (GPS needs time)
- Go outside
- **OR use Test Mode!**

### Issue: Empty list but no error

Look for:
```
?? Response length: 2 chars
?? First 200 chars: []
?? No stops returned from API
```

**Means**: 
- API worked, but no stops near your location
- You're not in Athens
- **Use Test Mode to verify app works!**

### Issue: API Error

Look for:
```
? OasaApiService: HTTP error - The request timed out
```

**Fix**:
- Check internet
- Increase timeout in Settings
- Retry

---

## Test Mode vs GPS Mode

| Feature | Test Mode ?? | GPS Mode ?? |
|---------|-------------|-------------|
| **Location** | Athens, Greece (hardcoded) | Your actual GPS |
| **Permission** | Not needed | Required |
| **Speed** | Instant | 5-30 seconds |
| **Internet** | Required for API | Required |
| **Use Case** | Testing, debugging | Real usage |
| **Stops Found** | Always returns Athens stops | Depends on location |

---

## Step-by-Step Debugging Session

### 1. Open Visual Studio Output
```
View ? Output
Show output from: Debug
Clear All (??? button)
```

### 2. Enable Test Mode
```
In app: Nearby tab ? Tap "?? Test" button
```

### 3. Watch Logs
You should see:
```
=================================================
?? MapViewModel: Starting LoadNearbyStops
   Test mode: True
?? Using TEST location (Athens, Greece)
?? LocationService: Using TEST location - Syntagma Square, Athens
   Lat: 37.9755, Lon: 23.7348
```

### 4. Copy the URL
Look for:
```
?? URL: http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.7348&p2=37.9755
```

### 5. Test URL in Browser
Paste URL ? Should see JSON with stops

### 6. Check App
Should show list of stops with distance in meters

### 7. If Still No Stops
```
Check Output for:
- ? errors
- ?? warnings
- API response content
```

---

## Expected Test Mode Results

When Test Mode is enabled, you should see stops like:

```
SYNTAGMA (010001) - 150m away
MITROPOLEOS (010002) - 280m away
ERMOU (010003) - 320m away
PANEPISTIMIO (010004) - 450m away
MONASTIRAKI (010005) - 520m away
...
```

If you don't see these, check Output for errors!

---

## Quick Commands

**Toggle Test Mode**:
```
Tap ??/?? button at bottom-right
```

**Force Refresh**:
```
Tap "?? Find Nearby Stops" button
```

**Clear Output**:
```
Visual Studio ? Output window ? Clear All button
```

---

## What to Share for Help

If still not working, share:

1. **Full Output log** (copy from Output window)
2. **URL** from the `?? URL:` line
3. **Response** from testing URL in browser
4. **Error messages** (any ? lines)
5. **Your location** (are you in Greece?)

---

**Now you have complete visibility into what's happening!** ??

Test Mode means you can **always test the app** even if:
- You're not in Athens
- GPS isn't working
- You don't want to grant location permission
- You're debugging on an emulator

**Just enable Test Mode and it works!** ???
