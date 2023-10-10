using Microsoft.AspNetCore.Mvc;
namespace LastSeenTask;

public interface IHistoricalDataStorage
{
    Dictionary<DateTime, int> UsersOnlineData { get; set; }
}

public class HistoricalDataStorage : IHistoricalDataStorage
{
    public Dictionary<DateTime, int> UsersOnlineData { get; set; } = new Dictionary<DateTime, int>();

    public void DisplayHistoricalData()
    {
        Console.WriteLine("\nHistorical Data of Users Online:");
        foreach (var entry in UsersOnlineData)
        {
            Console.WriteLine($"Date: {entry.Key}, Users Online: {entry.Value}");
        }
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

 