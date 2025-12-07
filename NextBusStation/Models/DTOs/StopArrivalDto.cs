using System.Text.Json.Serialization;

namespace NextBusStation.Models.DTOs;

public class StopArrivalDto
{
    [JsonPropertyName("route_code")]
    public string RouteCode { get; set; } = string.Empty;
    
    [JsonPropertyName("veh_code")]
    public string VehCode { get; set; } = string.Empty;
    
    [JsonPropertyName("btime2")]
    public string Btime2 { get; set; } = string.Empty;
}
