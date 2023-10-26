using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
namespace LastSeenTaskAPI.Controllers;

[ApiController]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly IHistoricalDataStorage _historicalDataStorage;
    private readonly IHistoricalDataStorageConcrete _userHistoricalData;
    private readonly IUserDataLoader _userDataLoader;
    private readonly ShowUsers _showUsers;

    public StatsController(IHistoricalDataStorage historicalDataStorage, ShowUsers showUsers, IHistoricalDataStorageConcrete userHistoricalData)
    {
        _historicalDataStorage = historicalDataStorage;
        _userHistoricalData = userHistoricalData;
        _showUsers = showUsers;
    }

    [HttpGet("users")]
    public IActionResult GetUsersOnline([FromQuery] DateTime date)
    {
        var usersResponses = _showUsers?.UsersShow("en");
        date = date.Date;
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
    
    [HttpGet("user")]
    public IActionResult GetUserOnlineData([FromQuery] DateTime date, [FromQuery] string userId)
    {
        var usersResponses = _showUsers?.UsersShow("en");
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
        var usersResponses = _showUsers?.UsersShow("en");
        var totalTime = _userHistoricalData.GetTotalOnlineTime(userId);
    
        return Ok(new { totalTime });
    }
    
    [HttpGet("user/average")]
    public IActionResult GetUserAverageOnlineTime([FromQuery] string userId)
    {
        var usersResponses = _showUsers?.UsersShow("en");
        var (weeklyAverage, dailyAverage) = _userHistoricalData.CalculateAverages(userId);
        return Ok(new 
        {
            weeklyAverage,
            dailyAverage
        });
    }

    [HttpPost("forget")]
    public IActionResult ForgetUser([FromQuery] string userId, [FromQuery] List<string> forgottenUsers)
    {
        var usersResponses = _showUsers?.UsersShow("en");
        _userHistoricalData.ForgetUser(userId, forgottenUsers);
        return Ok(new 
        {
            userId,
        });
    }
}
