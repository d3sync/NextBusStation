# NextBusStation - Athens Bus Arrival App

A .NET MAUI application for Android that shows nearby bus stops and real-time bus arrival information using the OASA Telematics API.

## Features

- **Find Nearby Stops**: Uses GPS to find bus stops near your current location
- **Real-time Arrivals**: Shows how many minutes until the next buses arrive at each stop
- **Route Information**: Displays all bus routes that serve each stop
- **Favorites**: Save favorite stops to local SQLite database
- **Offline Storage**: Stores recently viewed stops locally

## Technology Stack

- **.NET 10 MAUI** - Cross-platform framework
- **MVVM Pattern** - Using CommunityToolkit.Mvvm
- **SQLite** - Local database for favorites and caching
- **OASA Telematics API** - Athens public transport real-time data

## Project Structure

```
NextBusStation/
??? Models/                    # Data models
?   ??? BusStop.cs            # Bus stop entity
?   ??? StopArrival.cs        # Arrival information
?   ??? RouteInfo.cs          # Route information
?   ??? DTOs/                 # API response models
??? Services/                  # Business logic
?   ??? OasaApiService.cs     # OASA API client
?   ??? DatabaseService.cs    # SQLite database operations
?   ??? LocationService.cs    # GPS/location services
??? ViewModels/               # MVVM view models
?   ??? MapViewModel.cs       # Nearby stops screen
?   ??? StopDetailsViewModel.cs # Stop details screen
??? Views/                    # UI pages
?   ??? MapPage.xaml          # Main map/list view
?   ??? StopDetailsPage.xaml  # Stop details view
??? Converters/               # Value converters
    ??? FavoriteIconConverter.cs
```

## Permissions Required

### Android
- `ACCESS_FINE_LOCATION` - Get precise GPS location
- `ACCESS_COARSE_LOCATION` - Get approximate location
- `INTERNET` - Access OASA API
- `ACCESS_NETWORK_STATE` - Check network connectivity

## How to Use

1. **Launch the app** - You'll see the "Nearby Stops" screen
2. **Tap "Find Nearby Stops"** - The app will:
   - Request location permission (first time only)
   - Get your GPS coordinates
   - Fetch nearby bus stops from OASA API
3. **Tap on any stop** - View detailed information:
   - Next bus arrivals with countdown (in minutes)
   - All routes serving that stop
   - Auto-refreshes every 30 seconds
4. **Tap the star icon** - Save stops as favorites (future feature)

## OASA API Endpoints Used

1. **getClosestStops** - Find bus stops near coordinates
   ```
   http://telematics.oasa.gr/api/?act=getClosestStops&p1={longitude}&p2={latitude}
   ```

2. **getStopArrivals** - Get next arriving buses for a stop
   ```
   http://telematics.oasa.gr/api/?act=getStopArrivals&p1={stopCode}
   ```

3. **webRoutesForStop** - Get all routes serving a stop
   ```
   http://telematics.oasa.gr/api/?act=webRoutesForStop&p1={stopCode}
   ```

## Building and Running

### Prerequisites
- Visual Studio 2022 17.13+ with .NET MAUI workload
- Android SDK API 21+
- .NET 10 SDK

### Debug on Android Emulator
1. Open `NextBusStation.sln` in Visual Studio
2. Select "Android Emulator" as target
3. Press F5 or click "Start Debugging"

### Deploy to Physical Android Device
1. Enable Developer Mode on your Android device
2. Enable USB Debugging
3. Connect device via USB
4. Select your device in Visual Studio
5. Press F5

## Future Enhancements

- [ ] Add actual map view using OpenStreetMap/Mapsui
- [ ] Show bus markers on map
- [ ] Display favorites on startup
- [ ] Add route planning
- [ ] Add notifications for favorite stops
- [ ] Support for iOS platform
- [ ] Offline map tiles
- [ ] Search for stops by name/code

## Notes

- The app currently shows stops in a list view. Map integration with OpenStreetMap will be added in future versions.
- API responses return Greek and English descriptions - the app displays English when available.
- Time estimates are provided by OASA's real-time tracking system.
- The database stores stops locally to improve performance and enable offline viewing of recently visited stops.

## License

This is a personal/educational project. OASA API data belongs to Athens Urban Transport Organisation.
