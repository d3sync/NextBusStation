using System.Text.Json.Serialization;

namespace NextBusStation.Models.DTOs;

public class RouteForStopDto
{
    [JsonPropertyName("RouteCode")]
    public string RouteCode { get; set; } = string.Empty;
    
    [JsonPropertyName("LineCode")]
    public string LineCode { get; set; } = string.Empty;
    
    [JsonPropertyName("RouteDescr")]
    public string RouteDescr { get; set; } = string.Empty;
    
    [JsonPropertyName("RouteDescrEng")]
    public string RouteDescrEng { get; set; } = string.Empty;
    
    [JsonPropertyName("RouteType")]
    public string RouteType { get; set; } = string.Empty;
    
    [JsonPropertyName("RouteDistance")]
    public string RouteDistance { get; set; } = string.Empty;
    
    [JsonPropertyName("LineID")]
    public string? LineID { get; set; }
    
    [JsonPropertyName("LineDescr")]
    public string? LineDescr { get; set; }
    
    [JsonPropertyName("LineDescrEng")]
    public string? LineDescrEng { get; set; }
    
    [JsonPropertyName("MasterLineCode")]
    public string? MasterLineCode { get; set; }
}
