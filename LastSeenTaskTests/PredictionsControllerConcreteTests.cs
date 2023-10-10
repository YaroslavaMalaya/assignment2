using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LastSeenTaskTests;

public class PredictionsControllerConcreteTests
{
    private Mock<IHistoricalDataStorageConcrete> _userHistoricalDataMock;
    private PredictionsControllerConcrete _controller;

    [SetUp]
    public void SetUp()
    {
        _userHistoricalDataMock = new Mock<IHistoricalDataStorageConcrete>();
        _controller = new PredictionsControllerConcrete(_userHistoricalDataMock.Object);
    }

    [Test]
    public void GetPredictedUserOnline_UserNeverOnline_ReturnsZeroChance()
    {
        var userId = "testUser";
        var date = new DateTime(2023, 10, 11, 14, 30, 0);
        var tolerance = 0.1;

        _userHistoricalDataMock.Setup(x => x.CalculateOnlineChance(date, tolerance, userId)).Returns(0.0);

        var result = _controller.GetPredictedUserOnline(date, tolerance, userId) as OkObjectResult;
        var responseData = (Dictionary<string, object>)result.Value;

        Assert.That(responseData["onlineChance"], Is.EqualTo(0.0));
        Assert.That(responseData["willBeOnline"], Is.EqualTo(false));
    }

    [Test]
    public void GetPredictedUserOnline_UserOftenOnline_ReturnsHighChance()
    {
        var userId = "testUser";
        var date = new DateTime(2023, 10, 11, 14, 30, 0);      
        var tolerance = 0.1;

        _userHistoricalDataMock.Setup(x => x.CalculateOnlineChance(date, tolerance, userId)).Returns(0.9);

        var result = _controller.GetPredictedUserOnline(date, tolerance, userId) as OkObjectResult;
        var responseData = result.Value as Dictionary<string, object>;

        Assert.That(responseData["onlineChance"], Is.EqualTo(0.9));
        Assert.That(responseData["willBeOnline"], Is.EqualTo(true));
    }
}