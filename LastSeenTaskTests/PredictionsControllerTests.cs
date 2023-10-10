using LastSeenTask;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
namespace LastSeenTaskTests;

public class PredictionsControllerTests
{
    [Test]
    public void GetPredictedUsersOnline_ShouldReturnAverageForGivenDayOfWeek()
    {
        var mockDataStorage = new Mock<IHistoricalDataStorage>();
        mockDataStorage.Setup(ds => ds.GetAverageUsersForDayOfWeek(DayOfWeek.Monday)).Returns(100);
            
        var controller = new PredictionsController(mockDataStorage.Object);
        var result = controller.GetPredictedUsersOnline(new DateTime(2023, 10, 16));
        var okResult = result as OkObjectResult;
        Assert.IsInstanceOf<OkObjectResult>(result);

        var returnData = JObject.FromObject(okResult?.Value);
        Assert.That(returnData["onlineUsers"].Value<int>(), Is.EqualTo(100));
    }

    [Test]
    public void GetPredictedUsersOnline_ShouldReturnZeroIfNoDataForGivenDayOfWeek()
    {
        var mockDataStorage = new Mock<IHistoricalDataStorage>();
        mockDataStorage.Setup(ds => ds.GetAverageUsersForDayOfWeek(DayOfWeek.Tuesday)).Returns(0);
            
        var controller = new PredictionsController(mockDataStorage.Object);
        var result = controller.GetPredictedUsersOnline(new DateTime(2023, 10, 15));
        var okResult = result as OkObjectResult;
        Assert.IsInstanceOf<OkObjectResult>(result);

        JObject returnData = JObject.FromObject(okResult?.Value);
        Assert.That(returnData["onlineUsers"].Value<int>(), Is.EqualTo(0));
    }
}