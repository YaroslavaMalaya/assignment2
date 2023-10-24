using LastSeenTask;
using LastSeenTaskAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace LastSeenTaskTests;

public class HistoricalDataStorageAllTests
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
    public void When_QueryingSpecificDate_Expect_CorrectUsersOnlineCount()
    {
        var expectedDate = new DateTime(2023, 10, 10);
        var expectedCount = 100;
        var data = new Dictionary<DateTime, int>
        {
            { expectedDate, expectedCount }
        };
        _mockStorage.SetupGet(storage => storage.UsersOnlineData).Returns(data);

        var result = _controller.GetUsersOnline(expectedDate);
        var okResult = result as OkObjectResult;
        var returnedValue = JsonConvert.DeserializeObject<Dictionary<string, int>>(JsonConvert.SerializeObject(okResult?.Value));

        Assert.NotNull(returnedValue);
        Assert.True(returnedValue.ContainsKey("usersOnline"));
        Assert.That(returnedValue["usersOnline"], Is.EqualTo(expectedCount));
    }
}