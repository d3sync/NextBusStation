using SQLite;

namespace NextBusStation.Models;

[Table("stops")]
public class BusStop
{
    [PrimaryKey]
    public string StopCode { get; set; } = string.Empty;
    
    public string StopId { get; set; } = string.Empty;
    
    public string StopDescr { get; set; } = string.Empty;
    
    public string StopDescrEng { get; set; } = string.Empty;
    
    public string? StopStreet { get; set; }
    
    public double StopLat { get; set; }
    
    public double StopLng { get; set; }
    
    public bool IsFavorite { get; set; }
    
    [Ignore]
    public double Distance { get; set; }
}
