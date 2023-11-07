using LastSeenTask;
using LastSeenTaskAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using LastSeenTaskAPI.Controllers;
namespace LastSeenIntegrationTests;

public class HistoricalDataStorageConcreteIntegrationTests
{
    private StatsController _statsController;
    private PredictionsController _predictionsController;
    private HistoricalDataStorageConcrete _dataStorage;
    private Mock<IReports> _mockReports;
    private ReportController _controllerReport;

    [SetUp]
    public void Setup()
    {
        _dataStorage = new HistoricalDataStorageConcrete();
        _statsController = new StatsController(null, null, _dataStorage);
        _predictionsController = new PredictionsController(null, _dataStorage);
        _mockReports = new Mock<IReports>();
        _controllerReport = new ReportController(_mockReports.Object);

    }

    [Test]
    public void ForgetUserIntegration_ReturnsExpectedValue()
    {
        string userId = "User123";
        _dataStorage.AddUserData(DateTimeOffset.UtcNow, new User { Nickname = userId, LastSeenDate = DateTimeOffset.UtcNow});

        var result = _statsController.ForgetUser(userId, new List<string>()) as OkObjectResult;
        var returnedUserId = result?.Value.GetType().GetProperty("userId").GetValue(result.Value) as string;
        
        Assert.IsNotNull(result);
        Assert.That(returnedUserId, Is.EqualTo(userId));
        Assert.IsFalse(_dataStorage.UserOnlineHistory.ContainsKey(userId));
    }

    [Test]
    public void QueryUserTotalOnlineTimeIntegration_ReturnsPositiveValue()
    {
        string userId = "User1";
        _dataStorage.AddUserData(DateTimeOffset.UtcNow, new User { Nickname = userId, LastSeenDate = DateTimeOffset.UtcNow});

        var result = _statsController.GetUserTotalOnlineTime(userId) as OkObjectResult;
        var totalTime = (long)result?.Value.GetType().GetProperty("totalTime").GetValue(result.Value);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(totalTime > 0);
    }

    [Test]
    public void GetUserAverageOnlineTimeIntegration_ReturnsPositiveValue()
    {
        string userId = "User2";
        _dataStorage.AddUserData(DateTimeOffset.UtcNow, new User { Nickname = userId, LastSeenDate = DateTimeOffset.UtcNow });

        var result = _statsController.GetUserAverageOnlineTime(userId) as OkObjectResult;
        var dailyAverage = (long)result?.Value.GetType().GetProperty("dailyAverage").GetValue(result.Value);

        Assert.IsNotNull(result);
        Assert.IsTrue(dailyAverage > 0);
    }
    
    [Test]
    public void When_GettingReport_Expect_SuccessfulResponse()
    {
        string reportName = "TestReport";

        var expectedReport = new Dictionary<string, ReportResult>
        {
            {
                "User1", new ReportResult("User1", new Dictionary<string, double>
                {
                    { "dailyAverage", 5.0 }
                })
            }
        };

        _mockReports.Setup(r => r.GetReport(reportName)).Returns(expectedReport);

        var actionResult = _controllerReport.GetReport(reportName, null, null);
        var okResult = actionResult as OkObjectResult;

        var reportResult = okResult.Value as Dictionary<string, ReportResult>;

        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }
}