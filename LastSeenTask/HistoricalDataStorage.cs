using Microsoft.AspNetCore.Mvc;
namespace LastSeenTask;

public interface IHistoricalDataStorage
{
    Dictionary<DateTime, int> UsersOnlineData { get; set; }
    int GetAverageUsersForDayOfWeek(DayOfWeek dayOfWeek);
}

public class HistoricalDataStorage : IHistoricalDataStorage
{
    public Dictionary<DateTime, int> UsersOnlineData { get; set; } = new Dictionary<DateTime, int>();

    public void DisplayHistoricalData()
    {
        Console.WriteLine("\nHistorical Data of Users Online:");
        foreach (var element in UsersOnlineData)
        {
            Console.WriteLine($"Date: {element.Key}, Users Online: {element.Value}");
        }
    }
    
    public int GetAverageUsersForDayOfWeek(DayOfWeek dayOfWeek)
    {
        var filteredData = UsersOnlineData.Where(element => element.Key.DayOfWeek == dayOfWeek);
        var keyValuePairs = filteredData.ToList();
        if (!keyValuePairs.Any()) return 0;
        return (int)keyValuePairs.Average(element => element.Value);
    }
}

[ApiController]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly IHistoricalDataStorage _historicalDataStorage;

    public StatsController(IHistoricalDataStorage historicalDataStorage)
    {
        _historicalDataStorage = historicalDataStorage;
    }

    [HttpGet("users")]
    public IActionResult GetUsersOnline([FromQuery] DateTime date)
    {
        int? usersOnlineCount = _historicalDataStorage.UsersOnlineData.GetValueOrDefault(date);

        if (usersOnlineCount.HasValue)
        {
            return Ok(new { usersOnline = usersOnlineCount.Value });
        }
        else
        {
            return Ok(new { usersOnline = (int?)null });
        }
    }
}

[ApiController]
[Route("api/predictions")]
public class PredictionsController : ControllerBase
{
    private readonly IHistoricalDataStorage _historicalDataStorage;

    public PredictionsController(IHistoricalDataStorage historicalDataStorage)
    {
        _historicalDataStorage = historicalDataStorage;
    }

    [HttpGet("users")]
    public IActionResult GetPredictedUsersOnline([FromQuery] DateTime date)
    {
        var onlineUsers = _historicalDataStorage.GetAverageUsersForDayOfWeek(date.DayOfWeek);

        return Ok(new { onlineUsers });
    }
}
 