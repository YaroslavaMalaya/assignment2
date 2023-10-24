using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;
using LastSeenTaskAPI.Controllers;
namespace LastSeenTaskTests;

public class AverageTimeOnlineTests
{
    private Mock<IHistoricalDataStorage> _mockStorage;
    private Mock<IHistoricalDataStorageConcrete> _mockHistoricalDataStorage;
    private ShowUsers _mockShowUsers;
    private StatsController _controller;
    private Mock<IUserDataLoader> _mockUserDataLoader;
    private Mock<ILastSeenFormatter> _mockLastSeenFormatter;
    private List<string> _mockForgottenUsers; 

    [SetUp]
    public void Setup()
    {
        _mockUserDataLoader = new Mock<IUserDataLoader>();
        _mockLastSeenFormatter = new Mock<ILastSeenFormatter>();
        _mockForgottenUsers = new List<string>();  
        _mockStorage = new Mock<IHistoricalDataStorage>(); 
        _mockHistoricalDataStorage = new Mock<IHistoricalDataStorageConcrete>();
        _mockShowUsers = new ShowUsers(_mockUserDataLoader.Object, _mockLastSeenFormatter.Object, _mockForgottenUsers);
        _controller = new StatsController(_mockStorage.Object, _mockShowUsers, _mockHistoricalDataStorage.Object);
    }

    [Test]
    public void When_QueryingUserAverageOnlineTime_Expect_SuccessfulResponse()
    {
        var userId = "User1";
        var expectedWeeklyAverage = 2500L;
        var expectedDailyAverage = 500L;

        _mockHistoricalDataStorage.Setup(storage => storage.CalculateAverages(userId)).Returns((expectedWeeklyAverage, expectedDailyAverage));

        var actionResult = _controller.GetUserAverageOnlineTime(userId);
        var okResult = actionResult as OkObjectResult;

        var weeklyAverageResult = (long)(okResult.Value.GetType().GetProperty("weeklyAverage").GetValue(okResult.Value));
        var dailyAverageResult = (long)(okResult.Value.GetType().GetProperty("dailyAverage").GetValue(okResult.Value));

        Assert.IsNotNull(okResult);
        Assert.That(weeklyAverageResult, Is.EqualTo(expectedWeeklyAverage));
        Assert.That(dailyAverageResult, Is.EqualTo(expectedDailyAverage));
    }
}