using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
namespace LastSeenTaskAPI.Controllers;

[ApiController]
[Route("api/predictions")]
public class PredictionsController : ControllerBase
{
    private readonly IHistoricalDataStorage _historicalDataStorage;
    private readonly IHistoricalDataStorageConcrete _userHistoricalData;

    public PredictionsController(IHistoricalDataStorage historicalDataStorage, IHistoricalDataStorageConcrete userHistoricalData)
    {
        _historicalDataStorage = historicalDataStorage;
        _userHistoricalData = userHistoricalData;
    }

    [HttpGet("users")]
    public IActionResult GetPredictedUsersOnline([FromQuery] DateTime date)
    {
        var onlineUsers = _historicalDataStorage.GetAverageUsersForDayOfWeek(date.DayOfWeek);

        return Ok(new { onlineUsers });
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