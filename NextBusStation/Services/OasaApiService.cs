using System.Text.Json;
using NextBusStation.Models;
using NextBusStation.Models.DTOs;
using System.Globalization;

namespace NextBusStation.Services;

public class OasaApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string BaseUrl = "http://telematics.oasa.gr/api/";
    
    public OasaApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<List<BusStop>> GetClosestStopsAsync(double longitude, double latitude, int maxResults = 20)
    {
        try
        {
            // SWAPPED: Testing if API expects latitude first, then longitude
            var url = $"{BaseUrl}?act=getClosestStops&p1={latitude.ToString(CultureInfo.InvariantCulture)}&p2={longitude.ToString(CultureInfo.InvariantCulture)}";
            
            System.Diagnostics.Debug.WriteLine("?? OasaApiService: GetClosestStops");
            System.Diagnostics.Debug.WriteLine($"   ?? Location: Lat={latitude}, Lon={longitude}");
            System.Diagnostics.Debug.WriteLine($"   ?? URL: {url}");
            System.Diagnostics.Debug.WriteLine($"   ?? PARAMETERS SWAPPED FOR TESTING!");
            System.Diagnostics.Debug.WriteLine($"   ?? Requesting...");
            
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(url);
            
            System.Diagnostics.Debug.WriteLine($"   ?? Response length: {response.Length} chars");
            System.Diagnostics.Debug.WriteLine($"   ?? First 200 chars: {response.Substring(0, Math.Min(200, response.Length))}");
            
            var dtos = JsonSerializer.Deserialize<List<ClosestStopDto>>(response);
            
            if (dtos == null || !dtos.Any())
            {
                System.Diagnostics.Debug.WriteLine("   ?? No stops returned from API");
                return new List<BusStop>();
            }
            
            System.Diagnostics.Debug.WriteLine($"   ? Found {dtos.Count} stops");
            
            var stops = dtos.Select(dto =>
            {
                var stopLat = double.TryParse(dto.StopLat, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ? lat : 0;
                var stopLng = double.TryParse(dto.StopLng, NumberStyles.Any, CultureInfo.InvariantCulture, out var lng) ? lng : 0;
                
                // Calculate actual distance using Haversine formula
                var distance = CalculateDistance(latitude, longitude, stopLat, stopLng);
                
                return new BusStop
                {
                    StopCode = dto.StopCode ?? string.Empty,
                    StopId = dto.StopID ?? string.Empty,
                    StopDescr = dto.StopDescr ?? string.Empty,
                    StopDescrEng = dto.StopDescrEng ?? dto.StopDescr ?? string.Empty,
                    StopStreet = dto.StopStreet,
                    StopLat = stopLat,
                    StopLng = stopLng,
                    Distance = distance
                };
            })
            .OrderBy(s => s.Distance) // Sort by distance, closest first
            .Take(maxResults)
            .ToList();
            
            foreach (var stop in stops.Take(3))
            {
                System.Diagnostics.Debug.WriteLine($"   ?? {stop.StopDescrEng} ({stop.StopCode}) - {stop.Distance:F0}m away");
            }
            
            return stops;
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"? OasaApiService: HTTP error - {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"? OasaApiService: JSON parse error - {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? OasaApiService: Unexpected error - {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            throw;
        }
    }
    
    public async Task<List<StopArrival>> GetStopArrivalsAsync(string stopCode)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{BaseUrl}?act=getStopArrivals&p1={stopCode}";
            
            System.Diagnostics.Debug.WriteLine($"?? GetStopArrivals for stop: {stopCode}");
            
            var response = await httpClient.GetStringAsync(url);
            
            System.Diagnostics.Debug.WriteLine($"   ?? Response: {response.Substring(0, Math.Min(500, response.Length))}");
            
            var dtos = JsonSerializer.Deserialize<List<StopArrivalDto>>(response);
            
            if (dtos == null)
            {
                System.Diagnostics.Debug.WriteLine("   ?? No arrivals returned");
                return new List<StopArrival>();
            }
            
            System.Diagnostics.Debug.WriteLine($"   ? Found {dtos.Count} arrivals");
            foreach (var dto in dtos.Take(3))
            {
                System.Diagnostics.Debug.WriteLine($"      • RouteCode={dto.RouteCode}, VehCode={dto.VehCode}, Btime2={dto.Btime2}");
            }
            
            return dtos.Select(dto => new StopArrival
            {
                RouteCode = dto.RouteCode,
                VehCode = dto.VehCode,
                Btime2 = dto.Btime2
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error getting stop arrivals: {ex.Message}");
            return new List<StopArrival>();
        }
    }
    
    public async Task<List<RouteInfo>> GetRoutesForStopAsync(string stopCode)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{BaseUrl}?act=webRoutesForStop&p1={stopCode}";
            
            System.Diagnostics.Debug.WriteLine($"?? GetRoutesForStop for stop: {stopCode}");
            
            var response = await httpClient.GetStringAsync(url);
            
            System.Diagnostics.Debug.WriteLine($"   ?? Response: {response.Substring(0, Math.Min(500, response.Length))}");
            
            var dtos = JsonSerializer.Deserialize<List<RouteForStopDto>>(response);
            
            if (dtos == null)
            {
                System.Diagnostics.Debug.WriteLine("   ?? No routes returned");
                return new List<RouteInfo>();
            }
            
            System.Diagnostics.Debug.WriteLine($"   ? Found {dtos.Count} routes");
            foreach (var dto in dtos.Take(5))
            {
                System.Diagnostics.Debug.WriteLine($"      • RouteCode={dto.RouteCode}, LineID={dto.LineID ?? "(null)"}, LineDescr={dto.LineDescr}");
            }
            
            return dtos.Select(dto => new RouteInfo
            {
                RouteCode = dto.RouteCode,
                LineCode = dto.LineCode,
                RouteDescr = dto.RouteDescr,
                RouteDescrEng = dto.RouteDescrEng,
                RouteType = dto.RouteType,
                RouteDistance = dto.RouteDistance,
                LineID = dto.LineID,
                LineDescr = dto.LineDescr,
                LineDescrEng = dto.LineDescrEng,
                MasterLineCode = dto.MasterLineCode
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error getting routes for stop: {ex.Message}");
            return new List<RouteInfo>();
        }
    }
    
    // Haversine formula to calculate distance between two coordinates in meters
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;
        
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        var distanceKm = EarthRadiusKm * c;
        return distanceKm * 1000; // Convert to meters
    }
    
    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
