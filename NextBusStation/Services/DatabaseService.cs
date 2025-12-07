using SQLite;
using NextBusStation.Models;

namespace NextBusStation.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    
    public async Task InitializeAsync()
    {
        if (_database != null)
            return;
            
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "nextbusstation.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        
        await _database.CreateTableAsync<BusStop>();
        await _database.CreateTableAsync<NotificationSchedule>();
        await _database.CreateTableAsync<AppSettings>();
    }
    
    public async Task<SQLiteAsyncConnection> GetDatabaseConnectionAsync()
    {
        await InitializeAsync();
        return _database!;
    }
    
    public async Task<List<BusStop>> GetFavoriteStopsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<BusStop>()
            .Where(s => s.IsFavorite)
            .ToListAsync();
    }
    
    public async Task<BusStop?> GetStopAsync(string stopCode)
    {
        await InitializeAsync();
        return await _database!.Table<BusStop>()
            .Where(s => s.StopCode == stopCode)
            .FirstOrDefaultAsync();
    }
    
    public async Task<int> SaveStopAsync(BusStop stop)
    {
        await InitializeAsync();
        
        var existing = await GetStopAsync(stop.StopCode);
        if (existing != null)
        {
            return await _database!.UpdateAsync(stop);
        }
        else
        {
            return await _database!.InsertAsync(stop);
        }
    }
    
    public async Task<int> ToggleFavoriteAsync(string stopCode)
    {
        await InitializeAsync();
        
        var stop = await GetStopAsync(stopCode);
        if (stop != null)
        {
            stop.IsFavorite = !stop.IsFavorite;
            return await _database!.UpdateAsync(stop);
        }
        
        return 0;
    }
    
    public async Task<int> DeleteStopAsync(BusStop stop)
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(stop);
    }
    
    public async Task<List<NotificationSchedule>> GetAllSchedulesAsync()
    {
        await InitializeAsync();
        return await _database!.Table<NotificationSchedule>().ToListAsync();
    }
    
    public async Task<List<NotificationSchedule>> GetActiveSchedulesAsync()
    {
        await InitializeAsync();
        var allSchedules = await _database!.Table<NotificationSchedule>()
            .Where(s => s.IsEnabled)
            .ToListAsync();
        
        return allSchedules.Where(s => s.IsActiveNow).ToList();
    }
    
    public async Task<NotificationSchedule?> GetScheduleAsync(int id)
    {
        await InitializeAsync();
        return await _database!.Table<NotificationSchedule>()
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
    }
    
    public async Task<int> SaveScheduleAsync(NotificationSchedule schedule)
    {
        await InitializeAsync();
        
        if (schedule.Id == 0)
        {
            return await _database!.InsertAsync(schedule);
        }
        else
        {
            return await _database!.UpdateAsync(schedule);
        }
    }
    
    public async Task<int> DeleteScheduleAsync(NotificationSchedule schedule)
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(schedule);
    }
    
    public async Task UpdateLastNotificationTimeAsync(int scheduleId)
    {
        await InitializeAsync();
        
        var schedule = await GetScheduleAsync(scheduleId);
        if (schedule != null)
        {
            schedule.LastNotificationSent = DateTime.Now;
            await _database!.UpdateAsync(schedule);
        }
    }
}
