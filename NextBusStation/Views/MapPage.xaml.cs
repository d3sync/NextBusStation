using NextBusStation.ViewModels;
using System.Collections.Specialized;
using System.Text;

namespace NextBusStation.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    
    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        
        // Subscribe to collection changes to update map
        _viewModel.NearbyStops.CollectionChanged += OnNearbyStopsChanged;
        
        // Subscribe to property changes
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        
        // Initialize the map
        InitializeMap();
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Refresh debug features setting in case it was changed in settings
        await _viewModel.RefreshDebugSettingCommand.ExecuteAsync(null);
        
        if (_viewModel.NearbyStops.Count == 0)
        {
            await _viewModel.LoadNearbyStopsCommand.ExecuteAsync(null);
        }
    }
    
    private void InitializeMap()
    {
        var htmlSource = new HtmlWebViewSource
        {
            Html = GetMapHtml()
        };
        MapWebView.Source = htmlSource;
        
        // Handle navigation from JavaScript
        MapWebView.Navigating += OnWebViewNavigating;
    }
    
    private async void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        // Check if this is a stop selection event
        if (e.Url.StartsWith("stopselect://"))
        {
            e.Cancel = true; // Prevent actual navigation
            
            // Extract stop code from URL
            var stopCode = e.Url.Replace("stopselect://", "");
            
            // Find the stop in the collection
            var stop = _viewModel.NearbyStops.FirstOrDefault(s => s.StopCode == stopCode);
            
            if (stop != null)
            {
                await _viewModel.SelectStopCommand.ExecuteAsync(stop);
            }
        }
    }
    
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapViewModel.ShowMap) && _viewModel.ShowMap)
        {
            UpdateMap();
        }
        else if (e.PropertyName == nameof(MapViewModel.CurrentLocation))
        {
            UpdateMap();
        }
    }
    
    private void OnNearbyStopsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_viewModel.ShowMap)
        {
            UpdateMap();
        }
    }
    
    private void UpdateMap()
    {
        if (_viewModel.CurrentLocation == null && _viewModel.NearbyStops.Count == 0)
            return;
            
        var htmlSource = new HtmlWebViewSource
        {
            Html = GetMapHtml()
        };
        MapWebView.Source = htmlSource;
    }
    
    private string GetMapHtml()
    {
        var lat = _viewModel.CurrentLocation?.Latitude ?? 37.9838;
        var lon = _viewModel.CurrentLocation?.Longitude ?? 23.7275;
        
        var markers = new StringBuilder();
        
        // Add current location marker
        if (_viewModel.CurrentLocation != null)
        {
            markers.AppendLine($@"
                var currentLocationIcon = L.divIcon({{
                    className: 'current-location-marker',
                    html: '<div style=""background-color: #4285F4; width: 20px; height: 20px; border-radius: 50%; border: 3px solid white; box-shadow: 0 2px 6px rgba(0,0,0,0.3);""></div>',
                    iconSize: [20, 20],
                    iconAnchor: [10, 10]
                }});
                L.marker([{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}], {{icon: currentLocationIcon}})
                    .addTo(map)
                    .bindPopup('<b>Your Location</b><br>Current position');
            ");
        }
        
        // Add stop markers
        foreach (var stop in _viewModel.NearbyStops)
        {
            var stopName = stop.StopDescrEng.Replace("'", "\\'").Replace("\"", "&quot;");
            var stopNameGreek = stop.StopDescr.Replace("'", "\\'").Replace("\"", "&quot;");
            var stopCode = stop.StopCode;
            var distance = $"{stop.Distance:F0}m";
            var stopLat = stop.StopLat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var stopLon = stop.StopLng.ToString(System.Globalization.CultureInfo.InvariantCulture);
            
            markers.AppendLine($@"
                var marker{stopCode} = L.marker([{stopLat}, {stopLon}])
                    .addTo(map)
                    .bindPopup('<div style=""min-width: 200px;""><b>{stopName}</b><br><i>{stopNameGreek}</i><br>Code: {stopCode}<br>Distance: {distance}<br><br><button onclick=""selectStop(\'{stopCode}\')"" style=""padding: 8px 16px; background-color: #512BD4; color: white; border: none; border-radius: 6px; cursor: pointer; font-size: 14px; font-weight: bold;"">View Arrivals ?</button></div>');
                
                marker{stopCode}.on('click', function() {{ 
                    map.setView([{stopLat}, {stopLon}], 16);
                }});
            ");
        }
        
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"">
    <title>OpenStreetMap</title>
    <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"" 
          integrity=""sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY="" 
          crossorigin=""""/>
    <script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""
            integrity=""sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo="";
            crossorigin=""""></script>
    <style>
        body {{
            margin: 0;
            padding: 0;
        }}
        #map {{
            width: 100%;
            height: 100vh;
        }}
        .current-location-marker {{
            background: none;
            border: none;
        }}
        .leaflet-popup-content {{
            margin: 13px 19px;
            line-height: 1.4;
        }}
        button:active {{
            background-color: #6D42C7 !important;
        }}
    </style>
</head>
<body>
    <div id=""map""></div>
    <script>
        var map = L.map('map').setView([{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}], 15);
        
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors',
            maxZoom: 19
        }}).addTo(map);
        
        {markers}
        
        function selectStop(stopCode) {{
            window.location.href = 'stopselect://' + stopCode;
        }}
    </script>
</body>
</html>";
    }
}
