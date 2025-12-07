# LineID Display Fix Summary

## Problem Analysis

### Root Causes Identified:

1. **Duplicate XAML Elements** ?
   - Lines 101-102, 110-111, 169-170, and 177-178 had duplicate `<Label>` declarations
   - This caused the "An item with the same key has already been added" error

2. **API Data Structure** ??
   - `getStopArrivals` API returns: `route_code`, `veh_code`, `btime2` (NO LineID)
   - `webRoutesForStop` API returns: `RouteCode`, `LineID`, `LineDescr`, etc.
   - The LineID must be **enriched** by matching RouteCode between these two datasets

3. **Multiple Buses Per Line** ????
   - Yes, multiple buses of the same line (e.g., bus 876) can appear in the queue
   - Each arrival is a separate entry with the same RouteCode but different vehicle codes
   - This is **expected behavior** - shows all upcoming arrivals for that line

## Changes Made

### 1. Fixed StopDetailsPage.xaml ?
**Removed all duplicate Label elements:**
- Removed duplicate "Bus Line Number Badge" labels (line 101)
- Removed duplicate "Route Description" labels (line 110)
- Removed duplicate "Line Number Badge" labels (line 169)
- Removed duplicate "Route Info" labels (line 177)
- Fixed header Grid structure (removed extra StackLayout wrapper)

### 2. Enhanced StopDetailsViewModel.cs ??
**Improved data enrichment logic:**
```csharp
// Better fallback handling
if (routeLookup.TryGetValue(arrival.RouteCode, out var routeInfo))
{
    arrival.LineID = routeInfo.LineID ?? "ó";
    arrival.RouteDescription = routeInfo.RouteDescrEng ?? routeInfo.RouteDescr ?? "Unknown Route";
}
else
{
    // Fallback if route not found in lookup
    arrival.LineID = "ó";
    arrival.RouteDescription = $"Route {arrival.RouteCode}";
    
    System.Diagnostics.Debug.WriteLine($"?? No route info found for RouteCode: {arrival.RouteCode}");
}
```

**Benefits:**
- Always shows "ó" instead of blank when LineID is missing
- Provides meaningful fallback route description
- Logs missing route matches for debugging

### 3. Added Debug Logging to OasaApiService.cs ??
**New detailed logging:**
```csharp
// GetStopArrivals now logs:
System.Diagnostics.Debug.WriteLine($"?? GetStopArrivals for stop: {stopCode}");
System.Diagnostics.Debug.WriteLine($"   ? Found {dtos.Count} arrivals");
foreach (var dto in dtos.Take(3))
{
    System.Diagnostics.Debug.WriteLine($"      ï RouteCode={dto.RouteCode}, VehCode={dto.VehCode}, Btime2={dto.Btime2}");
}

// GetRoutesForStop now logs:
System.Diagnostics.Debug.WriteLine($"?? GetRoutesForStop for stop: {stopCode}");
System.Diagnostics.Debug.WriteLine($"   ? Found {dtos.Count} routes");
foreach (var dto in dtos.Take(5))
{
    System.Diagnostics.Debug.WriteLine($"      ï RouteCode={dto.RouteCode}, LineID={dto.LineID ?? "(null)"}, LineDescr={dto.LineDescr}");
}
```

## Expected Behavior

### Multiple Buses of Same Line ?
**Scenario:** Two bus 876 arrivals
```
???????????????????????????????????????
? 876  ? PEIRAIAS - VOULA   ? 5 min  ?
???????????????????????????????????????
? 876  ? PEIRAIAS - VOULA   ? 12 min ?
???????????????????????????????????????
```
This is **correct** - it shows:
- First bus 876 arriving in 5 minutes
- Second bus 876 arriving in 12 minutes

### When LineID is Available ?
```
???????????????????????????????????????
? 876  ? PEIRAIAS - VOULA   ? 5 min  ?
? 040  ? SYNTAGMA - PIRAEUS ? 8 min  ?
? A2   ? AIRPORT EXPRESS    ? 15 min ?
???????????????????????????????????????
```

### When LineID is Missing (Fallback) ??
```
???????????????????????????????????????
?  ó   ? Route 2045          ? 5 min  ?
???????????????????????????????????????
```
Shows:
- "ó" symbol instead of blank or "?"
- Route code in description

## Why "?" Was Appearing Before

The old code had:
```xaml
<Label Text="{Binding LineID, TargetNullValue='?'}"/>
```

This explicitly showed "?" when LineID was null. We've removed this because:
1. It's confusing - users don't know what "?" means
2. The "ó" symbol is clearer (indicates "not available")
3. We now have better fallback handling in the ViewModel

## Debugging the Issue

If you still see issues, check the Debug output for:

### 1. API Response Check
```
?? GetStopArrivals for stop: 400075
   ? Found 3 arrivals
      ï RouteCode=2045, VehCode=50328, Btime2=5
      ï RouteCode=2045, VehCode=50329, Btime2=12  ? Same line, different bus!
      ï RouteCode=2046, VehCode=20521, Btime2=8
```

### 2. Route Lookup Check
```
?? GetRoutesForStop for stop: 400075
   ? Found 2 routes
      ï RouteCode=2045, LineID=876, LineDescr=–≈…—¡…¡” - ¬œ’À¡
      ï RouteCode=2046, LineID=876, LineDescr=¬œ’À¡ - –≈…—¡…¡”
```

### 3. Missing Route Warning
```
?? No route info found for RouteCode: 2045
```
If you see this, it means:
- The arrival's RouteCode doesn't match any route in the routes list
- This could indicate an API inconsistency
- The fallback ("ó" and "Route 2045") will be shown

## Testing Checklist

- [x] Build successful ?
- [ ] No "item with same key" error
- [ ] Multiple buses of same line display correctly
- [ ] LineID shows bus number when available
- [ ] "ó" shows instead of blank when LineID missing
- [ ] Route descriptions display properly
- [ ] Debug logs show API responses

## Related Files Modified

1. `NextBusStation/Views/StopDetailsPage.xaml` - Fixed duplicates
2. `NextBusStation/ViewModels/StopDetailsViewModel.cs` - Enhanced enrichment
3. `NextBusStation/Services/OasaApiService.cs` - Added debug logging

## Next Steps

1. **Deploy and test** on device
2. **Check Debug output** for any "?? No route info found" warnings
3. **Verify** that multiple buses of same line appear correctly
4. If issues persist, **share the debug output** showing the API responses
