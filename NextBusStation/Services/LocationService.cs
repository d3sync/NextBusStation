namespace NextBusStation.Services;

public class LocationService
{
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("?? LocationService: Requesting current location...");
            
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(30));
            var location = await Geolocation.Default.GetLocationAsync(request);
            
            if (location != null)
            {
                System.Diagnostics.Debug.WriteLine($"? LocationService: Got location - Lat: {location.Latitude}, Lon: {location.Longitude}");
                System.Diagnostics.Debug.WriteLine($"   Accuracy: {location.Accuracy}m, Altitude: {location.Altitude}m");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("? LocationService: Location is null");
            }
            
            return location;
        }
        catch (FeatureNotSupportedException ex)
        {
            System.Diagnostics.Debug.WriteLine($"? LocationService: Feature not supported - {ex.Message}");
            return null;
        }
        catch (FeatureNotEnabledException ex)
        {
            System.Diagnostics.Debug.WriteLine($"? LocationService: Feature not enabled - {ex.Message}");
            return null;
        }
        catch (PermissionException ex)
        {
            System.Diagnostics.Debug.WriteLine($"? LocationService: Permission denied - {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? LocationService: Error getting location - {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            return null;
        }
    }
    
    public async Task<bool> CheckAndRequestLocationPermissionAsync()
    {
        System.Diagnostics.Debug.WriteLine("?? LocationService: Checking location permission...");
        
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        System.Diagnostics.Debug.WriteLine($"   Current status: {status}");
        
        if (status == PermissionStatus.Granted)
        {
            System.Diagnostics.Debug.WriteLine("? LocationService: Permission already granted");
            return true;
        }
        
        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            System.Diagnostics.Debug.WriteLine("? LocationService: Permission denied on iOS (cannot re-request)");
            return false;
        }
        
        System.Diagnostics.Debug.WriteLine("?? LocationService: Requesting permission...");
        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        System.Diagnostics.Debug.WriteLine($"   New status: {status}");
        
        return status == PermissionStatus.Granted;
    }
    
    // Helper method to get a test location in Athens
    public Location GetAthensTestLocation()
    {
        // Syntagma Square, Athens
        var testLocation = new Location(37.9755, 23.7348);
        System.Diagnostics.Debug.WriteLine($"?? LocationService: Using TEST location - Syntagma Square, Athens");
        System.Diagnostics.Debug.WriteLine($"   Lat: {testLocation.Latitude}, Lon: {testLocation.Longitude}");
        return testLocation;
    }
}
