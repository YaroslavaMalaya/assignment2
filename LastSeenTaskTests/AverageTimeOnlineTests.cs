using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LastSeenTaskTests;

public class AverageTimeOnlineTests
{
    private Mock<IHistoricalDataStorageConcrete> _mockStorageConcrete;
    private StatsControllerConcrete _controller;

    [SetUp]
    public void Setup()
    {
        _mockStorageConcrete = new Mock<IHistoricalDataStorageConcrete>();
        _controller = new StatsControllerConcrete(null, _mockStorageConcrete.Object);
    }

    [Test]
    public void When_QueryingUserAverageOnlineTime_Expect_SuccessfulResponse()
    {
        var userId = "User1";
        var expectedWeeklyAverage = 2500L;
        var expectedDailyAverage = 500L;

        _mockStorageConcrete.Setup(storage => storage.CalculateAverages(userId)).Returns((expectedWeeklyAverage, expectedDailyAverage));

        var actionResult = _controller.GetUserAverageOnlineTime(userId);
        var okResult = actionResult as OkObjectResult;

        var weeklyAverageResult = (long)(okResult.Value.GetType().GetProperty("weeklyAverage").GetValue(okResult.Value));
        var dailyAverageResult = (long)(okResult.Value.GetType().GetProperty("dailyAverage").GetValue(okResult.Value));

        Assert.IsNotNull(okResult);
        Assert.That(weeklyAverageResult, Is.EqualTo(expectedWeeklyAverage));
        Assert.That(dailyAverageResult, Is.EqualTo(expectedDailyAverage));
    }
}