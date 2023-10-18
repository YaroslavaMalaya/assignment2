using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LastSeenTaskTests;

public class TotalTimeOnlineTests
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
    public void When_QueryingUserTotalOnlineTime_Expect_SuccessfulResponse()
    {
        var userId = "User1";
        var expectedTime = 10000L;
        _mockStorageConcrete.Setup(storage => storage.GetTotalOnlineTime(userId)).Returns(expectedTime);

        var actionResult = _controller.GetUserTotalOnlineTime(userId);
        var okResult = actionResult as OkObjectResult;
        var result = (long)(okResult.Value.GetType().GetProperty("totalTime").GetValue(okResult.Value));

        Assert.IsNotNull(okResult);
        Assert.That(result, Is.EqualTo(expectedTime));
    }
}