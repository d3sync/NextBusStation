namespace NextBusStation.Models;

public class RouteInfo
{
    public string RouteCode { get; set; } = string.Empty;
    
    public string LineCode { get; set; } = string.Empty;
    
    public string RouteDescr { get; set; } = string.Empty;
    
    public string RouteDescrEng { get; set; } = string.Empty;
    
    public string RouteType { get; set; } = string.Empty;
    
    public string RouteDistance { get; set; } = string.Empty;
    
    public string? LineID { get; set; }
    
    public string? LineDescr { get; set; }
    
    public string? LineDescrEng { get; set; }
    
    public string? MasterLineCode { get; set; }
}
