using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;
using LastSeenTaskAPI.Controllers;
namespace LastSeenTaskTests;

public class ForgetUserTests
{
    private Mock<IHistoricalDataStorageConcrete> _mockStorageConcrete;
    private Mock<IHistoricalDataStorage> _mockHistoricalDataStorage;
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
        _mockStorageConcrete = new Mock<IHistoricalDataStorageConcrete>(); 
        _mockHistoricalDataStorage = new Mock<IHistoricalDataStorage>();
        _mockShowUsers = new ShowUsers(_mockUserDataLoader.Object, _mockLastSeenFormatter.Object, _mockForgottenUsers);
        _controller = new StatsController(_mockHistoricalDataStorage.Object, _mockShowUsers, _mockStorageConcrete.Object);
    }

    [Test]
    public void When_ForgettingUser_Expect_UserIsRemoved()
    {
        var userId = "User1";
        var forgottenUsers = new List<string>();
        _mockStorageConcrete.Setup(storage => storage.ForgetUser(userId, forgottenUsers)).Verifiable();

        var actionResult = _controller.ForgetUser(userId, forgottenUsers);
        var okResult = actionResult as OkObjectResult;
        var resultUserId = (string)(okResult.Value.GetType().GetProperty("userId").GetValue(okResult.Value));

        Assert.IsNotNull(okResult);
        Assert.That(resultUserId, Is.EqualTo(userId));
        _mockStorageConcrete.Verify(storage => storage.ForgetUser(userId, forgottenUsers), Times.Once());
    }
}