using System.Text.Json.Serialization;

namespace NextBusStation.Models.DTOs;

public class ClosestStopDto
{
    [JsonPropertyName("StopCode")]
    public string StopCode { get; set; } = string.Empty;
    
    [JsonPropertyName("StopID")]
    public string StopID { get; set; } = string.Empty;
    
    [JsonPropertyName("StopDescr")]
    public string StopDescr { get; set; } = string.Empty;
    
    [JsonPropertyName("StopDescrEng")]
    public string StopDescrEng { get; set; } = string.Empty;
    
    [JsonPropertyName("StopStreet")]
    public string? StopStreet { get; set; }
    
    [JsonPropertyName("StopStreetEng")]
    public string? StopStreetEng { get; set; }
    
    [JsonPropertyName("StopHeading")]
    public string StopHeading { get; set; } = string.Empty;
    
    [JsonPropertyName("StopLat")]
    public string StopLat { get; set; } = string.Empty;
    
    [JsonPropertyName("StopLng")]
    public string StopLng { get; set; } = string.Empty;
    
    [JsonPropertyName("distance")]
    public string Distance { get; set; } = string.Empty;
}
