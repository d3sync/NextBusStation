# Parameter Order Verification

## Current Implementation

### Test Location (Syntagma Square, Athens)
```csharp
var testLocation = new Location(37.9755, 23.7348);
//                               ?        ?
//                            Latitude  Longitude
```

### API Call
```csharp
await _oasaService.GetClosestStopsAsync(
    location.Longitude,  // p1 = 23.7348 (X coordinate)
    location.Latitude);  // p2 = 37.9755 (Y coordinate)
```

### Generated URL
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.7348&p2=37.9755
```

## OASA API Specification

From the docs:
```
p1 – x: longitude
p2 – y: latitude
```

## Verification

**Test this URL in your browser**:
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.7348&p2=37.9755
```

**Expected result**: JSON array with Athens bus stops near Syntagma Square

**If you get empty array `[]`**: Parameters might be swapped (though they look correct)

---

## Alternative Test (Piraeus)

If Syntagma doesn't work, try Piraeus port:

```
Coordinates: Lat=37.9375, Lon=23.6428
URL: http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.6428&p2=37.9375
```

---

## Debug Steps

1. **Enable Test Mode** in app
2. **Check Visual Studio Output** for the URL line:
   ```
   ?? URL: http://telematics.oasa.gr/api/?act=getClosestStops&p1=X.XXX&p2=Y.YYY
   ```
3. **Copy that exact URL**
4. **Paste in browser**
5. **Check if you get stops**

If browser returns stops but app doesn't, there's a parsing issue.
If browser returns empty array too, coordinates might be wrong.

---

## Current Status

? Parameter order is **CORRECT** as per OASA docs  
? Test location coordinates are **CORRECT** (Syntagma Square)  
? URL construction is **CORRECT**  

The code should work! ??

Test it and let me know what the actual URL in the debug output shows!
