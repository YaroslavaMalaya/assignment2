using LastSeenTask;
using LastSeenTaskAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LastSeenTaskTests;

public class TotalTimeOnlineTests
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
    public void When_QueryingUserTotalOnlineTime_Expect_SuccessfulResponse()
    {
        var userId = "User1";
        var expectedTime = 10000L;
        _mockHistoricalDataStorage.Setup(storage => storage.GetTotalOnlineTime(userId)).Returns(expectedTime);

        var actionResult = _controller.GetUserTotalOnlineTime(userId);
        var okResult = actionResult as OkObjectResult;
        var result = (long)(okResult.Value.GetType().GetProperty("totalTime").GetValue(okResult.Value));

        Assert.IsNotNull(okResult);
        Assert.That(result, Is.EqualTo(expectedTime));
    }
}