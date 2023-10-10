using Microsoft.AspNetCore.Mvc;
namespace LastSeenTask;

public class UserOnlineData
{
    public DateTime? NearestOnlineTime { get; set; }
    public bool? WasUserOnline { get; set; }
}

public interface IHistoricalDataStorageConcrete
{
    Dictionary<string, Dictionary<DateTime, bool>> UserOnlineHistory { get; set; }
    UserOnlineData GetUserHistoricalData(DateTime date, string userId);
    void AddUserData(DateTimeOffset currentDate, User user);
}

public class HistoricalDataStorageConcrete : IHistoricalDataStorageConcrete
{
    public Dictionary<string, Dictionary<DateTime, bool>> UserOnlineHistory { get; set; } = new Dictionary<string, Dictionary<DateTime, bool>>();

    public void DisplayHistoricalDataConcrete()
    {
        Console.WriteLine("\nHistorical User Online Data:");
        foreach (var userEntry in UserOnlineHistory)
        {
            Console.Write($"\nUser: {userEntry.Key}, ");
            foreach (var dateEntry in userEntry.Value)
            {
                Console.Write($"Date: {dateEntry.Key}, Online: {dateEntry.Value}");
            }
        }
    }
    
    public void AddUserData(DateTimeOffset time, User user)
    {
        if (user.LastSeenDate.HasValue)
        {
            if (!UserOnlineHistory.ContainsKey(user.Nickname))
            {
                UserOnlineHistory[user.Nickname] = new Dictionary<DateTime, bool>();
            }
            UserOnlineHistory[user.Nickname][time.DateTime] = true;
        }
    }
    
    public UserOnlineData GetUserHistoricalData(DateTime date, string userId)
    {
        if (!UserOnlineHistory.ContainsKey(userId))
        {
            return null;
        }

        if (!UserOnlineHistory[userId].ContainsKey(date))
        {
            DateTime? nearestTime = null;
            TimeSpan smallestDifference = TimeSpan.MaxValue;
            
            foreach (var time in UserOnlineHistory[userId].Keys)
            {
                var difference = time - date;

                if (Math.Abs(difference.TotalMinutes) < smallestDifference.TotalMinutes)
                {
                    nearestTime = time;
                    smallestDifference = difference;
                }
            }
            
            return new UserOnlineData { WasUserOnline = null, NearestOnlineTime = nearestTime };
        }

        if (UserOnlineHistory[userId][date])
        {
            return new UserOnlineData { WasUserOnline = true, NearestOnlineTime = null };
        }

        return new UserOnlineData { WasUserOnline = false, NearestOnlineTime = null };
    }
}

[ApiController]
[Route("api/stats")]
public class StatsControllerConcrete : ControllerBase
{
    private readonly IHistoricalDataStorage _historicalDataStorage;
    private readonly IHistoricalDataStorageConcrete _userHistoricalData;

    public StatsControllerConcrete(IHistoricalDataStorage historicalDataStorage, IHistoricalDataStorageConcrete userHistoricalData)
    {
        _historicalDataStorage = historicalDataStorage;
        _userHistoricalData = userHistoricalData;
    }
    
    [HttpGet("user")]
    public IActionResult GetUserOnlineData([FromQuery] DateTime date, [FromQuery] string userId)
    {
        var userOnlineData = _userHistoricalData.GetUserHistoricalData(date, userId);

        if (userOnlineData == null)
        {
            return NotFound();
        }

        return Ok(userOnlineData);
    }
}
