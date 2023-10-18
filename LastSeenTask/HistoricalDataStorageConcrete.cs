using System.Globalization;
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
    double CalculateOnlineChance(DateTime date, double tolerance, string userId);
    long GetTotalOnlineTime(string userId);
    (long weeklyAverage, long dailyAverage) CalculateAverages(string userId);
    void ForgetUser(string userId);
}

public class HistoricalDataStorageConcrete : IHistoricalDataStorageConcrete
{
    public Dictionary<string, Dictionary<DateTime, bool>> UserOnlineHistory { get; set; } = 
        new Dictionary<string, Dictionary<DateTime, bool>>();

    public void DisplayHistoricalDataConcrete()
    {
        Console.WriteLine("\nHistorical User Online Data:");
        foreach (var users in UserOnlineHistory)
        {
            Console.Write($"\nUser: {users.Key}, ");
            foreach (var date in users.Value)
            {
                Console.Write($"Date: {date.Key}, Online: {date.Value}");
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
    
    public double CalculateOnlineChance(DateTime date, double tolerance, string userId)
    {
        if (!UserOnlineHistory.ContainsKey(userId))
        {
            return 0.0;
        }

        var userHistory = UserOnlineHistory[userId];
        var dayOfWeek = date.DayOfWeek;
        var hour = date.Hour;
        var minute = date.Minute;
        
        var toleranceMinutes = (int)(tolerance * 60);
    
        var wasOnlineCount = userHistory.Count(kvp => kvp.Key.DayOfWeek == dayOfWeek &&
                                                      kvp.Key.Hour >= hour - toleranceMinutes/60 && kvp.Key.Hour <= hour + toleranceMinutes/60 
                                                      && kvp.Key.Minute >= minute - toleranceMinutes%60 && 
                                                      kvp.Key.Minute <= minute + toleranceMinutes%60 && kvp.Value);
    
        var totalWeeks = userHistory.Keys.Max().Subtract(userHistory.Keys.Min()).Days / 7;

        if (totalWeeks == 0)
            return 0.0;

        return (double)wasOnlineCount / totalWeeks;
    }
    
    public long GetTotalOnlineTime(string userId)
    {
        if (!UserOnlineHistory.ContainsKey(userId))
        {
            return 0;
        }

        var lastSeenTime = UserOnlineHistory[userId].Keys.Max();
        var timeOnline = DateTime.Now - lastSeenTime;

        return (long)timeOnline.TotalSeconds;
    }
    
    public (long weeklyAverage, long dailyAverage) CalculateAverages(string userId)
    {
        if (!UserOnlineHistory.ContainsKey(userId))
        {
            return (0, 0);
        }

        var userHistory = UserOnlineHistory[userId];
    
        var totalSecondsOnlineDaily = userHistory
            .Where(kvp => kvp.Value)
            .GroupBy(kvp => kvp.Key.Date)
            .Select(group => group.Sum(kvp => (kvp.Key - group.Key).TotalSeconds))
            .Average();

        var totalSecondsOnlineWeekly = userHistory
            .Where(kvp => kvp.Value)
            .GroupBy(kvp => new { Year = kvp.Key.Year, Week = 
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(kvp.Key, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) })
            .Select(group => group.Sum(kvp => (kvp.Key - group.First().Key).TotalSeconds))
            .Average();

        return ((long)totalSecondsOnlineWeekly, (long)totalSecondsOnlineDaily);
    }
    
    public void ForgetUser(string userId)
    {
        if (UserOnlineHistory.ContainsKey(userId))
        {
            UserOnlineHistory.Remove(userId);
        }
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
    
    [HttpGet("user/total")]
    public IActionResult GetUserTotalOnlineTime([FromQuery] string userId)
    {
        var totalTime = _userHistoricalData.GetTotalOnlineTime(userId);
    
        return Ok(new { totalTime });
    }
    
    [HttpGet("user/average")]
    public IActionResult GetUserAverageOnlineTime([FromQuery] string userId)
    {
        var (weeklyAverage, dailyAverage) = _userHistoricalData.CalculateAverages(userId);
    
        return Ok(new 
        {
            weeklyAverage,
            dailyAverage
        });
    }

    [HttpPost("forget")]
    public IActionResult ForgetUser([FromQuery] string userId)
    {
        _userHistoricalData.ForgetUser(userId);
        return Ok(new 
        {
            userId = userId
        });
    }
}

[ApiController]
[Route("api/predictions")]
public class PredictionsControllerConcrete : ControllerBase
{
    private readonly IHistoricalDataStorageConcrete _userHistoricalData;

    public PredictionsControllerConcrete(IHistoricalDataStorageConcrete userHistoricalData)
    {
        _userHistoricalData = userHistoricalData;
    }

    [HttpGet("user")]
    public IActionResult GetPredictedUserOnline([FromQuery] DateTime date, [FromQuery] double tolerance, [FromQuery] string userId)
    {
        var onlineChance = _userHistoricalData.CalculateOnlineChance(date, tolerance, userId);
        var willBeOnline = onlineChance > tolerance;

        return Ok(new Dictionary<string, object>
        {
            ["willBeOnline"] = willBeOnline,
            ["onlineChance"] = onlineChance
        });

    }
}