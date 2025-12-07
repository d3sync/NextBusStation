using SQLite;

namespace NextBusStation.Models;

[Table("notification_schedules")]
public class NotificationSchedule
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string StopCode { get; set; } = string.Empty;
    
    public string StopName { get; set; } = string.Empty;
    
    public TimeSpan StartTime { get; set; }
    
    public TimeSpan EndTime { get; set; }
    
    public double ProximityRadius { get; set; } = 500;
    
    public int CheckIntervalSeconds { get; set; } = 300;
    
    public int MinMinutesThreshold { get; set; } = 10;
    
    public bool MondayEnabled { get; set; } = true;
    
    public bool TuesdayEnabled { get; set; } = true;
    
    public bool WednesdayEnabled { get; set; } = true;
    
    public bool ThursdayEnabled { get; set; } = true;
    
    public bool FridayEnabled { get; set; } = true;
    
    public bool SaturdayEnabled { get; set; }
    
    public bool SundayEnabled { get; set; }
    
    public bool IsEnabled { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? LastNotificationSent { get; set; }
    
    [Ignore]
    public bool IsActiveNow
    {
        get
        {
            if (!IsEnabled)
                return false;
            
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            
            if (currentTime < StartTime || currentTime > EndTime)
                return false;
            
            return now.DayOfWeek switch
            {
                DayOfWeek.Monday => MondayEnabled,
                DayOfWeek.Tuesday => TuesdayEnabled,
                DayOfWeek.Wednesday => WednesdayEnabled,
                DayOfWeek.Thursday => ThursdayEnabled,
                DayOfWeek.Friday => FridayEnabled,
                DayOfWeek.Saturday => SaturdayEnabled,
                DayOfWeek.Sunday => SundayEnabled,
                _ => false
            };
        }
    }
    
    [Ignore]
    public string DaysOfWeekDisplay
    {
        get
        {
            var days = new List<string>();
            if (MondayEnabled) days.Add("Mon");
            if (TuesdayEnabled) days.Add("Tue");
            if (WednesdayEnabled) days.Add("Wed");
            if (ThursdayEnabled) days.Add("Thu");
            if (FridayEnabled) days.Add("Fri");
            if (SaturdayEnabled) days.Add("Sat");
            if (SundayEnabled) days.Add("Sun");
            
            return days.Count == 7 ? "Every day" :
                   days.Count == 5 && !SaturdayEnabled && !SundayEnabled ? "Weekdays" :
                   days.Count == 0 ? "None" :
                   string.Join(", ", days);
        }
    }
}
