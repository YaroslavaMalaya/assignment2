using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LastSeenTaskTests;

public class ForgetUserTests
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
    public void When_ForgettingUser_Expect_UserIsRemoved()
    {
        var userId = "User1";
        _mockStorageConcrete.Setup(storage => storage.ForgetUser(userId)).Verifiable();

        var actionResult = _controller.ForgetUser(userId);
        var okResult = actionResult as OkObjectResult;
        var resultUserId = (string)(okResult.Value.GetType().GetProperty("userId").GetValue(okResult.Value));

        Assert.IsNotNull(okResult);
        Assert.That(resultUserId, Is.EqualTo(userId));
        _mockStorageConcrete.Verify(storage => storage.ForgetUser(userId), Times.Once());
    }
}