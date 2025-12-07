# ? PARAMETERS SWAPPED!

## What Changed

The API call now uses:
```csharp
// OLD (per docs):
p1={longitude}&p2={latitude}

// NEW (swapped):
p1={latitude}&p2={longitude}
```

## New URL Format

With test location (Syntagma Square):
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=37.9755&p2=23.7348
```

**Before**: `p1=23.7348&p2=37.9755`  
**After**: `p1=37.9755&p2=23.7348`  

## To Test

1. **Stop the running app** (if debugging)
2. **Rebuild**: Clean + Build
3. **Run app**
4. **Enable Test Mode**
5. **Check Output** for the new URL
6. **Should now find stops!**

## Debug Output Will Show

```
?? OasaApiService: GetClosestStops
   ?? Location: Lat=37.9755, Lon=23.7348
   ?? URL: http://telematics.oasa.gr/api/?act=getClosestStops&p1=37.9755&p2=23.7348
   ?? PARAMETERS SWAPPED FOR TESTING!
   ?? Requesting...
```

Notice the URL now has **latitude FIRST**, then longitude.

## If This Works

It means the OASA API docs were wrong/misleading about parameter names!

The actual parameter order is:
- `p1` = **latitude** (not longitude as docs say)
- `p2` = **longitude** (not latitude as docs say)

## Next Steps

1. Stop debugging
2. Rebuild app
3. Run and test
4. Let me know if stops appear!

---

**The code has been changed - parameters are now SWAPPED!** ??
