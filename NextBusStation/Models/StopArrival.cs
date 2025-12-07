namespace NextBusStation.Models;

public class StopArrival
{
    public string RouteCode { get; set; } = string.Empty;
    
    public string VehCode { get; set; } = string.Empty;
    
    public string Btime2 { get; set; } = string.Empty;
    
    public string? LineID { get; set; }
    
    public string? RouteDescription { get; set; }
    
    public int MinutesUntilArrival => int.TryParse(Btime2, out var minutes) ? minutes : 0;
}
