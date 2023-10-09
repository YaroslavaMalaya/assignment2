using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace LastSeenTaskTests;

public class HistoricalDataStorageAllTests
{
    private Mock<IHistoricalDataStorage> _mockStorage;
    private StatsController _controller;
    
    [SetUp]
    public void SetUp()
    {
        _mockStorage = new Mock<IHistoricalDataStorage>();
        _controller = new StatsController(_mockStorage.Object);
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